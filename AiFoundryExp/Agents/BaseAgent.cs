namespace AiFoundryExp.Agents;

public abstract class BaseAgent
{
    public string Name { get; }
    public string Instructions { get; protected set; }
    protected IMessageBus Bus { get; }

    protected BaseAgent(AgentDefinition definition, IMessageBus bus)
    {
        Name = definition.Name;
        Instructions = definition.Instructions;
        Bus = bus;
        Bus.Subscribe(Name, OnMessageReceived);
    }

    /// <summary>
    /// Send a text message to another agent via the shared bus.
    /// </summary>
    protected void Send(string recipient, string content)
    {
        Bus.Publish(new AgentMessage(Name, recipient, content));
    }

    /// <summary>
    /// Override to handle incoming messages.
    /// </summary>
    protected virtual void OnMessageReceived(AgentMessage message) { }
}
