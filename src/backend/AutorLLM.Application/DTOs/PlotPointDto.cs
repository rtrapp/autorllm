namespace AutorLLM.Application.DTOs;

/// <summary>
/// Data Transfer Object for PlotPoint - used in Queries (read operations)
/// </summary>
public record PlotPointDto
{
    public Guid Id { get; init; }
    public Guid PlotId { get; init; }
    public Guid ChapterId { get; init; }
    public string Description { get; init; } = string.Empty;
    public int Intensity { get; init; }
    public int Order { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
