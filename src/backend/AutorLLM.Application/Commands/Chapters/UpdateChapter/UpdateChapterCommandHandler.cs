using AutorLLM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutorLLM.Application.Commands.Chapters.UpdateChapter;

/// <summary>
/// Handler for UpdateChapterCommand
/// Updates chapter via domain methods (UpdateTitle, UpdateSummary, UpdateContent)
/// WordCount is calculated automatically via domain entity
/// </summary>
public class UpdateChapterCommandHandler 
    : IRequestHandler<UpdateChapterCommand, Unit>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateChapterCommandHandler> _logger;

    public UpdateChapterCommandHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateChapterCommandHandler> logger)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Unit> Handle(
        UpdateChapterCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Updating chapter {ChapterId} in project {ProjectId}",
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

        // Get chapter through aggregate root
        var chapter = project.GetChapter(command.ChapterId);

        // Update via domain methods (encapsulated behavior)
        if (!string.IsNullOrWhiteSpace(command.Title))
        {
            chapter.UpdateTitle(command.Title);
        }

        if (command.Summary != null)
        {
            chapter.UpdateSummary(command.Summary);
        }

        if (command.Content != null)
        {
            chapter.UpdateContent(command.Content); // WordCount auto-calculated
        }

        // Persist through repository
        await _projectRepository.UpdateAsync(project, cancellationToken);

        // Commit transaction
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Chapter {ChapterId} updated successfully", command.ChapterId);

        return Unit.Value;
    }
}
