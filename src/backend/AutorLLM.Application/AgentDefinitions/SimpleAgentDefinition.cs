namespace AutorLLM.Application.AgentDefinitions;

public sealed class SimpleAgentDefinition : BaseAgentDefinition
{
    public SimpleAgentDefinition() : base(nameof(SimpleAgentDefinition), "You are a kind and helpful agent")
    {

    }
}