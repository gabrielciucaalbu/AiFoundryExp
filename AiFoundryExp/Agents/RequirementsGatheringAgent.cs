namespace AiFoundryExp.Agents;

/// <summary>
/// Translates business objectives into concrete technical requirements.
/// </summary>
public class RequirementsGatheringAgent : BaseAgent
{
    public RequirementsGatheringAgent(AgentDefinition definition, IMessageBus bus) : base(definition, bus) { }

    private readonly string[] _fields = ["key_features"]; 

    /// <summary>
    /// Explore user needs and constraints to define system requirements.
    /// </summary>
    public void CollectRequirements()
    {
        // Implementation would question users and map answers to technical needs.
    }

    /// <summary>
    /// Maintain traceability between business goals and technical specifications.
    /// </summary>
    public void MaintainTraceability()
    {
        // Implementation would link requirements back to the originating business objectives.
    }

    public override string? GenerateNextQuestion(Dictionary<string, string> context)
    {
        while (NextFieldIndex < _fields.Length && context.ContainsKey(_fields[NextFieldIndex]))
        {
            NextFieldIndex++;
        }

        if (NextFieldIndex >= _fields.Length)
        {
            return null;
        }

        string field = _fields[NextFieldIndex].Replace('_', ' ');
        return $"Please provide details about {field}.";
    }

    public override void ProcessAnswer(string answer, Dictionary<string, string> context)
    {
        if (NextFieldIndex < _fields.Length)
        {
            context[_fields[NextFieldIndex]] = answer;
        }
    }
}
