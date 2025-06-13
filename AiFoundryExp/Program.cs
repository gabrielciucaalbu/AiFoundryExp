using Azure.AI.Agents.Persistent;
using System.Text.Json;
using AiFoundryExp.Agents;

namespace AiFoundryExp;

class Program
{
    static async Task Main(string[] args)
    {
        string endpoint = Environment.GetEnvironmentVariable("PROJECT_ENDPOINT") ?? "https://agentic-experim-resource.services.ai.azure.com/api/projects/agentic-experim";
        string deployment = Environment.GetEnvironmentVariable("MODEL_DEPLOYMENT_NAME") ?? "your-model-deployment";

        string configPath = args.Length > 0 ? args[0] : Path.Combine("Configuration", "agents.json");
        AgentsConfiguration config;
        using (FileStream stream = File.OpenRead(configPath))
        {
            config = await JsonSerializer.DeserializeAsync<AgentsConfiguration>(stream) ?? new AgentsConfiguration();
        }

        string outputDir = "output";
        Directory.CreateDirectory(outputDir);
        string statePath = Path.Combine(outputDir, "state.json");
        string messageLog = Path.Combine(outputDir, "messages.log");
        OrchestrationEngine engine = await OrchestrationEngine.LoadAsync(configPath, statePath, messageLog);

        IReadOnlyList<BaseAgent> agents = AgentRegistry.LoadAgents(configPath, engine.Bus);
        Dictionary<string, BaseAgent> agentMap = agents.ToDictionary(a => a.Name);
        UserInteractionAgent uiAgent = (UserInteractionAgent)agentMap["User Interaction Agent"];
        DocumentGenerationAgent docAgent = (DocumentGenerationAgent)agentMap["Document Generation Agent"];

        string inputFile = Path.Combine("input", "input.text");
        Dictionary<string, string> context = new();
        if (File.Exists(inputFile))
        {
            string initial = File.ReadAllText(inputFile).Trim();
            if (!string.IsNullOrEmpty(initial))
            {
                context["business_idea"] = initial;
            }
        }

        using FileStream logStream =
            new(Path.Combine(outputDir, "conversation.log"),
                FileMode.Append, FileAccess.Write, FileShare.Read);
        using StreamWriter log = new(logStream);

        AgentFactory factory = new AgentFactory(endpoint, deployment);

        foreach (AgentDefinition definition in config.Agents)
        {
            PersistentAgent agent = await factory.EnsureAgentAsync(definition);
            Console.WriteLine($"Ensured agent '{agent.Name}' with ID {agent.Id}");
        }

        do
        {
            Console.WriteLine($"\n--- {engine.CurrentPhase} ---");
            log.WriteLine($"--- {engine.CurrentPhase} ---");

            foreach (AgentDefinition agentDef in engine.GetActiveAgents())
            {
                if (agentDef.Name == "User Interaction Agent")
                {
                    continue;
                }

                BaseAgent agent = agentMap[agentDef.Name];

                while (true)
                {
                    string? question = agent.GenerateNextQuestion(context);
                    if (string.IsNullOrEmpty(question))
                    {
                        Console.WriteLine($"Activating agent '{agentDef.Name}' with no questions.");
                        break;
                    }

                    string response = uiAgent.AskQuestion(question);

                    uiAgent.ProcessResponse(response);
                    agent.ProcessAnswer(response, context);

                    if (agent is DocumentGenerationAgent &&
                        response.Trim().Equals("yes", StringComparison.OrdinalIgnoreCase))
                    {
                        log.WriteLine("Generating documents...");
                        docAgent.GenerateDocuments(Path.Combine(outputDir, "conversation.log"), outputDir);
                        log.WriteLine("Document generated.");
                    }

                    log.WriteLine($"{agentDef.Name}: {question}");
                    log.WriteLine($"User: {response}");
                    log.WriteLine();
                }
            }
        }
        while (engine.MoveNextPhase());

        engine.SaveDecisionLog(Path.Combine(outputDir, "decision_log.json"));
    }

    // Questions are now generated dynamically by each agent and not defined here.
}
