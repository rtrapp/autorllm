using AutorLLM.Application.DTOs;
using MediatR;

namespace AutorLLM.Application.Queries.PlotPoints.GetPlotPointsByPlot;

/// <summary>
/// Query for retrieving all PlotPoints for a specific Plot, ordered by Chapter.Order
/// </summary>
public record GetPlotPointsByPlotQuery : IRequest<IEnumerable<PlotPointDto>>
{
    public Guid ProjectId { get; init; }
    public Guid PlotId { get; init; }
}
