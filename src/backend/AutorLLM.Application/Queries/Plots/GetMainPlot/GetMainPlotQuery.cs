using AutorLLM.Application.DTOs;
using MediatR;

namespace AutorLLM.Application.Queries.Plots.GetMainPlot;

/// <summary>
/// Query for retrieving the Main Plot of a Project
/// </summary>
public record GetMainPlotQuery : IRequest<PlotDto?>
{
    public Guid ProjectId { get; init; }
}
