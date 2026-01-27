using MediatR;

namespace AutorLLM.Application.Commands.Plots.UpdatePlot;

/// <summary>
/// Command for updating an existing Plot
/// </summary>
public record UpdatePlotCommand : IRequest<Unit>
{
    public Guid ProjectId { get; init; }
    public Guid PlotId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string? Resolution { get; init; }
    public bool IsActive { get; init; }
}
