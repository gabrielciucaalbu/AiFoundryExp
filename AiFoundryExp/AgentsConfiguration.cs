using System.Text.Json.Serialization;

namespace AiFoundryExp;

public class AgentsConfiguration
{
    [JsonPropertyName("agents")]
    public List<AgentDefinition> Agents { get; set; } = new();
}
