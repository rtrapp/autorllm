using AutorLLM.Domain.Interfaces;
using AutorLLM.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutorLLM.Application.Commands.Characters.CreateCharacter;

/// <summary>
/// Handler for CreateCharacterCommand
/// </summary>
public class CreateCharacterCommandHandler 
    : IRequestHandler<CreateCharacterCommand, CreateCharacterResult>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateCharacterCommandHandler> _logger;

    public CreateCharacterCommandHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateCharacterCommandHandler> logger)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<CreateCharacterResult> Handle(
        CreateCharacterCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating character {Name} in project {ProjectId}",
            command.Name,
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

        // Parse role
        var role = CharacterRole.FromString(command.Role);

        // Add character through aggregate root (encapsulated business logic)
        var character = project.AddCharacter(
            command.Name,
            command.Biography,
            role);

        // Persist through repository
        await _projectRepository.UpdateAsync(project, cancellationToken);

        // Commit transaction
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation(
            "Character created with ID: {CharacterId}",
            character.Id);

        return new CreateCharacterResult
        {
            CharacterId = character.Id,
            Success = true
        };
    }
}
