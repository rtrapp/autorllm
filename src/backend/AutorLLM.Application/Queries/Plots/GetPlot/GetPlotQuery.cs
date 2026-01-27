using AutorLLM.Application.DTOs;
using MediatR;

namespace AutorLLM.Application.Queries.Plots.GetPlot;

/// <summary>
/// Query for retrieving a single Plot by ID
/// </summary>
public record GetPlotQuery : IRequest<PlotDto>
{
    public Guid PlotId { get; init; }
}
