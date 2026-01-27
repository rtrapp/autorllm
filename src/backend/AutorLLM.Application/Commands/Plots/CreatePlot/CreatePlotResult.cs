namespace AutorLLM.Application.Commands.Plots.CreatePlot;

/// <summary>
/// Result of CreatePlotCommand
/// </summary>
public record CreatePlotResult
{
    public Guid PlotId { get; init; }
    public bool Success { get; init; }
}
