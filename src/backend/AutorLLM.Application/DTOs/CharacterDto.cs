namespace AutorLLM.Application.DTOs;

/// <summary>
/// Data Transfer Object for Character - used in Queries (read operations)
/// </summary>
public record CharacterDto
{
    public Guid Id { get; init; }
    public Guid ProjectId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public string? Backstory { get; init; }
    public string? Appearance { get; init; }
    public string? Personality { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
