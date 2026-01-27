using AutorLLM.Domain.Interfaces;
using AutorLLM.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutorLLM.Application.Commands.Characters.UpdateCharacter;

/// <summary>
/// Handler for UpdateCharacterCommand
/// </summary>
public class UpdateCharacterCommandHandler : IRequestHandler<UpdateCharacterCommand, Unit>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateCharacterCommandHandler> _logger;

    public UpdateCharacterCommandHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateCharacterCommandHandler> logger)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Unit> Handle(
        UpdateCharacterCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Updating character {CharacterId} in project {ProjectId}",
            command.CharacterId,
            command.ProjectId);

        // Load Project aggregate
        var project = await _projectRepository.GetByIdAsync(
            command.ProjectId,
            cancellationToken);

        if (project == null)
        {
            _logger.LogWarning("Project {ProjectId} not found", command.ProjectId);
            throw new InvalidOperationException($"Project {command.ProjectId} not found.");
        }

        // Get character from aggregate
        var character = project.GetCharacter(command.CharacterId);

        // Parse role
        var role = CharacterRole.FromString(command.Role);

        // Update character using domain methods
        character.UpdateDetails(command.Name, command.Description, role);

        if (command.Backstory != null)
            character.UpdateBackstory(command.Backstory);

        if (command.Appearance != null)
            character.UpdateAppearance(command.Appearance);

        if (command.Personality != null)
            character.UpdatePersonality(command.Personality);

        // Persist changes through repository
        await _projectRepository.UpdateAsync(project, cancellationToken);

        // Commit transaction
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation(
            "Character {CharacterId} updated successfully",
            command.CharacterId);

        return Unit.Value;
    }
}
