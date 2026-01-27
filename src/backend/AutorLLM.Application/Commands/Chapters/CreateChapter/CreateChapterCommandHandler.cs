using AutorLLM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutorLLM.Application.Commands.Chapters.CreateChapter;

/// <summary>
/// Handler for CreateChapterCommand
/// Domain Service assigns Order sequentially via Project aggregate
/// </summary>
public class CreateChapterCommandHandler 
    : IRequestHandler<CreateChapterCommand, CreateChapterResult>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateChapterCommandHandler> _logger;

    public CreateChapterCommandHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateChapterCommandHandler> logger)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<CreateChapterResult> Handle(
        CreateChapterCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating chapter {Title} in project {ProjectId}",
            command.Title,
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

        // Add chapter through aggregate root (Order is assigned sequentially)
        var chapter = project.AddChapter(command.Title);

        // Persist through repository
        await _projectRepository.UpdateAsync(project, cancellationToken);

        // Commit transaction
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation(
            "Chapter created with ID: {ChapterId}, Order: {Order}",
            chapter.Id,
            chapter.Order.Value);

        return new CreateChapterResult
        {
            ChapterId = chapter.Id,
            Order = chapter.Order.Value,
            Success = true
        };
    }
}
