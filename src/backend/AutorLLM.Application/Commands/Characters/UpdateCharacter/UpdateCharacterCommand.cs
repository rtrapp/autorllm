using MediatR;

namespace AutorLLM.Application.Commands.Characters.UpdateCharacter;

/// <summary>
/// Command for updating an existing Character
/// </summary>
public record UpdateCharacterCommand : IRequest<Unit>
{
    public Guid ProjectId { get; init; }
    public Guid CharacterId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public string? Backstory { get; init; }
    public string? Appearance { get; init; }
    public string? Personality { get; init; }
}
