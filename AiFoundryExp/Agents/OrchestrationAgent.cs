namespace AiFoundryExp.Agents;

/// <summary>
/// Manages workflow state and coordinates communication between all other agents.
/// </summary>
public class OrchestrationAgent : BaseAgent
{
    private readonly List<AgentMessage> _inbox = new();
    private readonly Dictionary<string, string> _context = new(StringComparer.OrdinalIgnoreCase);
    private HashSet<string> _knownAgents = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Engine controlling workflow progression. When set, the current phase will
    /// be used to choose which agents are activated.
    /// </summary>
    public OrchestrationEngine? Engine { get; set; }

    public OrchestrationAgent(AgentDefinition definition, IMessageBus bus) : base(definition, bus) { }

    protected override void OnMessageReceived(AgentMessage message)
    {
        lock (_inbox)
        {
            _inbox.Add(message);
        }
    }

    /// <summary>
    /// Determine which specialized agents should run based on the current phase and user inputs.
    /// </summary>
    public void ActivateNextAgents(IEnumerable<BaseAgent> agents)
    {
        ArgumentNullException.ThrowIfNull(agents);

        _knownAgents = new HashSet<string>(agents.Select(a => a.Name), StringComparer.OrdinalIgnoreCase);

        if (Engine is null)
        {
            return;
        }

        HashSet<string> active = Engine.GetActiveAgents().Select(a => a.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (BaseAgent agent in agents)
        {
            if (agent.Name.Equals(Name, StringComparison.OrdinalIgnoreCase))
                continue;

            if (active.Contains(agent.Name))
            {
                Send(agent.Name, "activate");
            }
        }
    }

    /// <summary>
    /// Maintain global context so all agents share consistent information.
    /// </summary>
    public void MaintainContext()
    {
        List<AgentMessage> messages;
        lock (_inbox)
        {
            messages = new List<AgentMessage>(_inbox);
            _inbox.Clear();
        }

        foreach (AgentMessage msg in messages)
        {
            string content = msg.Content;
            int sep = content.IndexOf('=');
            if (sep < 0)
                sep = content.IndexOf(':');
            if (sep > 0)
            {
                string key = content[..sep].Trim();
                string value = content[(sep + 1)..].Trim();
                if (!string.IsNullOrEmpty(key))
                {
                    _context[key] = value;
                }
            }
        }

        if (_knownAgents.Count == 0)
        {
            return;
        }

        string payload = string.Join(";", _context.Select(kv => $"{kv.Key}={kv.Value}"));
        foreach (string agentName in _knownAgents)
        {
            if (agentName.Equals(Name, StringComparison.OrdinalIgnoreCase))
                continue;

            Send(agentName, payload);
        }
    }

    /// <summary>
    /// Resolve contradictory recommendations from multiple agents. The selected
    /// decision is recorded in the provided decision log.
    /// </summary>
    public string ReconcileRecommendations(IEnumerable<AgentRecommendation> recommendations, DecisionLog log)
    {
        ArgumentNullException.ThrowIfNull(recommendations);
        ArgumentNullException.ThrowIfNull(log);

        var grouped = recommendations.GroupBy(r => r.Recommendation).ToList();

        // No conflict detected
        if (grouped.Count == 1)
        {
            var rec = grouped[0].First();
            log.AddEntry(rec.Recommendation, rec.Recommendation, "No conflict");
            return rec.Recommendation;
        }

        Console.WriteLine("Conflicting recommendations detected:");
        for (int i = 0; i < grouped.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {grouped[i].Key}");
            foreach (AgentRecommendation rec in grouped[i])
            {
                Console.WriteLine($"   - {rec.AgentName}: {rec.Justification}");
            }
        }

        Console.Write("Select the preferred option number: ");
        string? input = Console.ReadLine();
        if (int.TryParse(input, out int choice) && choice >= 1 && choice <= grouped.Count)
        {
            string chosen = grouped[choice - 1].Key;
            log.AddEntry("Conflict", chosen, $"User selected option {choice}");
            return chosen;
        }

        string defaultDecision = grouped[0].Key;
        log.AddEntry("Conflict", defaultDecision, "Defaulted to first option");
        return defaultDecision;
    }
}
