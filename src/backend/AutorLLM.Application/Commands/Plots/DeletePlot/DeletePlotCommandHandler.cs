using AutorLLM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutorLLM.Application.Commands.Plots.DeletePlot;

/// <summary>
/// Handler for DeletePlotCommand
/// </summary>
public class DeletePlotCommandHandler : IRequestHandler<DeletePlotCommand, Unit>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeletePlotCommandHandler> _logger;

    public DeletePlotCommandHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeletePlotCommandHandler> logger)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Unit> Handle(
        DeletePlotCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Deleting plot {PlotId} from project {ProjectId}",
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

        // Remove plot through aggregate root (validates business rules)
        // The CASCADE DELETE in SQL will handle PlotPoints automatically
        project.RemovePlot(command.PlotId);

        // Persist changes through repository
        await _projectRepository.UpdateAsync(project, cancellationToken);

        // Commit transaction
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation(
            "Plot {PlotId} deleted successfully",
            command.PlotId);

        return Unit.Value;
    }
}
