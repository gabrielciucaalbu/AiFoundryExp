using AiFoundryExp.Agents;

namespace AiFoundryExp.Tests;

public class TechnicalSpecificationAgentTests
{
    private static TechnicalSpecificationAgent CreateAgent()
    {
        return new(new AgentDefinition { Name = "TechSpec" }, new MessageBus());
    }

    [Fact]
    public void DefineArchitecture_PopulatesArchitectureInContext()
    {
        var context = new Dictionary<string, string>
        {
            ["key_features"] = "analytics, reports",
            ["technology_preferences"] = ".NET"
        };
        TechnicalSpecificationAgent agent = CreateAgent();

        agent.DefineArchitecture(context);

        Assert.True(context.TryGetValue("architecture", out string? arch));
        Assert.Contains("Components", arch);
        Assert.Contains("API Service", arch);
    }

    [Fact]
    public void DocumentRequirements_PopulatesSrsInContext()
    {
        var context = new Dictionary<string, string>
        {
            ["key_features"] = "login, dashboard",
            ["technology_preferences"] = "python"
        };
        TechnicalSpecificationAgent agent = CreateAgent();

        agent.DocumentRequirements(context);

        Assert.True(context.TryGetValue("srs", out string? srs));
        Assert.Contains("Functional Requirements", srs);
        Assert.Contains("login", srs);
        Assert.Contains("Non-Functional Requirements", srs);
        Assert.Contains("python", srs);
    }
}
