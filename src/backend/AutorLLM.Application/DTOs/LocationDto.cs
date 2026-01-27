namespace AutorLLM.Application.DTOs;

/// <summary>
/// Data Transfer Object for Location - used in Queries (read operations)
/// </summary>
public record LocationDto
{
    public Guid Id { get; init; }
    public Guid ProjectId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? Geography { get; init; }
    public string? Culture { get; init; }
    public string? Significance { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
