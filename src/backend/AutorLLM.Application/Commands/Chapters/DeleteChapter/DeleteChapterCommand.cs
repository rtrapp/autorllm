using MediatR;

namespace AutorLLM.Application.Commands.Chapters.DeleteChapter;

/// <summary>
/// Command for deleting a Chapter
/// Domain Service adjusts Order of subsequent chapters
/// </summary>
public record DeleteChapterCommand : IRequest<Unit>
{
    public Guid ProjectId { get; init; }
    public Guid ChapterId { get; init; }
}
