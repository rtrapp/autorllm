using AutorLLM.Domain.Common;

namespace AutorLLM.Domain.Events;

public class ChapterContentUpdatedEvent : DomainEvent
{
    public Guid ChapterId { get; }
    public Guid ProjectId { get; }
    public int WordCount { get; }

    public ChapterContentUpdatedEvent(Guid chapterId, Guid projectId, int wordCount)
    {
        ChapterId = chapterId;
        ProjectId = projectId;
        WordCount = wordCount;
    }
}
