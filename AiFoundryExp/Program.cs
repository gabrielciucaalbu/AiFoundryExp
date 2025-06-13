using Azure.AI.Agents.Persistent;

namespace AiFoundryExp;

class Program
{
    static async Task Main(string[] args)
    {
        string endpoint = Environment.GetEnvironmentVariable("PROJECT_ENDPOINT") ?? "https://your-aiservices-id.services.ai.azure.com/api/projects/your-project";
        string deployment = Environment.GetEnvironmentVariable("MODEL_DEPLOYMENT_NAME") ?? "your-model-deployment";

        AgentFactory factory = new AgentFactory(endpoint, deployment);
        PersistentAgent agent = await factory.CreateAgentAsync(
            name: "Sample Agent",
            instructions: "You are a helpful assistant.",
            temperature: 0.7f);

        Console.WriteLine($"Agent '{agent.Name}' created with ID {agent.Id}");
    }
}
