namespace AutorLLM.Application.Commands.PlotPoints.UpdatePlotPoint;

/// <summary>
/// Result of UpdatePlotPointCommand execution
/// </summary>
public record UpdatePlotPointResult
{
    public bool Success { get; init; }
}
