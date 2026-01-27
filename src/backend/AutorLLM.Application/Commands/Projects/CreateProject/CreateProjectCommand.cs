using MediatR;

namespace AutorLLM.Application.Commands.Projects.CreateProject;

/// <summary>
/// Command for creating a new Project
/// </summary>
public record CreateProjectCommand : IRequest<CreateProjectResult>
{
    public string Title { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;
    public string Synopsis { get; init; } = string.Empty;
}
