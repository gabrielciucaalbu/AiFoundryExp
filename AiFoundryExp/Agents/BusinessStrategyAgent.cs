using AiFoundryExp;

namespace AiFoundryExp.Agents;

/// <summary>
/// Handles market analysis, competitive positioning and high level business planning.
/// </summary>
public class BusinessStrategyAgent : BaseAgent
{
    public BusinessStrategyAgent(AgentDefinition definition, IMessageBus bus) : base(definition, bus) { }

    /// <summary>
    /// Build a comprehensive business model from user input.
    /// </summary>
    public BusinessModel BuildBusinessModel(Dictionary<string, string> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.TryGetValue("business_idea", out string? idea);
        context.TryGetValue("target_market", out string? market);
        context.TryGetValue("revenue_model", out string? revenue);

        return new BusinessModel
        {
            BusinessIdea = idea ?? string.Empty,
            TargetMarket = market ?? string.Empty,
            RevenueModel = revenue ?? string.Empty
        };
    }

    public override string? GenerateNextQuestion(Dictionary<string, string> context) => null;

    public override void ProcessAnswer(string answer, Dictionary<string, string> context) { }
}
