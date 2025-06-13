namespace AiFoundryExp.Agents;

/// <summary>
/// Produces detailed Software Requirements Specifications and ensures technical feasibility.
/// </summary>
public class TechnicalSpecificationAgent : BaseAgent
{
    public TechnicalSpecificationAgent(AgentDefinition definition, IMessageBus bus) : base(definition, bus) { }

    private readonly string[] _fields = ["technology_preferences"];

    /// <summary>
    /// Define system architecture and interfaces.
    /// </summary>
    public void DefineArchitecture()
    {
        // Implementation would describe components, interfaces and integrations.
    }

    /// <summary>
    /// Document functional and non-functional requirements in industry-standard format.
    /// </summary>
    public void DocumentRequirements()
    {
        // Implementation would create the SRS including constraints and quality attributes.
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
