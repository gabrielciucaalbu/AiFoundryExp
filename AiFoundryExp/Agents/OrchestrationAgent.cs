namespace AiFoundryExp.Agents;

/// <summary>
/// Manages workflow state and coordinates communication between all other agents.
/// </summary>
public class OrchestrationAgent : BaseAgent
{
    public OrchestrationAgent(AgentDefinition definition, IMessageBus bus) : base(definition, bus) { }

    /// <summary>
    /// Determine which specialized agents should run based on the current phase and user inputs.
    /// </summary>
    public void ActivateNextAgents(IEnumerable<BaseAgent> agents)
    {
        // Implementation would contain logic to choose and schedule agents.
    }

    /// <summary>
    /// Maintain global context so all agents share consistent information.
    /// </summary>
    public void MaintainContext()
    {
        // Implementation would update and distribute shared context.
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
