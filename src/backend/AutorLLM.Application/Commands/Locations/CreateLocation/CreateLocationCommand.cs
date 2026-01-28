using MediatR;

namespace AutorLLM.Application.Commands.Locations.CreateLocation;

/// <summary>
/// Command for creating a new Location
/// </summary>
public record CreateLocationCommand : IRequest<CreateLocationResult>
{
    public Guid ProjectId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? Geography { get; init; }
    public string? Culture { get; init; }
    public string? Significance { get; init; }
}
