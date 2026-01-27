using AutorLLM.Application.DTOs;
using AutorLLM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutorLLM.Application.Queries.Plots.ListPlots;

/// <summary>
/// Handler for ListPlotsQuery
/// </summary>
public class ListPlotsQueryHandler 
    : IRequestHandler<ListPlotsQuery, IEnumerable<PlotDto>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ILogger<ListPlotsQueryHandler> _logger;

    public ListPlotsQueryHandler(
        IProjectRepository projectRepository,
        ILogger<ListPlotsQueryHandler> logger)
    {
        _projectRepository = projectRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<PlotDto>> Handle(
        ListPlotsQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Retrieving all plots for project {ProjectId}",
            query.ProjectId);

        // Load Project aggregate
        var project = await _projectRepository.GetByIdAsync(
            query.ProjectId,
            cancellationToken);

        if (project == null)
        {
            _logger.LogWarning("Project {ProjectId} not found", query.ProjectId);
            throw new InvalidOperationException($"Project {query.ProjectId} not found.");
        }

        // Map plots to DTOs
        var plotDtos = project.Plots.Select(plot => new PlotDto
        {
            Id = plot.Id,
            ProjectId = plot.ProjectId,
            Title = plot.Title,
            Description = plot.Description,
            Type = plot.Type.ToString(),
            Resolution = plot.Resolution,
            IsActive = plot.IsActive,
            CreatedAt = plot.CreatedAt,
            UpdatedAt = plot.UpdatedAt
        }).ToList();

        _logger.LogInformation(
            "Retrieved {Count} plots for project {ProjectId}",
            plotDtos.Count,
            query.ProjectId);

        return plotDtos;
    }
}
