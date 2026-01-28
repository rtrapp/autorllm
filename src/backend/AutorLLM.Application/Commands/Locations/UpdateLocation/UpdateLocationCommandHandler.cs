using AutorLLM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutorLLM.Application.Commands.Locations.UpdateLocation;

/// <summary>
/// Handler for UpdateLocationCommand
/// </summary>
public class UpdateLocationCommandHandler : IRequestHandler<UpdateLocationCommand, Unit>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateLocationCommandHandler> _logger;

    public UpdateLocationCommandHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateLocationCommandHandler> logger)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Unit> Handle(
        UpdateLocationCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Updating location {LocationId} in project {ProjectId}",
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

        // Get location through aggregate root
        var location = project.GetLocation(command.LocationId);

        // Update through domain methods (encapsulated business logic)
        location.UpdateDetails(command.Name, command.Description);
        location.UpdateGeography(command.Geography);
        location.UpdateCulture(command.Culture);
        location.UpdateSignificance(command.Significance);

        // Persist through repository
        await _projectRepository.UpdateAsync(project, cancellationToken);

        // Commit transaction
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Location {LocationId} updated successfully", command.LocationId);

        return Unit.Value;
    }
}
