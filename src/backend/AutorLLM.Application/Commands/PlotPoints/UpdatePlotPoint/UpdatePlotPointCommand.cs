using MediatR;

namespace AutorLLM.Application.Commands.PlotPoints.UpdatePlotPoint;

/// <summary>
/// Command for updating an existing PlotPoint
/// </summary>
public record UpdatePlotPointCommand : IRequest<UpdatePlotPointResult>
{
    public Guid ProjectId { get; init; }
    public Guid PlotId { get; init; }
    public Guid PlotPointId { get; init; }
    public string Description { get; init; } = string.Empty;
    public int Intensity { get; init; }
}
