using AutorLLM.Application.DTOs;
using MediatR;

namespace AutorLLM.Application.Queries.Chapters.GetChapter;

/// <summary>
/// Query for retrieving a single Chapter by ID, including its content
/// </summary>
public record GetChapterQuery : IRequest<ChapterDto>
{
    public Guid ChapterId { get; init; }
}
