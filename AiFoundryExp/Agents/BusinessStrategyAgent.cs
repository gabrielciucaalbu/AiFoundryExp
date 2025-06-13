using AiFoundryExp;

namespace AiFoundryExp.Agents;

/// <summary>
/// Handles market analysis, competitive positioning and high level business planning.
/// </summary>
public class BusinessStrategyAgent : BaseAgent
{
    public BusinessStrategyAgent(AgentDefinition definition, IMessageBus bus) : base(definition, bus) { }

    private bool _inputLoaded;

    private readonly string[] _fields = ["business_idea", "target_market", "revenue_model"]; 

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

    /// <summary>
    /// Request additional clarification from the user through the User Interaction Agent.
    /// </summary>
    public void RequestClarification(Dictionary<string, string> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        foreach (string field in _fields)
        {
            if (!context.TryGetValue(field, out string? value) || string.IsNullOrWhiteSpace(value))
            {
                string readable = field.Replace('_', ' ');
                Send("User Interaction Agent", $"Could you clarify your {readable}?");
            }
        }
    }

    public override string? GenerateNextQuestion(Dictionary<string, string> context)
    {
        if (!_inputLoaded)
        {
            string path = Path.Combine(AppContext.BaseDirectory, "input", "input.txt");
            Dictionary<string, string> fileContext = InputParser.ParseFile(path);
            foreach (var kv in fileContext)
            {
                if (!context.ContainsKey(kv.Key))
                {
                    context[kv.Key] = kv.Value;
                }
            }
            _inputLoaded = true;
        }

        return base.GenerateNextQuestion(context);
    }

    public override void ProcessAnswer(string answer, Dictionary<string, string> context)
    {
        if (NextFieldIndex < _fields.Length)
        {
            context[_fields[NextFieldIndex]] = answer;
        }
    }
}
