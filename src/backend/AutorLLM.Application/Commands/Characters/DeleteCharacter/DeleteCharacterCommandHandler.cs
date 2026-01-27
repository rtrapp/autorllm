using AutorLLM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutorLLM.Application.Commands.Characters.DeleteCharacter;

/// <summary>
/// Handler for DeleteCharacterCommand
/// </summary>
public class DeleteCharacterCommandHandler : IRequestHandler<DeleteCharacterCommand, Unit>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteCharacterCommandHandler> _logger;

    public DeleteCharacterCommandHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteCharacterCommandHandler> logger)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Unit> Handle(
        DeleteCharacterCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Deleting character {CharacterId} from project {ProjectId}",
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

        // Remove character through aggregate root (validates business rules)
        project.RemoveCharacter(command.CharacterId);

        // Persist changes through repository
        await _projectRepository.UpdateAsync(project, cancellationToken);

        // Commit transaction
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation(
            "Character {CharacterId} deleted successfully",
            command.CharacterId);

        return Unit.Value;
    }
}
