using AutorLLM.Application.DTOs;
using AutorLLM.Domain.Exceptions;
using AutorLLM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutorLLM.Application.Queries.Projects.GetProject;

/// <summary>
/// Handler for GetProjectQuery
/// </summary>
public class GetProjectQueryHandler : IRequestHandler<GetProjectQuery, ProjectDto>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ILogger<GetProjectQueryHandler> _logger;

    public GetProjectQueryHandler(
        IProjectRepository projectRepository,
        ILogger<GetProjectQueryHandler> logger)
    {
        _projectRepository = projectRepository;
        _logger = logger;
    }

    public async Task<ProjectDto> Handle(
        GetProjectQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching project: {ProjectId}", query.ProjectId);

        var project = await _projectRepository.GetByIdAsync(query.ProjectId, cancellationToken);
        
        if (project == null)
            throw new ProjectNotFoundException(query.ProjectId);

        // Map domain entity to DTO
        var dto = new ProjectDto
        {
            Id = project.Id,
            Title = project.Title,
            Author = project.Author,
            Synopsis = project.Synopsis,
            Genre = project.Genre,
            TargetWordCount = project.TargetWordCount,
            CurrentWordCount = project.CurrentWordCount,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt
        };

        return dto;
    }
}
