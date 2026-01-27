using AutorLLM.Application.DTOs;
using MediatR;

namespace AutorLLM.Application.Queries.Locations.ListLocations;

/// <summary>
/// Query for retrieving all Locations for a specific Project
/// </summary>
public record ListLocationsQuery : IRequest<IEnumerable<LocationDto>>
{
    public Guid ProjectId { get; init; }
}
