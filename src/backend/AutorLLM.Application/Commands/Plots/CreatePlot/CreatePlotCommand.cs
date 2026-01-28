using MediatR;

namespace AutorLLM.Application.Commands.Plots.CreatePlot;

/// <summary>
/// Command for creating a new Plot
/// </summary>
public record CreatePlotCommand : IRequest<CreatePlotResult>
{
    public Guid ProjectId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string? Resolution { get; init; }
}
