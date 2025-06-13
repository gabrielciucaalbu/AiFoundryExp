using AiFoundryExp.Agents;
using AiFoundryExp;

namespace AiFoundryExp.Tests;

public class OrchestrationAgentTests
{
    private class TestBus : IMessageBus
    {
        private readonly MessageBus _inner = new();
        public List<AgentMessage> Messages { get; } = new();

        public void Publish(AgentMessage message)
        {
            Messages.Add(message);
            _inner.Publish(message);
        }

        public void Subscribe(string recipient, Action<AgentMessage> handler) => _inner.Subscribe(recipient, handler);

        public void Unsubscribe(string recipient, Action<AgentMessage> handler) => _inner.Unsubscribe(recipient, handler);
    }

    private static OrchestrationAgent CreateAgent() =>
        new(new AgentDefinition { Name = "Orch" }, new MessageBus());

    private static async Task<(OrchestrationAgent orch, List<BaseAgent> agents, TestBus bus, OrchestrationEngine engine)> CreateEngineAndAgentsAsync()
    {
        string configPath = Path.GetFullPath(Path.Combine("..", "..", "..", "..", "AiFoundryExp", "Configuration", "agents.json"));
        string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDir);
        string statePath = Path.Combine(tempDir, "state.json");

        OrchestrationEngine engine = await OrchestrationEngine.LoadAsync(configPath, statePath);
        TestBus bus = new();
        List<BaseAgent> agents = AgentRegistry.LoadAgents(configPath, bus).ToList();
        OrchestrationAgent orch = (OrchestrationAgent)agents.First(a => a is OrchestrationAgent);
        orch.Engine = engine;
        return (orch, agents, bus, engine);
    }

    [Fact]
    public void ReconcileRecommendations_ReturnsSingleRecommendation()
    {
        OrchestrationAgent agent = CreateAgent();
        DecisionLog log = new();
        var recs = new[] { new AgentRecommendation("A", "Option1", "") };

        string result = agent.ReconcileRecommendations(recs, log);

        Assert.Equal("Option1", result);
        Assert.Single(log.Entries);
        Assert.Equal("No conflict", log.Entries[0].Rationale);
    }

    [Fact]
    public void ReconcileRecommendations_UsesUserSelection()
    {
        OrchestrationAgent agent = CreateAgent();
        DecisionLog log = new();
        var recs = new[]
        {
            new AgentRecommendation("A", "First", ""),
            new AgentRecommendation("B", "Second", "")
        };

        TextReader original = Console.In;
        try
        {
            Console.SetIn(new StringReader("2"));
            string result = agent.ReconcileRecommendations(recs, log);
            Assert.Equal("Second", result);
            Assert.Equal("User selected option 2", log.Entries.Last().Rationale);
        }
        finally
        {
            Console.SetIn(original);
        }
    }

    [Fact]
    public async Task ActivateNextAgents_SendsActivationToPhaseAgents()
    {
        var (orch, agents, bus, engine) = await CreateEngineAndAgentsAsync();

        orch.ActivateNextAgents(agents);

        var expected = engine.GetActiveAgents().Select(a => a.Name);
        foreach (string name in expected)
        {
            Assert.Contains(bus.Messages, m => m.Recipient == name && m.Content == "activate");
        }

        foreach (string name in agents.Where(a => a.Name != orch.Name && !expected.Contains(a.Name)).Select(a => a.Name))
        {
            Assert.DoesNotContain(bus.Messages, m => m.Recipient == name && m.Content == "activate");
        }
    }

    [Fact]
    public async Task MaintainContext_BroadcastsAggregatedUpdates()
    {
        var (orch, agents, bus, _) = await CreateEngineAndAgentsAsync();

        orch.ActivateNextAgents(agents);

        bus.Publish(new AgentMessage("Business Strategy Agent", orch.Name, "target_market=SMEs"));
        bus.Publish(new AgentMessage("Requirements Gathering Agent", orch.Name, "key_features=login"));

        int before = bus.Messages.Count;
        orch.MaintainContext();

        int expectedMessages = before + agents.Count - 1; // broadcast to all except orchestration agent
        Assert.Equal(expectedMessages, bus.Messages.Count);

        var sent = bus.Messages.Skip(before).ToList();
        foreach (BaseAgent agent in agents.Where(a => a.Name != orch.Name))
        {
            Assert.Contains(sent, m => m.Recipient == agent.Name && m.Content.Contains("target_market=SMEs") && m.Content.Contains("key_features=login"));
        }
    }
}
