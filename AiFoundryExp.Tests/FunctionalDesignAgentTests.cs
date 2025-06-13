using AiFoundryExp.Agents;

namespace AiFoundryExp.Tests;

public class FunctionalDesignAgentTests
{
    [Fact]
    public void CreateFunctionalDesign_PopulatesContext()
    {
        var agent = new FunctionalDesignAgent(new AgentDefinition { Name = "FD" }, new MessageBus());
        var context = new Dictionary<string, string>
        {
            ["key_features"] = "login, reports",
            ["main_user_workflow"] = "users login then create reports"
        };

        agent.CreateFunctionalDesign(context);

        Assert.Equal("users login then create reports", context["workflow_description"]);
        Assert.Contains("login", context["ui_description"]);
    }

    [Fact]
    public void ValidateDesign_LogsMissingRequirements()
    {
        var agent = new FunctionalDesignAgent(new AgentDefinition { Name = "FD" }, new MessageBus());
        var context = new Dictionary<string, string>
        {
            ["key_features"] = "login, analytics",
            ["workflow_description"] = "only login implemented",
            ["ui_description"] = "UI supporting login"
        };

        using StringWriter sw = new();
        TextWriter original = Console.Out;
        Console.SetOut(sw);
        try
        {
            agent.ValidateDesign(context);
        }
        finally
        {
            Console.SetOut(original);
        }

        string output = sw.ToString();
        Assert.Contains("analytics", output);
    }
}
