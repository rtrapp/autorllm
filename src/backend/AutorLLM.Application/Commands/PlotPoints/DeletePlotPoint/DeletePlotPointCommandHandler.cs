using AutorLLM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutorLLM.Application.Commands.PlotPoints.DeletePlotPoint;

/// <summary>
/// Handler for DeletePlotPointCommand
/// </summary>
public class DeletePlotPointCommandHandler 
    : IRequestHandler<DeletePlotPointCommand, DeletePlotPointResult>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeletePlotPointCommandHandler> _logger;

    public DeletePlotPointCommandHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeletePlotPointCommandHandler> logger)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<DeletePlotPointResult> Handle(
        DeletePlotPointCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Deleting PlotPoint {PlotPointId} from Plot {PlotId} in Project {ProjectId}",
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

        // Remove PlotPoint through Plot (encapsulated business logic)
        plot.RemovePlotPoint(command.PlotPointId);

        // Persist changes through repository
        await _projectRepository.UpdateAsync(project, cancellationToken);

        // Commit transaction
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation(
            "PlotPoint {PlotPointId} deleted successfully",
            command.PlotPointId);

        return new DeletePlotPointResult
        {
            Success = true
        };
    }
}
