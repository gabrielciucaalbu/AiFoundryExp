using System.Text.Json;

namespace AiFoundryExp.Agents;

/// <summary>
/// Helper class to construct agent instances from the configuration file.
/// </summary>
public static class AgentRegistry
{
    public static IReadOnlyList<BaseAgent> LoadAgents(string configPath)
    {
        using FileStream stream = File.OpenRead(configPath);
        AgentsConfiguration config =
            JsonSerializer.Deserialize<AgentsConfiguration>(stream) ?? new AgentsConfiguration();

        List<BaseAgent> agents = new();
        foreach (AgentDefinition def in config.Agents)
        {
            BaseAgent? agent = def.Name switch
            {
                "Orchestration Agent" => new OrchestrationAgent(def),
                "Business Strategy Agent" => new BusinessStrategyAgent(def),
                "Requirements Gathering Agent" => new RequirementsGatheringAgent(def),
                "Technical Specification Agent" => new TechnicalSpecificationAgent(def),
                "Functional Design Agent" => new FunctionalDesignAgent(def),
                "User Interaction Agent" => new UserInteractionAgent(def),
                "Document Generation Agent" => new DocumentGenerationAgent(def),
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
