using System.Collections.Concurrent;

namespace AiFoundryExp;

public class MessageBus : IMessageBus
{
    private readonly ConcurrentDictionary<string, List<Action<AgentMessage>>> _handlers = new();
    private readonly string? _logFile;

    public MessageBus(string? logFile = null)
    {
        _logFile = logFile;
    }

    public void Publish(AgentMessage message)
    {
        if (_logFile is not null)
        {
            string entry = $"{DateTime.UtcNow:o} {message.Sender}->{message.Recipient}: {message.Content}";
            File.AppendAllText(_logFile, entry + Environment.NewLine);
        }
        if (_handlers.TryGetValue(message.Recipient, out List<Action<AgentMessage>>? handlers))
        {
            foreach (Action<AgentMessage> handler in handlers.ToArray())
            {
                handler(message);
            }
        }
    }

    public void Subscribe(string recipient, Action<AgentMessage> handler)
    {
        _handlers.AddOrUpdate(recipient,
            _ => new List<Action<AgentMessage>> { handler },
            (_, list) => { lock (list) { list.Add(handler); } return list; });
    }

    public void Unsubscribe(string recipient, Action<AgentMessage> handler)
    {
        if (_handlers.TryGetValue(recipient, out List<Action<AgentMessage>>? list))
        {
            lock (list)
            {
                list.Remove(handler);
                if (list.Count == 0)
                {
                    _handlers.TryRemove(recipient, out _);
                }
            }
        }
    }
}
