using AutorLLM.Application.Commands.Locations.CreateLocation;
using AutorLLM.Application.Commands.Locations.UpdateLocation;
using AutorLLM.Application.Commands.Locations.DeleteLocation;
using AutorLLM.Application.Queries.Locations.GetLocation;
using AutorLLM.Application.Queries.Locations.ListLocations;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AutorLLM.Api.Controllers;

/// <summary>
/// Locations API Controller - manages location entities
/// </summary>
[ApiController]
[Route("api/projects/{projectId}/[controller]")]
[Produces("application/json")]
public class LocationsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<LocationsController> _logger;

    public LocationsController(IMediator mediator, ILogger<LocationsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all locations for a project
    /// </summary>
    /// <param name="projectId">Project ID</param>
    /// <returns>List of locations</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAll(Guid projectId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all locations for project: {ProjectId}", projectId);

        var query = new ListLocationsQuery { ProjectId = projectId };
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Get a location by ID
    /// </summary>
    /// <param name="projectId">Project ID (for route consistency)</param>
    /// <param name="id">Location ID</param>
    /// <returns>Location details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid projectId, Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting location: {LocationId} from project: {ProjectId}", id, projectId);

        var query = new GetLocationQuery { LocationId = id };
        var result = await _mediator.Send(query, cancellationToken);

        // Verify the location belongs to the project
        if (result.ProjectId != projectId)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Create a new location
    /// </summary>
    /// <param name="projectId">Project ID</param>
    /// <param name="command">Location creation data</param>
    /// <returns>Created location ID</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(
        Guid projectId,
        [FromBody] CreateLocationCommand command,
        CancellationToken cancellationToken)
    {
        // Ensure command ProjectId matches route parameter
        if (command.ProjectId != projectId)
            return BadRequest("ProjectId mismatch");

        _logger.LogInformation("Creating location: {Name} in project: {ProjectId}", command.Name, projectId);

        var result = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { projectId, id = result.LocationId },
            result);
    }

    /// <summary>
    /// Update an existing location
    /// </summary>
    /// <param name="projectId">Project ID</param>
    /// <param name="id">Location ID</param>
    /// <param name="command">Location update data</param>
    /// <returns>No content</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid projectId,
        Guid id,
        [FromBody] UpdateLocationCommand command,
        CancellationToken cancellationToken)
    {
        // Ensure command IDs match route parameters
        if (id != command.LocationId || projectId != command.ProjectId)
            return BadRequest("ID mismatch");

        _logger.LogInformation("Updating location: {LocationId} in project: {ProjectId}", id, projectId);

        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Delete a location
    /// </summary>
    /// <param name="projectId">Project ID</param>
    /// <param name="id">Location ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid projectId,
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting location: {LocationId} from project: {ProjectId}", id, projectId);

        var command = new DeleteLocationCommand
        {
            ProjectId = projectId,
            LocationId = id
        };

        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }
}
