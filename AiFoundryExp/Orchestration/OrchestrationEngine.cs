using System.Text.Json;

namespace AiFoundryExp;

public class OrchestrationEngine
{
    private readonly Dictionary<string, AgentDefinition> _agents;
    public IMessageBus Bus { get; }
    private readonly string _statePath;

    public DecisionLog DecisionLog { get; } = new();

    public WorkflowPhase CurrentPhase { get; private set; } = WorkflowPhase.BusinessConceptDevelopment;

    private OrchestrationEngine(IEnumerable<AgentDefinition> agents, string statePath, string? messageLog)
    {
        _agents = agents.ToDictionary(a => a.Name, StringComparer.OrdinalIgnoreCase);
        Bus = new MessageBus(messageLog);
        _statePath = statePath;
        LoadState();
    }

    public static async Task<OrchestrationEngine> LoadAsync(string configPath, string statePath, string? messageLog = null)
    {
        using FileStream stream = File.OpenRead(configPath);
        AgentsConfiguration config = await JsonSerializer.DeserializeAsync<AgentsConfiguration>(stream) ?? new AgentsConfiguration();
        return new OrchestrationEngine(config.Agents, statePath, messageLog);
    }

    public IEnumerable<AgentDefinition> GetActiveAgents()
    {
        return CurrentPhase switch
        {
            WorkflowPhase.BusinessConcept => FilterAgents("Business Strategy Agent"),
            WorkflowPhase.DocumentGeneration => FilterAgents("Document Generation Agent"),
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
        if (CurrentPhase == WorkflowPhase.DocumentGeneration)
        {
            return false;
        }

        CurrentPhase = WorkflowPhase.DocumentGeneration;
        SaveState();
        return true;
    }

    public void SaveDecisionLog(string path) => DecisionLog.Save(path);

    private void LoadState()
    {
        EngineState state = FileStorage.LoadState(_statePath);
        CurrentPhase = state.CurrentPhase;
    }

    private void SaveState()
    {
        FileStorage.SaveState(_statePath, new EngineState { CurrentPhase = CurrentPhase });
    }
}
