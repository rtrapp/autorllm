using MediatR;

namespace AutorLLM.Application.Commands.Projects.DeleteProject;

/// <summary>
/// Command for deleting a Project
/// </summary>
public record DeleteProjectCommand : IRequest<Unit>
{
    public Guid ProjectId { get; init; }
}
