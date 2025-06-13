using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AiFoundryExp;

public class AgentsConfiguration
{
    [JsonPropertyName("agents")]
    public List<AgentDefinition> Agents { get; set; } = new();
}

public class AgentDefinition
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("instructions")]
    public string Instructions { get; set; } = string.Empty;

    [JsonPropertyName("temperature")]
    public float? Temperature { get; set; }

    [JsonPropertyName("top_p")]
    public float? TopP { get; set; }
}
