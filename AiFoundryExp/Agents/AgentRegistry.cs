using System.Text.Json;

namespace AiFoundryExp.Agents;

/// <summary>
/// Helper class to construct agent instances from the configuration file.
/// </summary>
public static class AgentRegistry
{
    public static IReadOnlyList<BaseAgent> LoadAgents(string configPath, IMessageBus bus)
    {
        using FileStream stream = File.OpenRead(configPath);
        AgentsConfiguration config =
            JsonSerializer.Deserialize<AgentsConfiguration>(stream) ?? new AgentsConfiguration();

        List<BaseAgent> agents = new();
        foreach (AgentDefinition def in config.Agents)
        {
            BaseAgent? agent = def.Name switch
            {
                "Orchestration Agent" => new OrchestrationAgent(def, bus),
                "Business Strategy Agent" => new BusinessStrategyAgent(def, bus),
                "Document Generation Agent" => new DocumentGenerationAgent(def, bus),
                _ => null
            };

            if (agent is not null)
            {
                agents.Add(agent);
            }
        }

        return agents;
    }
}
