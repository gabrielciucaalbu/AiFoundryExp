namespace AiFoundryExp;

/// <summary>
/// Represents a recommendation provided by an agent.
/// </summary>
/// <param name="AgentName">Name of the agent providing the recommendation.</param>
/// <param name="Recommendation">The proposed action.</param>
/// <param name="Justification">Reasoning behind the recommendation.</param>
public record AgentRecommendation(string AgentName, string Recommendation, string Justification);
