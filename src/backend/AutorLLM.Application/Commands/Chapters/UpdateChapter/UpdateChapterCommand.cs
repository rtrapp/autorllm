using MediatR;

namespace AutorLLM.Application.Commands.Chapters.UpdateChapter;

/// <summary>
/// Command for updating a Chapter
/// </summary>
public record UpdateChapterCommand : IRequest<Unit>
{
    public Guid ProjectId { get; init; }
    public Guid ChapterId { get; init; }
    public string? Title { get; init; }
    public string? Summary { get; init; }
    public string? Content { get; init; }
}
