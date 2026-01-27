using AutorLLM.Application.DTOs;
using AutorLLM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutorLLM.Application.Queries.PlotPoints.GetPlotPointsByChapter;

/// <summary>
/// Handler for GetPlotPointsByChapterQuery
/// Returns all PlotPoints for a specific Chapter
/// </summary>
public class GetPlotPointsByChapterQueryHandler 
    : IRequestHandler<GetPlotPointsByChapterQuery, IEnumerable<PlotPointDto>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ILogger<GetPlotPointsByChapterQueryHandler> _logger;

    public GetPlotPointsByChapterQueryHandler(
        IProjectRepository projectRepository,
        ILogger<GetPlotPointsByChapterQueryHandler> logger)
    {
        _projectRepository = projectRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<PlotPointDto>> Handle(
        GetPlotPointsByChapterQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Retrieving PlotPoints for Chapter {ChapterId} in Project {ProjectId}",
            query.ChapterId,
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

        // Verify Chapter exists
        var chapter = project.Chapters.FirstOrDefault(c => c.Id == query.ChapterId);
        if (chapter == null)
        {
            _logger.LogWarning(
                "Chapter {ChapterId} not found in Project {ProjectId}",
                query.ChapterId,
                query.ProjectId);
            return Enumerable.Empty<PlotPointDto>();
        }

        // Collect PlotPoints from all Plots that reference this Chapter
        var plotPoints = new List<PlotPointDto>();
        
        foreach (var plot in project.Plots)
        {
            var chapterPlotPoints = plot.PlotPoints
                .Where(pp => pp.ChapterId == query.ChapterId)
                .Select(pp => new PlotPointDto
                {
                    Id = pp.Id,
                    PlotId = pp.PlotId,
                    ChapterId = pp.ChapterId,
                    Description = pp.Description,
                    Intensity = pp.IntensityLevel,
                    Order = pp.Order,
                    CreatedAt = pp.CreatedAt,
                    UpdatedAt = pp.UpdatedAt
                });

            plotPoints.AddRange(chapterPlotPoints);
        }

        _logger.LogInformation(
            "Retrieved {Count} PlotPoints for Chapter {ChapterId}",
            plotPoints.Count,
            query.ChapterId);

        return plotPoints;
    }
}
