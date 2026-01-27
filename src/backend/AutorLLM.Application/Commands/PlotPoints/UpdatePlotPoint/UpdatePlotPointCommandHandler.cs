using AutorLLM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutorLLM.Application.Commands.PlotPoints.UpdatePlotPoint;

/// <summary>
/// Handler for UpdatePlotPointCommand
/// </summary>
public class UpdatePlotPointCommandHandler 
    : IRequestHandler<UpdatePlotPointCommand, UpdatePlotPointResult>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdatePlotPointCommandHandler> _logger;

    public UpdatePlotPointCommandHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdatePlotPointCommandHandler> logger)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<UpdatePlotPointResult> Handle(
        UpdatePlotPointCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Updating PlotPoint {PlotPointId} for Plot {PlotId} in Project {ProjectId}",
            command.PlotPointId,
            command.PlotId,
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

        // Get PlotPoint from Plot
        var plotPoint = plot.PlotPoints.FirstOrDefault(pp => pp.Id == command.PlotPointId);
        if (plotPoint == null)
        {
            _logger.LogWarning(
                "PlotPoint {PlotPointId} not found in Plot {PlotId}",
                command.PlotPointId,
                command.PlotId);
            throw new InvalidOperationException(
                $"PlotPoint {command.PlotPointId} not found in Plot {command.PlotId}.");
        }

        // Update PlotPoint using domain methods
        plotPoint.UpdateDetails(command.Description, command.Intensity);

        // Persist changes through repository
        await _projectRepository.UpdateAsync(project, cancellationToken);

        // Commit transaction
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation(
            "PlotPoint {PlotPointId} updated successfully",
            command.PlotPointId);

        return new UpdatePlotPointResult
        {
            Success = true
        };
    }
}
