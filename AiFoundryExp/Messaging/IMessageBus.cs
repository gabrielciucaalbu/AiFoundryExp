using Azure.AI.Agents.Persistent;

namespace AiFoundryExp;

public interface IMessageBus
{
    void Publish(AgentMessage message);
    void Subscribe(string recipient, Action<AgentMessage> handler);
    void Unsubscribe(string recipient, Action<AgentMessage> handler);
    void RegisterRemoteAgent(string name, PersistentAgentsClient client, PersistentAgent agent);
    string Prompt(string recipient, string prompt);
}
