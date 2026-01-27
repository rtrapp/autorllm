using AutorLLM.Application.DTOs;
using AutorLLM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutorLLM.Application.Queries.Characters.GetCharacter;

/// <summary>
/// Handler for GetCharacterQuery
/// </summary>
public class GetCharacterQueryHandler : IRequestHandler<GetCharacterQuery, CharacterDto>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ILogger<GetCharacterQueryHandler> _logger;

    public GetCharacterQueryHandler(
        IProjectRepository projectRepository,
        ILogger<GetCharacterQueryHandler> logger)
    {
        _projectRepository = projectRepository;
        _logger = logger;
    }

    public async Task<CharacterDto> Handle(
        GetCharacterQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Retrieving character {CharacterId}",
            query.CharacterId);

        // We need to find the character by searching through projects
        // In a real scenario, we might need ProjectId or a dedicated Character query in repository
        var projects = await _projectRepository.GetAllAsync(cancellationToken);
        
        foreach (var project in projects)
        {
            var character = project.Characters.FirstOrDefault(c => c.Id == query.CharacterId);
            if (character != null)
            {
                _logger.LogInformation(
                    "Character {CharacterId} found in project {ProjectId}",
                    query.CharacterId,
                    project.Id);

                return new CharacterDto
                {
                    Id = character.Id,
                    ProjectId = character.ProjectId,
                    Name = character.Name,
                    Description = character.Description,
                    Role = character.Role.ToString(),
                    Backstory = character.Backstory,
                    Appearance = character.Appearance,
                    Personality = character.Personality,
                    CreatedAt = character.CreatedAt,
                    UpdatedAt = character.UpdatedAt
                };
            }
        }

        _logger.LogWarning("Character {CharacterId} not found", query.CharacterId);
        throw new InvalidOperationException($"Character {query.CharacterId} not found.");
    }
}
