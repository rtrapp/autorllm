using AutorLLM.Domain.Interfaces;
using AutorLLM.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutorLLM.Application.Commands.Plots.UpdatePlot;

/// <summary>
/// Handler for UpdatePlotCommand
/// </summary>
public class UpdatePlotCommandHandler : IRequestHandler<UpdatePlotCommand, Unit>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdatePlotCommandHandler> _logger;

    public UpdatePlotCommandHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdatePlotCommandHandler> logger)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Unit> Handle(
        UpdatePlotCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Updating plot {PlotId} in project {ProjectId}",
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

        // Get plot from aggregate
        var plot = project.GetPlot(command.PlotId);

        // Parse plot type
        var plotType = PlotType.Create(command.Type);

        // Update through domain methods
        plot.UpdateDetails(command.Title, command.Description, plotType);
        plot.SetResolution(command.Resolution);
        
        if (command.IsActive)
            plot.Activate();
        else
            plot.Deactivate();

        // Persist changes through repository
        await _projectRepository.UpdateAsync(project, cancellationToken);

        // Commit transaction
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation(
            "Plot {PlotId} updated successfully",
            command.PlotId);

        return Unit.Value;
    }
}
