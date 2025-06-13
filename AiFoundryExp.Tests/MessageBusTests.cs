namespace AiFoundryExp.Tests;

public class MessageBusTests
{
    [Fact]
    public void Publish_ForwardsMessageToSubscriber()
    {
        MessageBus bus = new();
        AgentMessage? received = null;
        bus.Subscribe("B", m => received = m);

        AgentMessage message = new("A", "B", "hello");
        bus.Publish(message);

        Assert.NotNull(received);
        Assert.Equal(message, received);
    }

    [Fact]
    public void Unsubscribe_RemovesHandler()
    {
        MessageBus bus = new();
        AgentMessage? received = null;
        Action<AgentMessage> handler = m => received = m;
        bus.Subscribe("B", handler);
        bus.Unsubscribe("B", handler);

        bus.Publish(new AgentMessage("A", "B", "test"));
        Assert.Null(received);
    }
}
