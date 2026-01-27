namespace AutorLLM.Application.Commands.Chapters.CreateChapter;

/// <summary>
/// Result of creating a Chapter
/// </summary>
public record CreateChapterResult
{
    public Guid ChapterId { get; init; }
    public int Order { get; init; }
    public bool Success { get; init; }
}
