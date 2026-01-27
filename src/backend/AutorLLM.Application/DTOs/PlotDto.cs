namespace AutorLLM.Application.DTOs;

/// <summary>
/// Data Transfer Object for Plot - used in Queries (read operations)
/// </summary>
public record PlotDto
{
    public Guid Id { get; init; }
    public Guid ProjectId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string? Resolution { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
