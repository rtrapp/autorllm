using AutorLLM.Application.DTOs;
using MediatR;

namespace AutorLLM.Application.Queries.Characters.GetCharacter;

/// <summary>
/// Query for retrieving a single Character by ID
/// </summary>
public record GetCharacterQuery : IRequest<CharacterDto>
{
    public Guid CharacterId { get; init; }
}
