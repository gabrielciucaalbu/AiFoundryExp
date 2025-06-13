namespace AiFoundryExp.Agents;

/// <summary>
/// Translates business objectives into concrete technical requirements.
/// </summary>
using System.Text.Json;

public class RequirementsGatheringAgent : BaseAgent
{
    public RequirementsGatheringAgent(AgentDefinition definition, IMessageBus bus) : base(definition, bus) { }

    private readonly string[] _fields = ["key_features"]; 

    /// <summary>
    /// Explore user needs and constraints to define system requirements.
    /// </summary>
    public void CollectRequirements(Dictionary<string, string> context)
    {
        if (!context.TryGetValue("key_features", out string? features) || string.IsNullOrWhiteSpace(features))
        {
            return;
        }

        List<string> items = features
            .Split(new[] { '\n', ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(f => f.Trim())
            .Where(f => f.Length > 0)
            .ToList();

        List<string> requirements = items
            .Select(f => $"Support {f}")
            .ToList();

        context["technical_requirements"] = JsonSerializer.Serialize(requirements);

        foreach (string req in requirements)
        {
            Send("Technical Specification Agent", req);
            Send("Functional Design Agent", req);
        }
    }

    /// <summary>
    /// Maintain traceability between business goals and technical specifications.
    /// </summary>
    public void MaintainTraceability(Dictionary<string, string> context)
    {
        if (!context.TryGetValue("technical_requirements", out string? json))
        {
            return;
        }

        List<string>? requirements = JsonSerializer.Deserialize<List<string>>(json);
        if (requirements is null || requirements.Count == 0)
        {
            return;
        }

        context.TryGetValue("business_idea", out string? businessObjective);
        businessObjective ??= string.Empty;

        Dictionary<string, string> mapping = new();
        foreach (string req in requirements)
        {
            mapping[req] = businessObjective;
        }

        context["traceability"] = JsonSerializer.Serialize(mapping);
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
