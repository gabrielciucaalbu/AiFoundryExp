using System.Collections.Generic;

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
    /// Generate the next question needed from the user. Return null if no further information is required.
    /// </summary>
    public virtual string? GenerateNextQuestion(Dictionary<string, string> context) => null;

    /// <summary>
    /// Process a user answer to the last generated question.
    /// </summary>
    public virtual void ProcessAnswer(string answer, Dictionary<string, string> context) { }
}
