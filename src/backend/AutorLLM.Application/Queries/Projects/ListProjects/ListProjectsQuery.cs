using AutorLLM.Application.DTOs;
using MediatR;

namespace AutorLLM.Application.Queries.Projects.ListProjects;

/// <summary>
/// Query for retrieving all Projects
/// </summary>
public record ListProjectsQuery : IRequest<IEnumerable<ProjectDto>>
{
}
