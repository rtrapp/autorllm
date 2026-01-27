using AutorLLM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutorLLM.Application.Commands.Chapters.ReorderChapters;

/// <summary>
/// Handler for ReorderChaptersCommand
/// Processes reordering in batch via Project aggregate
/// </summary>
public class ReorderChaptersCommandHandler 
    : IRequestHandler<ReorderChaptersCommand, Unit>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReorderChaptersCommandHandler> _logger;

    public ReorderChaptersCommandHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        ILogger<ReorderChaptersCommandHandler> logger)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Unit> Handle(
        ReorderChaptersCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Reordering {Count} chapters in project {ProjectId}",
            command.ChapterIds.Count,
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

        // Reorder through aggregate root (batch operation)
        project.ReorderChapters(command.ChapterIds);

        // Persist through repository
        await _projectRepository.UpdateAsync(project, cancellationToken);

        // Commit transaction
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Chapters reordered successfully in project {ProjectId}", command.ProjectId);

        return Unit.Value;
    }
}
