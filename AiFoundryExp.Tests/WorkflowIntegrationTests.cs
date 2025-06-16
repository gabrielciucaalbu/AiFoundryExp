namespace AiFoundryExp.Tests;

public class WorkflowIntegrationTests
{
    [Fact]
    public async Task Engine_ProgressesThroughAllPhases()
    {
        string configPath = Path.GetFullPath(Path.Combine("..", "..", "..", "..", "AiFoundryExp", "Configuration", "agents.json"));
        string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDir);
        string statePath = Path.Combine(tempDir, "state.json");

        OrchestrationEngine engine = await OrchestrationEngine.LoadAsync(configPath, statePath);
        var phases = new List<WorkflowPhase>();
        while (true)
        {
            phases.Add(engine.CurrentPhase);
            if (!engine.MoveNextPhase())
                break;
        }

        var expected = new[]
        {
            WorkflowPhase.BusinessConcept,
            WorkflowPhase.DocumentGeneration
        };

        Assert.Equal(expected, phases);
    }
}
