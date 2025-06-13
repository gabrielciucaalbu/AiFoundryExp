using System.Text.Json;

namespace AiFoundryExp;

public class OrchestrationEngine
{
    private readonly Dictionary<string, AgentDefinition> _agents;
    public IMessageBus Bus { get; }

    public DecisionLog DecisionLog { get; } = new();

    public WorkflowPhase CurrentPhase { get; private set; } = WorkflowPhase.BusinessConceptDevelopment;

    private OrchestrationEngine(IEnumerable<AgentDefinition> agents)
    {
        _agents = agents.ToDictionary(a => a.Name, StringComparer.OrdinalIgnoreCase);
        Bus = new MessageBus();
    }

    public static async Task<OrchestrationEngine> LoadAsync(string configPath)
    {
        using FileStream stream = File.OpenRead(configPath);
        AgentsConfiguration config = await JsonSerializer.DeserializeAsync<AgentsConfiguration>(stream) ?? new AgentsConfiguration();
        return new OrchestrationEngine(config.Agents);
    }

    public IEnumerable<AgentDefinition> GetActiveAgents()
    {
        return CurrentPhase switch
        {
            WorkflowPhase.BusinessConceptDevelopment => FilterAgents("Business Strategy Agent", "User Interaction Agent"),
            WorkflowPhase.RequirementsDiscovery => FilterAgents("Requirements Gathering Agent", "Business Strategy Agent", "User Interaction Agent"),
            WorkflowPhase.TechnicalSpecification => FilterAgents("Technical Specification Agent", "Requirements Gathering Agent", "Business Strategy Agent", "User Interaction Agent"),
            WorkflowPhase.FunctionalDesign => FilterAgents("Functional Design Agent", "Technical Specification Agent", "User Interaction Agent", "Requirements Gathering Agent"),
            WorkflowPhase.DocumentGenerationAndRefinement => FilterAgents("Document Generation Agent", "User Interaction Agent"),
            _ => Enumerable.Empty<AgentDefinition>()
        };
    }

    private IEnumerable<AgentDefinition> FilterAgents(params string[] names)
    {
        foreach (string name in names)
        {
            if (_agents.TryGetValue(name, out AgentDefinition? agent))
            {
                yield return agent;
            }
        }
    }

    public bool MoveNextPhase()
    {
        if (CurrentPhase == WorkflowPhase.DocumentGenerationAndRefinement)
        {
            return false;
        }

        CurrentPhase = (WorkflowPhase)((int)CurrentPhase + 1);
        return true;
    }

    public void SaveDecisionLog(string path) => DecisionLog.Save(path);
}
