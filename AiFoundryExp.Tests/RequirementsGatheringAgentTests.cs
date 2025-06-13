using AiFoundryExp.Agents;
using System.Text.Json;
using System.Linq;

namespace AiFoundryExp.Tests;

public class RequirementsGatheringAgentTests
{
    [Fact]
    public void CollectRequirements_CreatesRequirementsAndForwards()
    {
        MessageBus bus = new();
        List<AgentMessage> messages = new();
        bus.Subscribe("Technical Specification Agent", m => messages.Add(m));
        bus.Subscribe("Functional Design Agent", m => messages.Add(m));

        RequirementsGatheringAgent agent = new(new AgentDefinition { Name = "Req" }, bus);
        var context = new Dictionary<string, string>
        {
            ["key_features"] = "invoicing, budgeting",
            ["business_idea"] = "Automate invoices"
        };

        agent.CollectRequirements(context);

        Assert.True(context.ContainsKey("technical_requirements"));
        var reqs = JsonSerializer.Deserialize<List<string>>(context["technical_requirements"]);
        Assert.NotNull(reqs);
        Assert.Equal(2, reqs!.Count);
        Assert.Equal(reqs!.Count * 2, messages.Count);
    }

    [Fact]
    public void MaintainTraceability_PersistsMapping()
    {
        MessageBus bus = new();
        RequirementsGatheringAgent agent = new(new AgentDefinition { Name = "Req" }, bus);
        var context = new Dictionary<string, string>
        {
            ["key_features"] = "reporting",
            ["business_idea"] = "Automate invoices"
        };

        agent.CollectRequirements(context);
        agent.MaintainTraceability(context);

        Assert.True(context.ContainsKey("traceability"));
        var map = JsonSerializer.Deserialize<Dictionary<string, string>>(context["traceability"]);
        Assert.NotNull(map);
        Assert.Single(map!);
        Assert.Equal("Automate invoices", map!.Values.First());
    }
}
