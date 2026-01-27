using MediatR;

namespace AutorLLM.Application.Commands.Plots.DeletePlot;

/// <summary>
/// Command for deleting a Plot
/// </summary>
public record DeletePlotCommand : IRequest<Unit>
{
    public Guid ProjectId { get; init; }
    public Guid PlotId { get; init; }
}
