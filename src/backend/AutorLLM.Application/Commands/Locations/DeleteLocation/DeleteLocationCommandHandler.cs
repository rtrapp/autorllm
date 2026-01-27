using AutorLLM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutorLLM.Application.Commands.Locations.DeleteLocation;

/// <summary>
/// Handler for DeleteLocationCommand
/// </summary>
public class DeleteLocationCommandHandler : IRequestHandler<DeleteLocationCommand, Unit>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteLocationCommandHandler> _logger;

    public DeleteLocationCommandHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteLocationCommandHandler> logger)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Unit> Handle(
        DeleteLocationCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Deleting location {LocationId} from project {ProjectId}",
            command.LocationId,
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

        // Remove location through aggregate root (encapsulated business logic)
        project.RemoveLocation(command.LocationId);

        // Persist through repository
        await _projectRepository.UpdateAsync(project, cancellationToken);

        // Commit transaction
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Location {LocationId} deleted successfully", command.LocationId);

        return Unit.Value;
    }
}
