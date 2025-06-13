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
    /// Results are stored in the provided context dictionary under
    /// <c>workflow_description</c> and <c>ui_description</c> keys.
    /// </summary>
    public void CreateFunctionalDesign(Dictionary<string, string> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.TryGetValue("main_user_workflow", out string? workflow);
        context.TryGetValue("key_features", out string? features);

        // Basic synthesis of a functional workflow and UI description. In a real
        // implementation this would involve complex reasoning over the collected
        // requirements.
        string workflowDesc = string.IsNullOrWhiteSpace(workflow)
            ? $"Workflow for {features}".Trim()
            : workflow;

        string uiDesc = string.IsNullOrWhiteSpace(features)
            ? "Generic user interface"
            : $"UI supporting {features}";

        context["workflow_description"] = workflowDesc;
        context["ui_description"] = uiDesc;
    }

    /// <summary>
    /// Validate that the design addresses all captured requirements from
    /// <see cref="RequirementsGatheringAgent"/>. Any missing requirements are
    /// written to the console so that they can be acted upon.
    /// </summary>
    public void ValidateDesign(Dictionary<string, string> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (!context.TryGetValue("key_features", out string? features))
        {
            Console.WriteLine("No requirements available for validation.");
            return;
        }

        context.TryGetValue("workflow_description", out string? workflow);
        context.TryGetValue("ui_description", out string? ui);

        string designText = string.Join(" ", workflow, ui);

        var missing = new List<string>();
        foreach (string feature in features.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            string trimmed = feature.Trim();
            if (trimmed.Length == 0)
                continue;

            if (!designText.Contains(trimmed, StringComparison.OrdinalIgnoreCase))
            {
                missing.Add(trimmed);
            }
        }

        if (missing.Count > 0)
        {
            Console.WriteLine($"Missing requirements in functional design: {string.Join(", ", missing)}");
        }
    }

    public override string? GenerateNextQuestion(Dictionary<string, string> context)
    {
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
