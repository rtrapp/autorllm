using AutorLLM.Domain.Common;

namespace AutorLLM.Domain.Events;

public class ProjectCreatedEvent : DomainEvent
{
    public Guid ProjectId { get; }
    public string Title { get; }

    public ProjectCreatedEvent(Guid projectId, string title)
    {
        ProjectId = projectId;
        Title = title;
    }
}
