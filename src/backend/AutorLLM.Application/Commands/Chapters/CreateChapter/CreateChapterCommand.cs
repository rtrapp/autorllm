using MediatR;

namespace AutorLLM.Application.Commands.Chapters.CreateChapter;

/// <summary>
/// Command for creating a new Chapter
/// </summary>
public record CreateChapterCommand : IRequest<CreateChapterResult>
{
    public Guid ProjectId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Summary { get; init; }
}
