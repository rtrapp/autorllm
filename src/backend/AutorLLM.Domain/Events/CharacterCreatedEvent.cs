using AutorLLM.Domain.Common;

namespace AutorLLM.Domain.Events;

public class CharacterCreatedEvent : DomainEvent
{
    public Guid CharacterId { get; }
    public Guid ProjectId { get; }
    public string Name { get; }

    public CharacterCreatedEvent(Guid characterId, Guid projectId, string name)
    {
        CharacterId = characterId;
        ProjectId = projectId;
        Name = name;
    }
}
