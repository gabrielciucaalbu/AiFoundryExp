using System.Text;

namespace AiFoundryExp.Agents;

/// <summary>
/// Compiles professional documents from the outputs of all other agents.
/// </summary>
public class DocumentGenerationAgent : BaseAgent
{
    public DocumentGenerationAgent(AgentDefinition definition, IMessageBus bus) : base(definition, bus) { }

    private bool _confirmed;

    /// <summary>
    /// Generate a document draft incorporating feedback and ensuring consistency.
    /// </summary>
    public void GenerateDocuments(string logPath, string outputDir)
    {
        Directory.CreateDirectory(outputDir);

        if (!File.Exists(logPath))
        {
            Console.WriteLine($"Conversation log not found: {logPath}");
            return;
        }

        StringBuilder businessPlan = new();
        StringBuilder srs = new();
        StringBuilder functionalSpec = new();

        foreach (string line in File.ReadLines(logPath))
        {
            if (line.StartsWith("Business Strategy Agent:"))
            {
                businessPlan.AppendLine(line.Substring("Business Strategy Agent:".Length).Trim());
            }
            else if (line.StartsWith("Requirements Gathering Agent:") ||
                     line.StartsWith("Technical Specification Agent:"))
            {
                srs.AppendLine(line.Substring(line.IndexOf(':') + 1).Trim());
            }
            else if (line.StartsWith("Functional Design Agent:"))
            {
                functionalSpec.AppendLine(line.Substring("Functional Design Agent:".Length).Trim());
            }
        }

        File.WriteAllText(Path.Combine(outputDir, "BusinessPlan.txt"), businessPlan.ToString());
        File.WriteAllText(Path.Combine(outputDir, "SRS.txt"), srs.ToString());
        File.WriteAllText(Path.Combine(outputDir, "FunctionalSpec.txt"), functionalSpec.ToString());

        Console.WriteLine($"Documents generated in '{outputDir}'.");
    }

    public override string? GenerateNextQuestion(Dictionary<string, string> context)
    {
        if (_confirmed)
        {
            return null;
        }

        return "Generate the final documents now? (yes/no)";
    }

    public override void ProcessAnswer(string answer, Dictionary<string, string> context)
    {
        if (answer.Trim().Equals("yes", StringComparison.OrdinalIgnoreCase))
        {
            _confirmed = true;
        }
    }
}
