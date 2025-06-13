using System.Text.Json;
using AiFoundryExp;

namespace AiFoundryExp.Agents;

/// <summary>
/// Handles all communication with users and adapts terminology to their expertise.
/// </summary>
public class UserInteractionAgent : BaseAgent
{
    public UserInteractionAgent(AgentDefinition definition, IMessageBus bus) : base(definition, bus) { }

    /// <summary>
    /// Present a question to the user and return their response.
    /// </summary>
    public string AskQuestion(string question)
    {
        Console.Write(question + " \u003e ");
        return Console.ReadLine() ?? string.Empty;
    }

    /// <summary>
    /// Interpret a user response and convert it into structured data for other agents.
    /// The parsed tokens are published to the orchestration agent via the message bus.
    /// </summary>
    public void ProcessResponse(string response)
    {
        Console.WriteLine($"Received: {response}");

        Dictionary<string, string> tokens = InputParser.ParseText(response);
        string json = JsonSerializer.Serialize(tokens);
        Send("Orchestration Agent", json);
    }
}
