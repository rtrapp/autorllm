using AutorLLM.Application.DTOs;
using MediatR;

namespace AutorLLM.Application.Queries.Plots.ListPlots;

/// <summary>
/// Query for retrieving all Plots for a specific Project
/// </summary>
public record ListPlotsQuery : IRequest<IEnumerable<PlotDto>>
{
    public Guid ProjectId { get; init; }
}
