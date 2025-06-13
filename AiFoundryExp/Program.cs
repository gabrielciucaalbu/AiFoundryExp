using Azure.AI.Agents.Persistent;
using System.Text.Json;
using AiFoundryExp.Agents;

namespace AiFoundryExp;

class Program
{
    static async Task Main(string[] args)
    {
        string endpoint = Environment.GetEnvironmentVariable("PROJECT_ENDPOINT") ?? "https://your-aiservices-id.services.ai.azure.com/api/projects/your-project";
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
        UserInteractionAgent uiAgent = agents.OfType<UserInteractionAgent>().First();
        DocumentGenerationAgent docAgent = agents.OfType<DocumentGenerationAgent>().First();

        string inputFile = Path.Combine("input", "input.text");
        Queue<string> pendingResponses = new();
        if (File.Exists(inputFile))
        {
            pendingResponses.Enqueue(File.ReadAllText(inputFile).Trim());
        }

        using StreamWriter log = new(Path.Combine(outputDir, "conversation.log"), append: true);

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

            foreach (AgentDefinition agent in engine.GetActiveAgents())
            {
                string question = GetQuestion(engine.CurrentPhase, agent.Name);
                if (string.IsNullOrEmpty(question))
                {
                    Console.WriteLine($"Activating agent '{agent.Name}'");
                    continue;
                }

                string response;
                if (pendingResponses.Count > 0)
                {
                    response = pendingResponses.Dequeue();
                    Console.WriteLine($"{question} \n> {response}");
                }
                else
                {
                    response = uiAgent.AskQuestion(question);
                }

                uiAgent.ProcessResponse(response);
                if (agent.Name == "Document Generation Agent" &&
                    response.Trim().Equals("yes", StringComparison.OrdinalIgnoreCase))
                {
                    DocumentGenerationAgent docAgent = (DocumentGenerationAgent)agents.First(a => a is DocumentGenerationAgent);
                    docAgent.GenerateDocuments(outputDir);
                    log.WriteLine("Document generated.");
                }

                log.WriteLine($"{agent.Name}: {question}");
                log.WriteLine($"User: {response}");
                log.WriteLine();

                if (agent.Name == "Document Generation Agent" &&
                    response.Trim().Equals("yes", StringComparison.OrdinalIgnoreCase))
                {
                    log.WriteLine("Generating documents...");
                    docAgent.GenerateDocuments(Path.Combine(outputDir, "conversation.log"), outputDir);
                }
            }
        }
        while (engine.MoveNextPhase());

        engine.SaveDecisionLog(Path.Combine(outputDir, "decision_log.json"));
    }

    static string GetQuestion(WorkflowPhase phase, string agentName)
    {
        return (phase, agentName) switch
        {
            (WorkflowPhase.BusinessConceptDevelopment, "Business Strategy Agent")
                => "What is your core business idea?",
            (WorkflowPhase.RequirementsDiscovery, "Requirements Gathering Agent")
                => "What key features should the system have?",
            (WorkflowPhase.TechnicalSpecification, "Technical Specification Agent")
                => "Are there specific technologies you plan to use?",
            (WorkflowPhase.FunctionalDesign, "Functional Design Agent")
                => "Describe the main user workflow.",
            (WorkflowPhase.DocumentGenerationAndRefinement, "Document Generation Agent")
                => "Generate the final document? (yes/no)",
            _ => string.Empty
        };
    }
}
