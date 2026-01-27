using MediatR;

namespace AutorLLM.Application.Commands.Characters.CreateCharacter;

/// <summary>
/// Command for creating a new Character
/// </summary>
public record CreateCharacterCommand : IRequest<CreateCharacterResult>
{
    public Guid ProjectId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public string Biography { get; init; } = string.Empty;
}
