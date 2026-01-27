using MediatR;

namespace AutorLLM.Application.Commands.Locations.UpdateLocation;

/// <summary>
/// Command for updating an existing Location
/// </summary>
public record UpdateLocationCommand : IRequest<Unit>
{
    public Guid ProjectId { get; init; }
    public Guid LocationId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? Geography { get; init; }
    public string? Culture { get; init; }
    public string? Significance { get; init; }
}
