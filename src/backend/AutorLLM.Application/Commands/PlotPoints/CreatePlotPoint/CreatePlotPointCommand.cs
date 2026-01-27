using MediatR;

namespace AutorLLM.Application.Commands.PlotPoints.CreatePlotPoint;

/// <summary>
/// Command for creating a new PlotPoint
/// </summary>
public record CreatePlotPointCommand : IRequest<CreatePlotPointResult>
{
    public Guid ProjectId { get; init; }
    public Guid PlotId { get; init; }
    public Guid ChapterId { get; init; }
    public string Description { get; init; } = string.Empty;
    public int Intensity { get; init; }
}
