using AutorLLM.Application.DTOs;
using AutorLLM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutorLLM.Application.Queries.Projects.ListProjects;

/// <summary>
/// Handler for ListProjectsQuery
/// </summary>
public class ListProjectsQueryHandler : IRequestHandler<ListProjectsQuery, IEnumerable<ProjectDto>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ILogger<ListProjectsQueryHandler> _logger;

    public ListProjectsQueryHandler(
        IProjectRepository projectRepository,
        ILogger<ListProjectsQueryHandler> logger)
    {
        _projectRepository = projectRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<ProjectDto>> Handle(
        ListProjectsQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching all projects");

        var projects = await _projectRepository.GetAllAsync(cancellationToken);

        // Map domain entities to DTOs
        var dtos = projects.Select(p => new ProjectDto
        {
            Id = p.Id,
            Title = p.Title,
            Author = p.Author,
            Synopsis = p.Synopsis,
            Genre = p.Genre,
            TargetWordCount = p.TargetWordCount,
            CurrentWordCount = p.CurrentWordCount,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        });

        return dtos;
    }
}
