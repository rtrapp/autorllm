using AutorLLM.Application.DTOs;
using AutorLLM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutorLLM.Application.Queries.Characters.ListCharacters;

/// <summary>
/// Handler for ListCharactersQuery
/// </summary>
public class ListCharactersQueryHandler 
    : IRequestHandler<ListCharactersQuery, IEnumerable<CharacterDto>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ILogger<ListCharactersQueryHandler> _logger;

    public ListCharactersQueryHandler(
        IProjectRepository projectRepository,
        ILogger<ListCharactersQueryHandler> logger)
    {
        _projectRepository = projectRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<CharacterDto>> Handle(
        ListCharactersQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Retrieving all characters for project {ProjectId}",
            query.ProjectId);

        // Load Project aggregate
        var project = await _projectRepository.GetByIdAsync(
            query.ProjectId,
            cancellationToken);

        if (project == null)
        {
            _logger.LogWarning("Project {ProjectId} not found", query.ProjectId);
            throw new InvalidOperationException($"Project {query.ProjectId} not found.");
        }

        // Map characters to DTOs
        var characterDtos = project.Characters.Select(character => new CharacterDto
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
        }).ToList();

        _logger.LogInformation(
            "Retrieved {Count} characters for project {ProjectId}",
            characterDtos.Count,
            query.ProjectId);

        return characterDtos;
    }
}
