namespace AiFoundryExp.Agents;

/// <summary>
/// Handles all communication with users and adapts terminology to their expertise.
/// </summary>
public class UserInteractionAgent : BaseAgent
{
    public UserInteractionAgent(AgentDefinition definition, IMessageBus bus) : base(definition, bus) { }

    /// <summary>
    /// Present a question to the user.
    /// </summary>
    public void AskQuestion(string question)
    {
        // Implementation would tailor language and output to the user's skill level.
    }

    /// <summary>
    /// Interpret a user response and convert it into structured data for other agents.
    /// </summary>
    public void ProcessResponse(string response)
    {
        // Implementation would parse the response and pass it to the orchestrator.
    }
}
