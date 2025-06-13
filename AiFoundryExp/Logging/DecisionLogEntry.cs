namespace AiFoundryExp;

/// <summary>
/// Details about a single decision.
/// </summary>
public class DecisionLogEntry
{
    public DateTime Timestamp { get; init; }

    public string Context { get; init; } = string.Empty;

    public string Decision { get; init; } = string.Empty;

    public string Rationale { get; init; } = string.Empty;
}
