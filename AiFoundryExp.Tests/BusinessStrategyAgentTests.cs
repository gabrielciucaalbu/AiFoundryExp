using AiFoundryExp;
using AiFoundryExp.Agents;
using Azure.AI.Agents.Persistent;

namespace AiFoundryExp.Tests;

public class BusinessStrategyAgentTests
{
    private static BusinessStrategyAgent CreateAgent(IMessageBus bus)
        => new(new AgentDefinition { Name = "Business Strategy Agent" }, bus);

    private class TestBus : IMessageBus
    {
        public string? PromptedText;
        public string Prompt(string recipient, string prompt)
        {
            PromptedText = prompt;
            return "none";
        }

        public void Publish(AgentMessage message) { }
        public void Subscribe(string recipient, Action<AgentMessage> handler) { }
        public void Unsubscribe(string recipient, Action<AgentMessage> handler) { }
        public void RegisterRemoteAgent(string name, PersistentAgentsClient client, PersistentAgent agent) { }
    }

    [Fact]
    public void BuildBusinessModel_MapsContextValues()
    {
        MessageBus bus = new();
        BusinessStrategyAgent agent = CreateAgent(bus);
        var context = new Dictionary<string, string>
        {
            ["business_idea"] = "AI SaaS",
            ["target_market"] = "SMBs",
            ["revenue_model"] = "subscription"
        };

        BusinessModel model = agent.BuildBusinessModel(context);

        Assert.Equal("AI SaaS", model.BusinessIdea);
        Assert.Equal("SMBs", model.TargetMarket);
        Assert.Equal("subscription", model.RevenueModel);
    }

    [Fact]
    public void RequestClarification_SendsMessagesForMissingFields()
    {
        MessageBus bus = new();
        List<AgentMessage> received = new();
        bus.Subscribe("User Interaction Agent", m => received.Add(m));
        BusinessStrategyAgent agent = CreateAgent(bus);
        var context = new Dictionary<string, string>
        {
            ["business_idea"] = "Idea"
        };

        agent.RequestClarification(context);

        Assert.Equal(2, received.Count);
        Assert.All(received, m => Assert.Equal("User Interaction Agent", m.Recipient));
        Assert.Contains(received, m => m.Content.Contains("target market", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(received, m => m.Content.Contains("revenue model", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void GenerateNextQuestion_LoadsInputFile()
    {
        TestBus bus = new();
        BusinessStrategyAgent agent = CreateAgent(bus);
        var context = new Dictionary<string, string>();

        string dir = Path.Combine(AppContext.BaseDirectory, "input");
        Directory.CreateDirectory(dir);
        string file = Path.Combine(dir, "input.txt");
        File.WriteAllText(file, "target_market: testers");
        try
        {
            agent.GenerateNextQuestion(context);
            Assert.Equal("testers", context["target_market"]);
        }
        finally
        {
            File.Delete(file);
            if (Directory.Exists(dir) && Directory.GetFileSystemEntries(dir).Length == 0)
                Directory.Delete(dir);
        }
    }
}
