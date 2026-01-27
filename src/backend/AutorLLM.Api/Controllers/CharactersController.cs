using AutorLLM.Application.Commands.Characters.CreateCharacter;
using AutorLLM.Application.Commands.Characters.UpdateCharacter;
using AutorLLM.Application.Commands.Characters.DeleteCharacter;
using AutorLLM.Application.Queries.Characters.GetCharacter;
using AutorLLM.Application.Queries.Characters.ListCharacters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AutorLLM.Api.Controllers;

/// <summary>
/// Characters API Controller - manages character entities
/// </summary>
[ApiController]
[Route("api/projects/{projectId}/[controller]")]
[Produces("application/json")]
public class CharactersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CharactersController> _logger;

    public CharactersController(IMediator mediator, ILogger<CharactersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all characters for a project
    /// </summary>
    /// <param name="projectId">Project ID</param>
    /// <returns>List of characters</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAll(Guid projectId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all characters for project: {ProjectId}", projectId);

        var query = new ListCharactersQuery { ProjectId = projectId };
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Get a character by ID
    /// </summary>
    /// <param name="projectId">Project ID (for route consistency)</param>
    /// <param name="id">Character ID</param>
    /// <returns>Character details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid projectId, Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting character: {CharacterId} from project: {ProjectId}", id, projectId);

        var query = new GetCharacterQuery { CharacterId = id };
        var result = await _mediator.Send(query, cancellationToken);

        // Verify the character belongs to the project
        if (result.ProjectId != projectId)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Create a new character
    /// </summary>
    /// <param name="projectId">Project ID</param>
    /// <param name="command">Character creation data</param>
    /// <returns>Created character ID</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(
        Guid projectId,
        [FromBody] CreateCharacterCommand command,
        CancellationToken cancellationToken)
    {
        // Ensure command ProjectId matches route parameter
        if (command.ProjectId != projectId)
            return BadRequest("ProjectId mismatch");

        _logger.LogInformation("Creating character: {Name} in project: {ProjectId}", command.Name, projectId);

        var result = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { projectId, id = result.CharacterId },
            result);
    }

    /// <summary>
    /// Update an existing character
    /// </summary>
    /// <param name="projectId">Project ID</param>
    /// <param name="id">Character ID</param>
    /// <param name="command">Character update data</param>
    /// <returns>No content</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid projectId,
        Guid id,
        [FromBody] UpdateCharacterCommand command,
        CancellationToken cancellationToken)
    {
        // Ensure command IDs match route parameters
        if (id != command.CharacterId || projectId != command.ProjectId)
            return BadRequest("ID mismatch");

        _logger.LogInformation("Updating character: {CharacterId} in project: {ProjectId}", id, projectId);

        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Delete a character
    /// </summary>
    /// <param name="projectId">Project ID</param>
    /// <param name="id">Character ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid projectId,
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting character: {CharacterId} from project: {ProjectId}", id, projectId);

        var command = new DeleteCharacterCommand
        {
            ProjectId = projectId,
            CharacterId = id
        };

        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }
}
