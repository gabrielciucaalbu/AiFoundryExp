using AiFoundryExp.Agents;

namespace AiFoundryExp.Tests;

public class DocumentGenerationAgentTests
{
    [Fact]
    public void GenerateDocuments_WritesBusinessPlan()
    {
        string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDir);

        DocumentGenerationAgent agent = new(new AgentDefinition { Name = "DocGen" }, new MessageBus());
        BusinessModel model = new() { BusinessIdea = "Idea", TargetMarket = "QA", RevenueModel = "subscriptions" };

        agent.GenerateDocuments(model, tempDir);

        string file = Path.Combine(tempDir, "BusinessPlan.txt");
        Assert.True(File.Exists(file));
        string text = File.ReadAllText(file);
        Assert.Contains("Idea", text);
        Assert.Contains("QA", text);
        Assert.Contains("subscriptions", text);
    }
}
