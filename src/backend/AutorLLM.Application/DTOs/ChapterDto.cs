namespace AutorLLM.Application.DTOs;

/// <summary>
/// Data Transfer Object for Chapter - used in Queries (read operations)
/// </summary>
public record ChapterDto
{
    public Guid Id { get; init; }
    public Guid ProjectId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Summary { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public int Order { get; init; }
    public int WordCount { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
