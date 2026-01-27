using AutorLLM.Domain.Exceptions;
using AutorLLM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutorLLM.Application.Commands.Projects.DeleteProject;

/// <summary>
/// Handler for DeleteProjectCommand
/// </summary>
public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, Unit>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteProjectCommandHandler> _logger;

    public DeleteProjectCommandHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteProjectCommandHandler> logger)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Unit> Handle(
        DeleteProjectCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting project: {ProjectId}", command.ProjectId);

        // Verify project exists
        var exists = await _projectRepository.ExistsAsync(command.ProjectId, cancellationToken);
        
        if (!exists)
            throw new ProjectNotFoundException(command.ProjectId);

        // Business rule: Could check if project has chapters before deleting
        // For now, cascade delete is handled by database

        // Delete project
        await _projectRepository.DeleteAsync(command.ProjectId, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Project deleted: {ProjectId}", command.ProjectId);

        return Unit.Value;
    }
}
