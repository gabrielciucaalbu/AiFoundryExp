using System.IO;
using Xunit;
using AiFoundryExp;
using AiFoundryExp.Agents;

namespace AiFoundryExp.Tests;

public class OrchestrationAgentTests
{
    private static OrchestrationAgent CreateAgent() =>
        new(new AgentDefinition { Name = "Orch" }, new MessageBus());

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
}
