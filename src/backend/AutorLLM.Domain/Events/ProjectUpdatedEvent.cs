using AutorLLM.Domain.Common;

namespace AutorLLM.Domain.Events;

public class ProjectUpdatedEvent : DomainEvent
{
    public Guid ProjectId { get; }

    public ProjectUpdatedEvent(Guid projectId)
    {
        ProjectId = projectId;
    }
}
