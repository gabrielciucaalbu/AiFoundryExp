using System.Text;

namespace AiFoundryExp.Agents;

/// <summary>
/// Generates simple business plan documents from the provided model.
/// </summary>
public class DocumentGenerationAgent : BaseAgent
{
    public DocumentGenerationAgent(AgentDefinition definition, IMessageBus bus) : base(definition, bus) { }

    /// <summary>
    /// Create a basic business plan file based on the provided model.
    /// </summary>
    public void GenerateDocuments(BusinessModel model, string outputDir)
    {
        ArgumentNullException.ThrowIfNull(model);

        Directory.CreateDirectory(outputDir);
        StringBuilder plan = new();
        plan.AppendLine($"Idea: {model.BusinessIdea}");
        plan.AppendLine($"Target Market: {model.TargetMarket}");
        plan.AppendLine($"Revenue Model: {model.RevenueModel}");

        File.WriteAllText(Path.Combine(outputDir, "BusinessPlan.txt"), plan.ToString());
        Console.WriteLine($"Documents generated in '{outputDir}'.");
    }

    public override string? GenerateNextQuestion(Dictionary<string, string> context) => null;

    public override void ProcessAnswer(string answer, Dictionary<string, string> context) { }
}
