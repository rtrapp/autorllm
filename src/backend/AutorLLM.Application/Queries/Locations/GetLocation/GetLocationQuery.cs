using AutorLLM.Application.DTOs;
using MediatR;

namespace AutorLLM.Application.Queries.Locations.GetLocation;

/// <summary>
/// Query for retrieving a single Location by ID
/// </summary>
public record GetLocationQuery : IRequest<LocationDto>
{
    public Guid LocationId { get; init; }
}
