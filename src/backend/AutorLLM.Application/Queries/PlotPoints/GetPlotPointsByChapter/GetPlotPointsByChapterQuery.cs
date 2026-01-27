using AutorLLM.Application.DTOs;
using MediatR;

namespace AutorLLM.Application.Queries.PlotPoints.GetPlotPointsByChapter;

/// <summary>
/// Query for retrieving all PlotPoints for a specific Chapter
/// </summary>
public record GetPlotPointsByChapterQuery : IRequest<IEnumerable<PlotPointDto>>
{
    public Guid ProjectId { get; init; }
    public Guid ChapterId { get; init; }
}
