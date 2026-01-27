using AutorLLM.Application.DTOs;
using AutorLLM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutorLLM.Application.Queries.Locations.ListLocations;

/// <summary>
/// Handler for ListLocationsQuery
/// </summary>
public class ListLocationsQueryHandler 
    : IRequestHandler<ListLocationsQuery, IEnumerable<LocationDto>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ILogger<ListLocationsQueryHandler> _logger;

    public ListLocationsQueryHandler(
        IProjectRepository projectRepository,
        ILogger<ListLocationsQueryHandler> logger)
    {
        _projectRepository = projectRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<LocationDto>> Handle(
        ListLocationsQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Retrieving all locations for project {ProjectId}",
            query.ProjectId);

        // Load Project aggregate
        var project = await _projectRepository.GetByIdAsync(
            query.ProjectId,
            cancellationToken);

        if (project == null)
        {
            _logger.LogWarning("Project {ProjectId} not found", query.ProjectId);
            throw new InvalidOperationException($"Project {query.ProjectId} not found.");
        }

        // Map locations to DTOs
        var locationDtos = project.Locations.Select(location => new LocationDto
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
        }).ToList();

        _logger.LogInformation(
            "Retrieved {Count} locations for project {ProjectId}",
            locationDtos.Count,
            query.ProjectId);

        return locationDtos;
    }
}
