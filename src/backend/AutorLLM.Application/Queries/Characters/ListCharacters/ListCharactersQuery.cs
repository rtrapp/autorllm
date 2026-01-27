using AutorLLM.Application.DTOs;
using MediatR;

namespace AutorLLM.Application.Queries.Characters.ListCharacters;

/// <summary>
/// Query for retrieving all Characters for a specific Project
/// </summary>
public record ListCharactersQuery : IRequest<IEnumerable<CharacterDto>>
{
    public Guid ProjectId { get; init; }
}
