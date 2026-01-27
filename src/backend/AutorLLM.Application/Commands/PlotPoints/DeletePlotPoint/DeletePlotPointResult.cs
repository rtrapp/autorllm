namespace AutorLLM.Application.Commands.PlotPoints.DeletePlotPoint;

/// <summary>
/// Result of DeletePlotPointCommand execution
/// </summary>
public record DeletePlotPointResult
{
    public bool Success { get; init; }
}
