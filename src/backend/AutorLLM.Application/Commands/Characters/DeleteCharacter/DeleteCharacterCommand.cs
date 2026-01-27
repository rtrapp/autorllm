using MediatR;

namespace AutorLLM.Application.Commands.Characters.DeleteCharacter;

/// <summary>
/// Command for deleting a Character
/// </summary>
public record DeleteCharacterCommand : IRequest<Unit>
{
    public Guid ProjectId { get; init; }
    public Guid CharacterId { get; init; }
}
