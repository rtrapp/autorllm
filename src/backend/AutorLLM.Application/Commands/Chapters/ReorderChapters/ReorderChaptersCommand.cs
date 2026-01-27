using MediatR;

namespace AutorLLM.Application.Commands.Chapters.ReorderChapters;

/// <summary>
/// Command for reordering chapters in batch
/// </summary>
public record ReorderChaptersCommand : IRequest<Unit>
{
    public Guid ProjectId { get; init; }
    public List<Guid> ChapterIds { get; init; } = new();
}
