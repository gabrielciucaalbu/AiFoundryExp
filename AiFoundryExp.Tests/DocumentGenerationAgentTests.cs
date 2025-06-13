using AiFoundryExp.Agents;

namespace AiFoundryExp.Tests;

public class DocumentGenerationAgentTests
{
    [Fact]
    public void GenerateDocuments_CanReadWhileLogIsOpenForWrite()
    {
        string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDir);
        string logPath = Path.Combine(tempDir, "conversation.log");

        using FileStream logStream = new(logPath, FileMode.Create, FileAccess.Write, FileShare.Read);
        using StreamWriter log = new(logStream);
        log.WriteLine("Business Strategy Agent: Test plan");
        log.Flush();

        DocumentGenerationAgent agent = new(new AgentDefinition { Name = "DocGen" }, new MessageBus());

        Exception? ex = Record.Exception(() => agent.GenerateDocuments(logPath, tempDir));

        Assert.Null(ex);
    }
}
