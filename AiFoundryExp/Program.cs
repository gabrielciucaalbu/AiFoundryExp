using Azure.AI.Agents.Persistent;
using System.Text.Json;
using System;
using AiFoundryExp.Agents;

namespace AiFoundryExp;

class Program
{
    static async Task Main(string[] args)
    {
        string endpoint = Environment.GetEnvironmentVariable("PROJECT_ENDPOINT") ?? "https://agentic-experim-resource.services.ai.azure.com/api/projects/agentic-experim";
        string deployment = Environment.GetEnvironmentVariable("MODEL_DEPLOYMENT_NAME") ?? "gpt-4.1";

        string configPath = args.Length > 0 ? args[0] : Path.Combine("Configuration", "agents.json");

        AgentsConfiguration config = await LoadConfiguration(configPath);

        string outputDir = InitializeOutputDirectory();

        string statePath = Path.Combine(outputDir, "state.json");
        string messageLog = Path.Combine(outputDir, "agents_comm.log");

        OrchestrationEngine engine = await OrchestrationEngine.LoadAsync(configPath, statePath, messageLog);

        Dictionary<string, BaseAgent> agentMap = InitializeAgents(configPath, engine);
        BusinessStrategyAgent strategy = (BusinessStrategyAgent)agentMap["Business Strategy Agent"];
        DocumentGenerationAgent docAgent = (DocumentGenerationAgent)agentMap["Document Generation Agent"];
        
        string inputFile = Path.Combine("input", "input.text");
        Dictionary<string, string> context = InputParser.ParseFile(inputFile);


        AgentFactory factory = new AgentFactory(endpoint, deployment);
        await RegisterPersistentAgents(config, factory, engine, endpoint);

        RunWorkflow(agentMap, strategy, docAgent, context, outputDir);

        engine.SaveDecisionLog(Path.Combine(outputDir, "decision_log.json"));
    }

    private static async Task<AgentsConfiguration> LoadConfiguration(string path)
    {
        using FileStream stream = File.OpenRead(path);
        return await JsonSerializer.DeserializeAsync<AgentsConfiguration>(stream) ?? new AgentsConfiguration();
    }

    private static string InitializeOutputDirectory()
    {
        const string dir = "output";
        Directory.CreateDirectory(dir);
        return dir;
    }

    private static Dictionary<string, BaseAgent> InitializeAgents(string configPath, OrchestrationEngine engine)
    {
        IReadOnlyList<BaseAgent> agents = AgentRegistry.LoadAgents(configPath, engine.Bus);
        return agents.ToDictionary(a => a.Name);
    }

    private static async Task RegisterPersistentAgents(AgentsConfiguration config, AgentFactory factory, OrchestrationEngine engine, string defaultEndpoint)
    {
        foreach (AgentDefinition definition in config.Agents)
        {
            PersistentAgent agent = await factory.EnsureAgentAsync(definition);
            Console.WriteLine($"Ensured agent '{agent.Name}' with ID {agent.Id}");
            PersistentAgentsClient client = factory.GetClient(string.IsNullOrWhiteSpace(definition.Endpoint) ? defaultEndpoint : definition.Endpoint!);
            engine.Bus.RegisterRemoteAgent(definition.Name, client, agent);
        }
    }

    private static void RunWorkflow(
        Dictionary<string, BaseAgent> agentMap,
        BusinessStrategyAgent strategy,
        DocumentGenerationAgent docAgent,
        Dictionary<string, string> context,
        string outputDir)
    {
        OrchestrationAgent orch = (OrchestrationAgent)agentMap["Orchestration Agent"];
        orch.ActivateNextAgents(agentMap.Values);
        orch.MaintainContext();

        BusinessModel model = strategy.BuildBusinessModel(context);
        docAgent.GenerateDocuments(model, outputDir);
    }
}
