using AutorLLM.Application.DTOs;
using AutorLLM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutorLLM.Application.Queries.Chapters.ListChapters;

/// <summary>
/// Handler for ListChaptersQuery
/// Returns ordered list of chapters
/// </summary>
public class ListChaptersQueryHandler 
    : IRequestHandler<ListChaptersQuery, IEnumerable<ChapterDto>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ILogger<ListChaptersQueryHandler> _logger;

    public ListChaptersQueryHandler(
        IProjectRepository projectRepository,
        ILogger<ListChaptersQueryHandler> logger)
    {
        _projectRepository = projectRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<ChapterDto>> Handle(
        ListChaptersQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting chapters for project {ProjectId}",
            query.ProjectId);

        // Load Project aggregate with all chapters
        var project = await _projectRepository.GetByIdAsync(
            query.ProjectId,
            cancellationToken);

        if (project == null)
        {
            _logger.LogWarning("Project {ProjectId} not found", query.ProjectId);
            throw new InvalidOperationException($"Project {query.ProjectId} not found.");
        }

        // Return chapters ordered by Order property
        var chapters = project.Chapters
            .OrderBy(c => c.Order.Value)
            .Select(c => new ChapterDto
            {
                Id = c.Id,
                ProjectId = c.ProjectId,
                Title = c.Title,
                Summary = c.Summary,
                Content = c.Content,
                Order = c.Order.Value,
                WordCount = c.WordCount,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .ToList();

        _logger.LogInformation(
            "Retrieved {Count} chapters for project {ProjectId}",
            chapters.Count,
            query.ProjectId);

        return chapters;
    }
}
