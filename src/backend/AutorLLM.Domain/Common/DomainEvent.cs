namespace AutorLLM.Domain.Common;

/// <summary>
/// Base class for all domain events.
/// Domain events represent something that happened in the domain.
/// </summary>
public abstract class DomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredAt { get; }

    protected DomainEvent()
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;
    }
}
