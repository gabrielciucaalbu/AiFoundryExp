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

}
