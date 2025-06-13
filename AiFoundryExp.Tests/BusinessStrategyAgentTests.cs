using AiFoundryExp;
using AiFoundryExp.Agents;

namespace AiFoundryExp.Tests;

public class BusinessStrategyAgentTests
{
    private static BusinessStrategyAgent CreateAgent(IMessageBus bus)
        => new(new AgentDefinition { Name = "Business Strategy Agent" }, bus);

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
}
