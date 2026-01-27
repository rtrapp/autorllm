namespace AutorLLM.Application.DTOs;

/// <summary>
/// Data Transfer Object for Project - used in Queries (read operations)
/// </summary>
public record ProjectDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;
    public string Synopsis { get; init; } = string.Empty;
    public string? Genre { get; init; }
    public int TargetWordCount { get; init; }
    public int CurrentWordCount { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
