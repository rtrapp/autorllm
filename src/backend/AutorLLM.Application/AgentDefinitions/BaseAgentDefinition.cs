namespace AutorLLM.Application.AgentDefinitions;

public abstract class BaseAgentDefinition
{
    public string Name { get; private set; } = "";

    public string Instructions { get; private set; } = "";

    protected BaseAgentDefinition(string name, string instructions)
    {
        this.Name = name;
        this.Instructions = instructions;
    }
}