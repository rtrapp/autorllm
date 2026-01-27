using MediatR;

namespace AutorLLM.Application.Commands.Locations.DeleteLocation;

/// <summary>
/// Command for deleting a Location
/// </summary>
public record DeleteLocationCommand : IRequest<Unit>
{
    public Guid ProjectId { get; init; }
    public Guid LocationId { get; init; }
}
