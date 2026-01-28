using AutorLLM.Application.Commands.Plots.CreatePlot;
using AutorLLM.Application.Commands.Plots.UpdatePlot;
using AutorLLM.Application.Commands.Plots.DeletePlot;
using AutorLLM.Application.Queries.Plots.GetPlot;
using AutorLLM.Application.Queries.Plots.ListPlots;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AutorLLM.Api.Controllers;

/// <summary>
/// Plots API Controller - manages plot/narrative arc entities
/// </summary>
[ApiController]
[Route("api/projects/{projectId}/[controller]")]
[Produces("application/json")]
public class PlotsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PlotsController> _logger;

    public PlotsController(IMediator mediator, ILogger<PlotsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all plots for a project
    /// </summary>
    /// <param name="projectId">Project ID</param>
    /// <returns>List of plots</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAll(Guid projectId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all plots for project: {ProjectId}", projectId);

        var query = new ListPlotsQuery { ProjectId = projectId };
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Get a plot by ID
    /// </summary>
    /// <param name="projectId">Project ID (for route consistency)</param>
    /// <param name="id">Plot ID</param>
    /// <returns>Plot details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid projectId, Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting plot: {PlotId} from project: {ProjectId}", id, projectId);

        var query = new GetPlotQuery { PlotId = id };
        var result = await _mediator.Send(query, cancellationToken);

        // Verify the plot belongs to the project
        if (result.ProjectId != projectId)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Create a new plot
    /// </summary>
    /// <param name="projectId">Project ID</param>
    /// <param name="command">Plot creation data</param>
    /// <returns>Created plot ID</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(
        Guid projectId,
        [FromBody] CreatePlotCommand command,
        CancellationToken cancellationToken)
    {
        // Ensure command ProjectId matches route parameter
        if (command.ProjectId != projectId)
            return BadRequest("ProjectId mismatch");

        _logger.LogInformation("Creating plot: {Title} in project: {ProjectId}", command.Title, projectId);

        var result = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { projectId, id = result.PlotId },
            result);
    }

    /// <summary>
    /// Update an existing plot
    /// </summary>
    /// <param name="projectId">Project ID</param>
    /// <param name="id">Plot ID</param>
    /// <param name="command">Plot update data</param>
    /// <returns>No content</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid projectId,
        Guid id,
        [FromBody] UpdatePlotCommand command,
        CancellationToken cancellationToken)
    {
        // Ensure command IDs match route parameters
        if (id != command.PlotId || projectId != command.ProjectId)
            return BadRequest("ID mismatch");

        _logger.LogInformation("Updating plot: {PlotId} in project: {ProjectId}", id, projectId);

        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Delete a plot
    /// </summary>
    /// <param name="projectId">Project ID</param>
    /// <param name="id">Plot ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid projectId,
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting plot: {PlotId} from project: {ProjectId}", id, projectId);

        var command = new DeletePlotCommand
        {
            PlotId = id,
            ProjectId = projectId
        };

        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }
}
