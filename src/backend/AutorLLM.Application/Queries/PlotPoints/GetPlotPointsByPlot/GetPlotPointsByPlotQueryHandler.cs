using AutorLLM.Application.DTOs;
using AutorLLM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutorLLM.Application.Queries.PlotPoints.GetPlotPointsByPlot;

/// <summary>
/// Handler for GetPlotPointsByPlotQuery
/// Returns PlotPoints ordered by Chapter.Order
/// </summary>
public class GetPlotPointsByPlotQueryHandler 
    : IRequestHandler<GetPlotPointsByPlotQuery, IEnumerable<PlotPointDto>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ILogger<GetPlotPointsByPlotQueryHandler> _logger;

    public GetPlotPointsByPlotQueryHandler(
        IProjectRepository projectRepository,
        ILogger<GetPlotPointsByPlotQueryHandler> logger)
    {
        _projectRepository = projectRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<PlotPointDto>> Handle(
        GetPlotPointsByPlotQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Retrieving PlotPoints for Plot {PlotId} in Project {ProjectId}",
            query.PlotId,
            query.ProjectId);

        // Load Project aggregate
        var project = await _projectRepository.GetByIdAsync(
            query.ProjectId,
            cancellationToken);

        if (project == null)
        {
            _logger.LogWarning("Project {ProjectId} not found", query.ProjectId);
            return Enumerable.Empty<PlotPointDto>();
        }

        // Get Plot from aggregate
        var plot = project.Plots.FirstOrDefault(p => p.Id == query.PlotId);
        if (plot == null)
        {
            _logger.LogWarning(
                "Plot {PlotId} not found in Project {ProjectId}",
                query.PlotId,
                query.ProjectId);
            return Enumerable.Empty<PlotPointDto>();
        }

        // Order PlotPoints by Chapter.Order
        // We need to join with Chapters to get the Order
        var orderedPlotPoints = plot.PlotPoints
            .Select(pp =>
            {
                var chapter = project.Chapters.FirstOrDefault(c => c.Id == pp.ChapterId);
                return new
                {
                    PlotPoint = pp,
                    ChapterOrder = chapter?.Order ?? int.MaxValue
                };
            })
            .OrderBy(x => x.ChapterOrder)
            .Select(x => new PlotPointDto
            {
                Id = x.PlotPoint.Id,
                PlotId = x.PlotPoint.PlotId,
                ChapterId = x.PlotPoint.ChapterId,
                Description = x.PlotPoint.Description,
                Intensity = x.PlotPoint.IntensityLevel,
                Order = x.PlotPoint.Order,
                CreatedAt = x.PlotPoint.CreatedAt,
                UpdatedAt = x.PlotPoint.UpdatedAt
            })
            .ToList();

        _logger.LogInformation(
            "Retrieved {Count} PlotPoints for Plot {PlotId}",
            orderedPlotPoints.Count,
            query.PlotId);

        return orderedPlotPoints;
    }
}
