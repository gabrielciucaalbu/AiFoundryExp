using Azure.AI.Agents.Persistent;
using System.Text.Json;

namespace AiFoundryExp;

class Program
{
    static async Task Main(string[] args)
    {
        string endpoint = Environment.GetEnvironmentVariable("PROJECT_ENDPOINT") ?? "https://your-aiservices-id.services.ai.azure.com/api/projects/your-project";
        string deployment = Environment.GetEnvironmentVariable("MODEL_DEPLOYMENT_NAME") ?? "your-model-deployment";

        string configPath = args.Length > 0 ? args[0] : "agents.json";
        AgentsConfiguration config;
        using (FileStream stream = File.OpenRead(configPath))
        {
            config = await JsonSerializer.DeserializeAsync<AgentsConfiguration>(stream) ?? new AgentsConfiguration();
        }

        AgentFactory factory = new AgentFactory(endpoint, deployment);

        foreach (AgentDefinition definition in config.Agents)
        {
            PersistentAgent agent = await factory.EnsureAgentAsync(definition);
            Console.WriteLine($"Ensured agent '{agent.Name}' with ID {agent.Id}");
        }
    }
}
