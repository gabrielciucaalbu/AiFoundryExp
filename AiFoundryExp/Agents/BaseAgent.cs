namespace AiFoundryExp.Agents;

public abstract class BaseAgent
{
    public string Name { get; }
    public string Instructions { get; protected set; }

    protected BaseAgent(AgentDefinition definition)
    {
        Name = definition.Name;
        Instructions = definition.Instructions;
    }
}
