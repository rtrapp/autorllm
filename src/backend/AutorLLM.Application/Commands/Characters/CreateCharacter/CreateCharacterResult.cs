namespace AutorLLM.Application.Commands.Characters.CreateCharacter;

/// <summary>
/// Result returned after successfully creating a Character
/// </summary>
public record CreateCharacterResult
{
    public Guid CharacterId { get; init; }
    public bool Success { get; init; }
}
