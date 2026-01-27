using AutorLLM.Domain.Common;

namespace AutorLLM.Domain.Events;

public class CharacterDeletedEvent : DomainEvent
{
    public Guid CharacterId { get; }
    public Guid ProjectId { get; }

    public CharacterDeletedEvent(Guid characterId, Guid projectId)
    {
        CharacterId = characterId;
        ProjectId = projectId;
    }
}
