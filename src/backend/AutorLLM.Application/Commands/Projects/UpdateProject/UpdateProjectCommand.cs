using MediatR;

namespace AutorLLM.Application.Commands.Projects.UpdateProject;

/// <summary>
/// Command for updating an existing Project
/// </summary>
public record UpdateProjectCommand : IRequest<Unit>
{
    public Guid ProjectId { get; init; }
    public string? Title { get; init; }
    public string? Author { get; init; }
    public string? Synopsis { get; init; }
    public string? Genre { get; init; }
    public int? TargetWordCount { get; init; }
}
