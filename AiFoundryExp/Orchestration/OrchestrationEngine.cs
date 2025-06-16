using System.Text.Json;

namespace AiFoundryExp;

public class OrchestrationEngine
{
    private readonly Dictionary<string, AgentDefinition> _agents;
    public IMessageBus Bus { get; }

    public DecisionLog DecisionLog { get; } = new();

    private OrchestrationEngine(IEnumerable<AgentDefinition> agents, string? messageLog)
    {
        _agents = agents.ToDictionary(a => a.Name, StringComparer.OrdinalIgnoreCase);
        Bus = new MessageBus(messageLog);
    }

    public static async Task<OrchestrationEngine> LoadAsync(string configPath, string? messageLog = null)
    {
        using FileStream stream = File.OpenRead(configPath);
        AgentsConfiguration config = await JsonSerializer.DeserializeAsync<AgentsConfiguration>(stream) ?? new AgentsConfiguration();
        return new OrchestrationEngine(config.Agents, messageLog);
    }

    public IEnumerable<AgentDefinition> GetActiveAgents()
    {
        return _agents.Values.Where(a =>
            !a.Name.Equals("Orchestration Agent", StringComparison.OrdinalIgnoreCase));
    }

    public void SaveDecisionLog(string path) => DecisionLog.Save(path);
}
