using AutorLLM.Application.DTOs;
using MediatR;

namespace AutorLLM.Application.Queries.Chapters.ListChapters;

/// <summary>
/// Query for retrieving all Chapters for a specific Project
/// </summary>
public record ListChaptersQuery : IRequest<IEnumerable<ChapterDto>>
{
    public Guid ProjectId { get; init; }
}
