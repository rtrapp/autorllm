using AutorLLM.Domain.Exceptions;
using AutorLLM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutorLLM.Application.Commands.Projects.UpdateProject;

/// <summary>
/// Handler for UpdateProjectCommand
/// </summary>
public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, Unit>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateProjectCommandHandler> _logger;

    public UpdateProjectCommandHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateProjectCommandHandler> logger)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Unit> Handle(
        UpdateProjectCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating project: {ProjectId}", command.ProjectId);

        // Retrieve aggregate
        var project = await _projectRepository.GetByIdAsync(command.ProjectId, cancellationToken);
        
        if (project == null)
            throw new ProjectNotFoundException(command.ProjectId);

        // Use domain methods (not property setters)
        if (command.Title != null)
            project.UpdateTitle(command.Title);

        if (command.Author != null)
            project.UpdateAuthor(command.Author);

        if (command.Synopsis != null)
            project.UpdateSynopsis(command.Synopsis);

        if (command.Genre != null)
            project.SetGenre(command.Genre);

        if (command.TargetWordCount.HasValue)
            project.SetTargetWordCount(command.TargetWordCount.Value);

        // Persist changes
        await _projectRepository.UpdateAsync(project, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Project updated: {ProjectId}", project.Id);

        return Unit.Value;
    }
}
