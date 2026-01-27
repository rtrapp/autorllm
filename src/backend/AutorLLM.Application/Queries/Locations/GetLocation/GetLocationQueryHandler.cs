using AutorLLM.Application.DTOs;
using AutorLLM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutorLLM.Application.Queries.Locations.GetLocation;

/// <summary>
/// Handler for GetLocationQuery
/// </summary>
public class GetLocationQueryHandler : IRequestHandler<GetLocationQuery, LocationDto>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ILogger<GetLocationQueryHandler> _logger;

    public GetLocationQueryHandler(
        IProjectRepository projectRepository,
        ILogger<GetLocationQueryHandler> logger)
    {
        _projectRepository = projectRepository;
        _logger = logger;
    }

    public async Task<LocationDto> Handle(
        GetLocationQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Retrieving location {LocationId}",
            query.LocationId);

        // We need to find the location by searching through projects
        var projects = await _projectRepository.GetAllAsync(cancellationToken);
        
        foreach (var project in projects)
        {
            var location = project.Locations.FirstOrDefault(l => l.Id == query.LocationId);
            if (location != null)
            {
                _logger.LogInformation(
                    "Location {LocationId} found in project {ProjectId}",
                    query.LocationId,
                    project.Id);

                return new LocationDto
                {
                    Id = location.Id,
                    ProjectId = location.ProjectId,
                    Name = location.Name,
                    Description = location.Description,
                    Geography = location.Geography,
                    Culture = location.Culture,
                    Significance = location.Significance,
                    CreatedAt = location.CreatedAt,
                    UpdatedAt = location.UpdatedAt
                };
            }
        }

        _logger.LogWarning("Location {LocationId} not found", query.LocationId);
        throw new InvalidOperationException($"Location {query.LocationId} not found.");
    }
}
