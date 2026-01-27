using AutorLLM.Application.DTOs;
using MediatR;

namespace AutorLLM.Application.Queries.Projects.GetProject;

/// <summary>
/// Query for retrieving a single Project by ID
/// </summary>
public record GetProjectQuery : IRequest<ProjectDto>
{
    public Guid ProjectId { get; init; }
}
