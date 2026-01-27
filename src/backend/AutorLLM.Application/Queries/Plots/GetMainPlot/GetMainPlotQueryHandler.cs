using AutorLLM.Application.DTOs;
using AutorLLM.Domain.Interfaces;
using AutorLLM.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutorLLM.Application.Queries.Plots.GetMainPlot;

/// <summary>
/// Handler for GetMainPlotQuery
/// </summary>
public class GetMainPlotQueryHandler : IRequestHandler<GetMainPlotQuery, PlotDto?>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ILogger<GetMainPlotQueryHandler> _logger;

    public GetMainPlotQueryHandler(
        IProjectRepository projectRepository,
        ILogger<GetMainPlotQueryHandler> logger)
    {
        _projectRepository = projectRepository;
        _logger = logger;
    }

    public async Task<PlotDto?> Handle(
        GetMainPlotQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Retrieving main plot for project {ProjectId}",
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

        // Find main plot
        var mainPlot = project.Plots.FirstOrDefault(p => p.Type.Value == PlotType.Main.Value);

        if (mainPlot == null)
        {
            _logger.LogInformation(
                "No main plot found for project {ProjectId}",
                query.ProjectId);
            return null;
        }

        _logger.LogInformation(
            "Found main plot {PlotId} for project {ProjectId}",
            mainPlot.Id,
            query.ProjectId);

        return new PlotDto
        {
            Id = mainPlot.Id,
            ProjectId = mainPlot.ProjectId,
            Title = mainPlot.Title,
            Description = mainPlot.Description,
            Type = mainPlot.Type.ToString(),
            Resolution = mainPlot.Resolution,
            IsActive = mainPlot.IsActive,
            CreatedAt = mainPlot.CreatedAt,
            UpdatedAt = mainPlot.UpdatedAt
        };
    }
}
