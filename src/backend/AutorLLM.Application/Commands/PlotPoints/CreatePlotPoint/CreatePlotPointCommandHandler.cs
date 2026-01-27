using AutorLLM.Domain.Entities;
using AutorLLM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutorLLM.Application.Commands.PlotPoints.CreatePlotPoint;

/// <summary>
/// Handler for CreatePlotPointCommand
/// </summary>
public class CreatePlotPointCommandHandler 
    : IRequestHandler<CreatePlotPointCommand, CreatePlotPointResult>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreatePlotPointCommandHandler> _logger;

    public CreatePlotPointCommandHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreatePlotPointCommandHandler> logger)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<CreatePlotPointResult> Handle(
        CreatePlotPointCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating PlotPoint for Plot {PlotId} in Chapter {ChapterId} of Project {ProjectId}",
            command.PlotId,
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

        // Get Plot from aggregate
        var plot = project.GetPlot(command.PlotId);

        // Validate that Chapter exists
        var chapter = project.GetChapter(command.ChapterId);

        // Validate unique constraint: one PlotPoint per Plot per Chapter
        if (plot.PlotPoints.Any(pp => pp.ChapterId == command.ChapterId))
        {
            _logger.LogWarning(
                "PlotPoint already exists for Plot {PlotId} in Chapter {ChapterId}",
                command.PlotId,
                command.ChapterId);
            throw new InvalidOperationException(
                $"Plot {command.PlotId} already has a PlotPoint in Chapter {command.ChapterId}. " +
                "A plot can only have one point per chapter.");
        }

        // Create PlotPoint using factory method
        // Order is determined by the number of existing PlotPoints
        var order = plot.PlotPoints.Count;
        var plotPoint = PlotPoint.Create(
            command.PlotId,
            command.ChapterId,
            command.Description,
            command.Intensity,
            order);

        // Add PlotPoint through Plot (encapsulated business logic)
        plot.AddPlotPoint(plotPoint);

        // Persist changes through repository
        await _projectRepository.UpdateAsync(project, cancellationToken);

        // Commit transaction
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation(
            "PlotPoint created with ID: {PlotPointId}",
            plotPoint.Id);

        return new CreatePlotPointResult
        {
            PlotPointId = plotPoint.Id,
            Success = true
        };
    }
}
