using System.Collections.Generic;

namespace AiFoundryExp.Agents;

/// <summary>
/// Handles market analysis, competitive positioning and high level business planning.
/// </summary>
public class BusinessStrategyAgent : BaseAgent
{
    public BusinessStrategyAgent(AgentDefinition definition, IMessageBus bus) : base(definition, bus) { }

    private readonly string[] _fields = ["business_idea", "target_market", "revenue_model"]; 

    /// <summary>
    /// Build a comprehensive business model from user input.
    /// </summary>
    public void BuildBusinessModel()
    {
        // Implementation would generate a model including value proposition, target markets and revenue plans.
    }

    /// <summary>
    /// Request additional clarification from the user through the User Interaction Agent.
    /// </summary>
    public void RequestClarification()
    {
        // Implementation would formulate questions for missing business details.
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
        return $"Please provide details about your {field}.";
    }

    public override void ProcessAnswer(string answer, Dictionary<string, string> context)
    {
        if (NextFieldIndex < _fields.Length)
        {
            context[_fields[NextFieldIndex]] = answer;
        }
    }
}
