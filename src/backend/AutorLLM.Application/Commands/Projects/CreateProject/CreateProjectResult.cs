namespace AutorLLM.Application.Commands.Projects.CreateProject;

/// <summary>
/// Result returned after successfully creating a Project
/// </summary>
public record CreateProjectResult
{
    public Guid ProjectId { get; init; }
    public bool Success { get; init; }
}
