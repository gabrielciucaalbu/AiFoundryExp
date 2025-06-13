namespace AiFoundryExp.Agents;

/// <summary>
/// Produces detailed Software Requirements Specifications and ensures technical feasibility.
/// </summary>
public class TechnicalSpecificationAgent : BaseAgent
{
    public TechnicalSpecificationAgent(AgentDefinition definition, IMessageBus bus) : base(definition, bus) { }

    private readonly string[] _fields = ["technology_preferences"];

    /// <summary>
    /// Define system architecture and interfaces based on the provided
    /// project context. The resulting description is stored in the
    /// <c>architecture</c> field of the context dictionary.
    /// </summary>
    public void DefineArchitecture(Dictionary<string, string> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.TryGetValue("technology_preferences", out string? techPrefs);
        context.TryGetValue("key_features", out string? features);

        List<string> components = new()
        {
            "User Interface",
            "API Service",
            "Data Store"
        };

        if (features is not null &&
            features.Contains("analytics", StringComparison.OrdinalIgnoreCase))
        {
            components.Add("Analytics Module");
        }

        string architecture =
            $"Technology stack: {techPrefs ?? "standard stack"}\n" +
            "Components:" + string.Concat(components.Select(c => "\n- " + c)) +
            "\nInterfaces:\n- UI <-> API\n- API <-> Data Store";

        context["architecture"] = architecture;
    }

    /// <summary>
    /// Compile functional and non-functional requirements into a simple
    /// Software Requirements Specification (SRS) and store the text in the
    /// <c>srs</c> field of the context dictionary.
    /// </summary>
    public void DocumentRequirements(Dictionary<string, string> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        List<string> functional = new();
        if (context.TryGetValue("key_features", out string? features))
        {
            foreach (string feature in features.Split(',', '\n'))
            {
                string trimmed = feature.Trim();
                if (!string.IsNullOrEmpty(trimmed))
                {
                    functional.Add("- " + trimmed);
                }
            }
        }

        List<string> nonFunctional = new();
        if (context.TryGetValue("technology_preferences", out string? tech))
        {
            nonFunctional.Add("- Preferred technology stack: " + tech);
        }

        string srs = "Software Requirements Specification\n\n" +
                     "Functional Requirements:\n" +
                     (functional.Count > 0
                        ? string.Join('\n', functional)
                        : "- None specified") +
                     "\n\nNon-Functional Requirements:\n" +
                     (nonFunctional.Count > 0
                        ? string.Join('\n', nonFunctional)
                        : "- None specified");

        context["srs"] = srs;
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
            NextFieldIndex++;
        }
    }
}
