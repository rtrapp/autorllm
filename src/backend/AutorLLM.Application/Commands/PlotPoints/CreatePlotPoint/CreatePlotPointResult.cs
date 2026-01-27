namespace AutorLLM.Application.Commands.PlotPoints.CreatePlotPoint;

/// <summary>
/// Result of CreatePlotPointCommand execution
/// </summary>
public record CreatePlotPointResult
{
    public Guid PlotPointId { get; init; }
    public bool Success { get; init; }
}
