using AutorLLM.Application.DTOs;
using AutorLLM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutorLLM.Application.Queries.Plots.GetPlot;

/// <summary>
/// Handler for GetPlotQuery
/// </summary>
public class GetPlotQueryHandler : IRequestHandler<GetPlotQuery, PlotDto>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ILogger<GetPlotQueryHandler> _logger;

    public GetPlotQueryHandler(
        IProjectRepository projectRepository,
        ILogger<GetPlotQueryHandler> logger)
    {
        _projectRepository = projectRepository;
        _logger = logger;
    }

    public async Task<PlotDto> Handle(
        GetPlotQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Retrieving plot {PlotId}",
            query.PlotId);

        // We need to load all projects to find which one contains this plot
        // In a real scenario, we might add a method GetProjectByPlotId to the repository
        // For now, we'll iterate through projects (not optimal but follows aggregate boundaries)
        var projects = await _projectRepository.GetAllAsync(cancellationToken);

        foreach (var project in projects)
        {
            var plot = project.Plots.FirstOrDefault(p => p.Id == query.PlotId);
            if (plot != null)
            {
                _logger.LogInformation(
                    "Found plot {PlotId} in project {ProjectId}",
                    query.PlotId,
                    project.Id);

                return new PlotDto
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
                };
            }
        }

        _logger.LogWarning("Plot {PlotId} not found", query.PlotId);
        throw new InvalidOperationException($"Plot {query.PlotId} not found.");
    }
}
