using AutorLLM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutorLLM.Application.Commands.Locations.CreateLocation;

/// <summary>
/// Handler for CreateLocationCommand
/// </summary>
public class CreateLocationCommandHandler 
    : IRequestHandler<CreateLocationCommand, CreateLocationResult>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateLocationCommandHandler> _logger;

    public CreateLocationCommandHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateLocationCommandHandler> logger)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<CreateLocationResult> Handle(
        CreateLocationCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating location {Name} in project {ProjectId}",
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

        // Add location through aggregate root (encapsulated business logic)
        var location = project.AddLocation(
            command.Name,
            command.Description);

        // Persist through repository
        await _projectRepository.UpdateAsync(project, cancellationToken);

        // Commit transaction
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation(
            "Location created with ID: {LocationId}",
            location.Id);

        return new CreateLocationResult
        {
            LocationId = location.Id,
            Success = true
        };
    }
}
