namespace AiFoundryExp;

public interface IMessageBus
{
    void Publish(AgentMessage message);
    void Subscribe(string recipient, Action<AgentMessage> handler);
    void Unsubscribe(string recipient, Action<AgentMessage> handler);
}
