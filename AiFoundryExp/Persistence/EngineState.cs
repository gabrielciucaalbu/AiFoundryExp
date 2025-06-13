using System.Text.Json.Serialization;

namespace AiFoundryExp;

public class EngineState
{
    [JsonPropertyName("currentPhase")]
    public WorkflowPhase CurrentPhase { get; set; } = WorkflowPhase.BusinessConceptDevelopment;
}
