using System.Text.Json;

namespace AiFoundryExp.Agents;

public abstract class BaseAgent
{
    public string Name { get; }
    public string Instructions { get; protected set; }
    protected IMessageBus Bus { get; }

    /// <summary>
    /// Index of the next information field this agent expects.
    /// Used by simple question generation heuristics.
    /// </summary>
    protected int NextFieldIndex { get; set; }

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

    /// <summary>
    /// Generate the next question needed from the user by prompting the remote
    /// agent. Returns <c>null</c> if the agent indicates no further questions.
    /// </summary>
    public virtual string? GenerateNextQuestion(Dictionary<string, string> context)
    {
        string json = JsonSerializer.Serialize(context);
        string prompt = $"Given the following context as JSON:\n{json}\n" +
                        "Provide the next question you would ask the user. " +
                        "If you have no question, reply with 'none'.";

        string response = Bus.Prompt(Name, prompt).Trim();
        return string.IsNullOrEmpty(response) ||
               response.Equals("none", StringComparison.OrdinalIgnoreCase)
            ? null
            : response;
    }

    /// <summary>
    /// Process a user answer to the last generated question.
    /// </summary>
    public virtual void ProcessAnswer(string answer, Dictionary<string, string> context) { }
}
