using AutorLLM.Application.Commands.Projects.CreateProject;
using AutorLLM.Application.Commands.Projects.UpdateProject;
using AutorLLM.Application.Commands.Projects.DeleteProject;
using AutorLLM.Application.Queries.Projects.GetProject;
using AutorLLM.Application.Queries.Projects.ListProjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AutorLLM.Api.Controllers;

/// <summary>
/// Projects API Controller - manages book projects
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(IMediator mediator, ILogger<ProjectsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all projects
    /// </summary>
    /// <returns>List of all projects</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all projects");

        var query = new ListProjectsQuery();
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Get a project by ID
    /// </summary>
    /// <param name="id">Project ID</param>
    /// <returns>Project details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting project: {ProjectId}", id);

        var query = new GetProjectQuery { ProjectId = id };
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Create a new project
    /// </summary>
    /// <param name="command">Project creation data</param>
    /// <returns>Created project ID</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateProjectCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating project: {Title}", command.Title);

        var result = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.ProjectId },
            result);
    }

    /// <summary>
    /// Update an existing project
    /// </summary>
    /// <param name="id">Project ID</param>
    /// <param name="command">Project update data</param>
    /// <returns>No content</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateProjectCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.ProjectId)
            return BadRequest("ID mismatch");

        _logger.LogInformation("Updating project: {ProjectId}", id);

        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Delete a project
    /// </summary>
    /// <param name="id">Project ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting project: {ProjectId}", id);

        var command = new DeleteProjectCommand { ProjectId = id };
        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }
}
