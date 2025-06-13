using AiFoundryExp.Agents;
using System.Text.Json;

namespace AiFoundryExp.Tests;

public class UserInteractionAgentTests
{
    [Fact]
    public void ProcessResponse_PublishesParsedTokens()
    {
        MessageBus bus = new();
        AgentMessage? received = null;
        bus.Subscribe("Orchestration Agent", m => received = m);

        UserInteractionAgent agent = new(new AgentDefinition { Name = "User Interaction Agent" }, bus);
        agent.ProcessResponse("foo: bar\nbaz=qux");

        Assert.NotNull(received);
        var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(received!.Content);
        Assert.Equal("User Interaction Agent", received!.Sender);
        Assert.Equal("Orchestration Agent", received!.Recipient);
        Assert.NotNull(dict);
        Assert.Equal("bar", dict!["foo"]);
        Assert.Equal("qux", dict!["baz"]);
    }

    [Fact]
    public void ProcessResponse_ParsesJson()
    {
        MessageBus bus = new();
        AgentMessage? received = null;
        bus.Subscribe("Orchestration Agent", m => received = m);

        UserInteractionAgent agent = new(new AgentDefinition { Name = "User Interaction Agent" }, bus);
        agent.ProcessResponse("{\"x\":\"1\",\"y\":\"2\"}");

        Assert.NotNull(received);
        var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(received!.Content);
        Assert.NotNull(dict);
        Assert.Equal("1", dict!["x"]);
        Assert.Equal("2", dict!["y"]);
    }
}
