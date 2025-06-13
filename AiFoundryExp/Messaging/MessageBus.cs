using System.Collections.Concurrent;
using Azure.AI.Agents.Persistent;

namespace AiFoundryExp;

public class MessageBus : IMessageBus
{
    private readonly ConcurrentDictionary<string, List<Action<AgentMessage>>> _handlers = new();
    private readonly string? _logFile;

    private readonly Dictionary<string, (PersistentAgentsClient Client, PersistentAgent Agent)> _remoteAgents = new(StringComparer.OrdinalIgnoreCase);

    public MessageBus(string? logFile = null)
    {
        _logFile = logFile;
    }

    public void RegisterRemoteAgent(string name, PersistentAgentsClient client, PersistentAgent agent)
    {
        _remoteAgents[name] = (client, agent);
    }

    public void Publish(AgentMessage message)
    {
        if (_logFile is not null)
        {
            string entry = $"{DateTime.UtcNow:o} {message.Sender}->{message.Recipient}: {message.Content}";
            File.AppendAllText(_logFile, entry + Environment.NewLine);
        }

        if (_remoteAgents.TryGetValue(message.Recipient, out var remote))
        {
            string reply = SendPromptAsync(remote.Client, remote.Agent, message.Content).GetAwaiter().GetResult();
            Dispatch(new AgentMessage(message.Recipient, message.Sender, reply));
            return;
        }

        Dispatch(message);
    }

    private void Dispatch(AgentMessage message)
    {
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

    private static async Task<string> SendPromptAsync(PersistentAgentsClient client, PersistentAgent agent, string prompt)
    {
        PersistentAgentThread thread = await client.Threads.CreateThreadAsync();
        await client.Messages.CreateMessageAsync(thread.Id, MessageRole.User, prompt);
        ThreadRun run = await client.Runs.CreateRunAsync(thread.Id, agent.Id);

        while (run.Status == RunStatus.Queued || run.Status == RunStatus.InProgress || run.Status == RunStatus.RequiresAction)
        {
            await Task.Delay(500);
            run = await client.Runs.GetRunAsync(thread.Id, run.Id);
        }

        await foreach (PersistentThreadMessage msg in client.Messages.GetMessagesAsync(thread.Id, order: ListSortOrder.Descending))
        {
            if (msg.Role == MessageRole.Agent)
            {
                foreach (MessageContent item in msg.ContentItems)
                {
                    if (item is MessageTextContent text)
                    {
                        return text.Text;
                    }
                }
            }
        }
        return string.Empty;
    }
}
