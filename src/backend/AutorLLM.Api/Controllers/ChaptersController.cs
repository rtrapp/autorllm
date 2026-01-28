using AutorLLM.Application.Commands.Chapters.CreateChapter;
using AutorLLM.Application.Commands.Chapters.DeleteChapter;
using AutorLLM.Application.Commands.Chapters.UpdateChapter;
using AutorLLM.Application.Commands.Chapters.ReorderChapters;
using AutorLLM.Application.Queries.Chapters.GetChapter;
using AutorLLM.Application.Queries.Chapters.ListChapters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AutorLLM.Api.Controllers;

/// <summary>
/// Chapters API Controller - manages book chapters
/// </summary>
[ApiController]
[Route("api/projects/{projectId}/chapters")]
[Produces("application/json")]
public class ChaptersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ChaptersController> _logger;

    public ChaptersController(IMediator mediator, ILogger<ChaptersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all chapters for a project
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(Guid projectId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all chapters for project {ProjectId}", projectId);
        var query = new ListChaptersQuery { ProjectId = projectId };
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get a chapter by ID
    /// </summary>
    [HttpGet("{chapterId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid projectId, Guid chapterId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting chapter {ChapterId} for project {ProjectId}", chapterId, projectId);
        var query = new GetChapterQuery { ChapterId = chapterId };
        var result = await _mediator.Send(query, cancellationToken);
        
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Create a new chapter
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        Guid projectId, 
        [FromBody] CreateChapterCommand command, 
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating chapter for project {ProjectId}", projectId);

        if (projectId != command.ProjectId && command.ProjectId != Guid.Empty)
           return BadRequest("ProjectId mismatch");

        var finalCommand = command with { ProjectId = projectId };
        var result = await _mediator.Send(finalCommand, cancellationToken);
        
        // Note: Summary is currently not supported in CreateChapterCommand. 
        // If needed, we should update the command or handle it separately.
        
        return CreatedAtAction(
            nameof(GetById),
            new { projectId = projectId, chapterId = result.ChapterId },
            result);
    }

    /// <summary>
    /// Update a chapter
    /// </summary>
    [HttpPut("{chapterId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(
        Guid projectId,
        Guid chapterId,
        [FromBody] UpdateChapterCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating chapter {ChapterId} for project {ProjectId}", chapterId, projectId);

        if (projectId != command.ProjectId && command.ProjectId != Guid.Empty)
             return BadRequest("ProjectId mismatch");
        
        if (chapterId != command.ChapterId && command.ChapterId != Guid.Empty)
             return BadRequest("ChapterId mismatch");

        var finalCommand = command with { ProjectId = projectId, ChapterId = chapterId };
        
        await _mediator.Send(finalCommand, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Delete a chapter
    /// </summary>
    [HttpDelete("{chapterId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(
        Guid projectId,
        Guid chapterId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting chapter {ChapterId} for project {ProjectId}", chapterId, projectId);
        var command = new DeleteChapterCommand { ProjectId = projectId, ChapterId = chapterId };
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Reorder chapters
    /// </summary>
    [HttpPut("reorder")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Reorder(
        Guid projectId,
        [FromBody] ReorderChaptersCommand command,
        CancellationToken cancellationToken)
    {
         _logger.LogInformation("Reordering chapters for project {ProjectId}", projectId);

         if (projectId != command.ProjectId && command.ProjectId != Guid.Empty)
             return BadRequest("ProjectId mismatch");
             
         var finalCommand = command with { ProjectId = projectId };
         await _mediator.Send(finalCommand, cancellationToken);
         return NoContent();
    }
}
