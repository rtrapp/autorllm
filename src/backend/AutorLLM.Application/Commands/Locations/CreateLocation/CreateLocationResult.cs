namespace AutorLLM.Application.Commands.Locations.CreateLocation;

/// <summary>
/// Result returned after successfully creating a Location
/// </summary>
public record CreateLocationResult
{
    public Guid LocationId { get; init; }
    public bool Success { get; init; }
}
