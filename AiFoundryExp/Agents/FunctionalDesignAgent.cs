using System.Collections.Generic;

namespace AiFoundryExp.Agents;

/// <summary>
/// Converts requirements into implementable functional specifications and workflows.
/// </summary>
public class FunctionalDesignAgent : BaseAgent
{
    public FunctionalDesignAgent(AgentDefinition definition, IMessageBus bus) : base(definition, bus) { }

    private readonly string[] _fields = ["main_user_workflow"]; 

    /// <summary>
    /// Create detailed descriptions of system behavior and user interaction.
    /// </summary>
    public void CreateFunctionalDesign()
    {
        // Implementation would outline workflows, UI specifications and business logic.
    }

    /// <summary>
    /// Validate that the design addresses all captured requirements.
    /// </summary>
    public void ValidateDesign()
    {
        // Implementation would cross-check the design against requirements.
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
