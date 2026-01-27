using AutorLLM.Domain.Common;

namespace AutorLLM.Domain.Events;

public class ChapterCreatedEvent : DomainEvent
{
    public Guid ChapterId { get; }
    public Guid ProjectId { get; }
    public string Title { get; }

    public ChapterCreatedEvent(Guid chapterId, Guid projectId, string title)
    {
        ChapterId = chapterId;
        ProjectId = projectId;
        Title = title;
    }
}
