using MediatR;

namespace AutorLLM.Application.Commands.PlotPoints.DeletePlotPoint;

/// <summary>
/// Command for deleting a PlotPoint
/// </summary>
public record DeletePlotPointCommand : IRequest<DeletePlotPointResult>
{
    public Guid ProjectId { get; init; }
    public Guid PlotId { get; init; }
    public Guid PlotPointId { get; init; }
}
