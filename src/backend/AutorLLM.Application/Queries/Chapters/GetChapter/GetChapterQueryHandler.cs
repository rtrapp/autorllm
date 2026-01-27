using AutorLLM.Application.DTOs;
using AutorLLM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutorLLM.Application.Queries.Chapters.GetChapter;

/// <summary>
/// Handler for GetChapterQuery
/// Returns Chapter aggregate with Content value object
/// </summary>
public class GetChapterQueryHandler 
    : IRequestHandler<GetChapterQuery, ChapterDto>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ILogger<GetChapterQueryHandler> _logger;

    public GetChapterQueryHandler(
        IProjectRepository projectRepository,
        ILogger<GetChapterQueryHandler> logger)
    {
        _projectRepository = projectRepository;
        _logger = logger;
    }

    public async Task<ChapterDto> Handle(
        GetChapterQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting chapter {ChapterId}", query.ChapterId);

        // We need to get the chapter through repository
        // Since chapter is a child entity, we query directly
        var chapter = await _projectRepository.GetChapterByIdAsync(
            query.ChapterId,
            cancellationToken);

        if (chapter == null)
        {
            _logger.LogWarning("Chapter {ChapterId} not found", query.ChapterId);
            throw new InvalidOperationException($"Chapter {query.ChapterId} not found.");
        }

        _logger.LogInformation(
            "Chapter {ChapterId} retrieved successfully",
            query.ChapterId);

        return new ChapterDto
        {
            Id = chapter.Id,
            ProjectId = chapter.ProjectId,
            Title = chapter.Title,
            Summary = chapter.Summary,
            Content = chapter.Content,
            Order = chapter.Order.Value,
            WordCount = chapter.WordCount,
            CreatedAt = chapter.CreatedAt,
            UpdatedAt = chapter.UpdatedAt
        };
    }
}
