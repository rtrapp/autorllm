using AutorLLM.Domain.Interfaces;
using AutorLLM.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutorLLM.Application.Commands.Plots.CreatePlot;

/// <summary>
/// Handler for CreatePlotCommand
/// </summary>
public class CreatePlotCommandHandler 
    : IRequestHandler<CreatePlotCommand, CreatePlotResult>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreatePlotCommandHandler> _logger;

    public CreatePlotCommandHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreatePlotCommandHandler> logger)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<CreatePlotResult> Handle(
        CreatePlotCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating plot {Title} in project {ProjectId}",
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

        // Parse plot type
        var plotType = PlotType.Create(command.Type);

        // Add plot through aggregate root (encapsulated business logic)
        var plot = project.AddPlot(
            command.Title,
            command.Description,
            plotType);

        // Persist through repository
        await _projectRepository.UpdateAsync(project, cancellationToken);

        // Commit transaction
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation(
            "Plot created with ID: {PlotId}",
            plot.Id);

        return new CreatePlotResult
        {
            PlotId = plot.Id,
            Success = true
        };
    }
}
