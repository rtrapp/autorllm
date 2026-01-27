using AutorLLM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutorLLM.Application.Commands.Chapters.DeleteChapter;

/// <summary>
/// Handler for DeleteChapterCommand
/// Deletes chapter and adjusts Order of subsequent chapters via Domain Service
/// </summary>
public class DeleteChapterCommandHandler 
    : IRequestHandler<DeleteChapterCommand, Unit>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteChapterCommandHandler> _logger;

    public DeleteChapterCommandHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteChapterCommandHandler> logger)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Unit> Handle(
        DeleteChapterCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Deleting chapter {ChapterId} from project {ProjectId}",
            command.ChapterId,
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

        // Remove chapter through aggregate root (handles Order adjustment)
        project.RemoveChapter(command.ChapterId);

        // Persist through repository
        await _projectRepository.UpdateAsync(project, cancellationToken);

        // Commit transaction
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Chapter {ChapterId} deleted successfully", command.ChapterId);

        return Unit.Value;
    }
}
