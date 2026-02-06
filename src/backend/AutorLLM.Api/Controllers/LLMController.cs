using AutorLLM.Application.AgentDefinitions;
using AutorLLM.Application.Commands.LLM.StartBrainstorm;
using AutorLLM.Application.Commands.Brainstorm;
using AutorLLM.Application.Services;
using AutorLLM.Infrastructure.Exceptions;
using Microsoft.Agents.AI;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AutorLLM.Api.Controllers;

/// <summary>
/// LLM API Controller - for testing Agent Framework integration and brainstorming
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class LLMController : ControllerBase
{
    private readonly IAgentService _agentService;
    private readonly AutorLLM.Application.AgentDefinitions.BrainstormAgentDefinition _brainstormAgent;
    private readonly IMediator _mediator;
    private readonly ILogger<LLMController> _logger;

    public LLMController(
        IAgentService agentService,
        AutorLLM.Application.AgentDefinitions.BrainstormAgentDefinition brainstormAgent,
        IMediator mediator,
        ILogger<LLMController> logger)
    {
        _agentService = agentService;
        _brainstormAgent = brainstormAgent;
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Start brainstorm session - initiates conversation with LLM about book idea
    /// </summary>
    [HttpPost("brainstorm/start")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> StartBrainstorm(
        [FromBody] StartBrainstormRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new StartBrainstormCommand
            {
                BookIdea = request.BookIdea
            };

            var result = await _mediator.Send(command, cancellationToken);

            return Ok(result);
        }
        catch (OllamaConnectionException ex)
        {
            _logger.LogError(ex, "Ollama connection failed during brainstorm start");
            return StatusCode(503, new
            {
                error = "LLM não disponível. Verifique se o Ollama está rodando.",
                details = new
                {
                    endpoint = ex.Endpoint,
                    model = ex.Model,
                    retryAttempts = ex.RetryAttempts
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start brainstorm session");
            return StatusCode(500, new { error = "Erro interno ao iniciar brainstorm" });
        }
    }

    /// <summary>
    /// Generate outline from accumulated brainstorm context
    /// </summary>
    [HttpPost("brainstorm/generate-outline")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GenerateOutline(
        [FromBody] GenerateOutlineRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new GenerateOutlineCommand
            {
                SessionId = request.SessionId,
                BookIdea = request.BookIdea,
                Title = request.Title,
                Author = request.Author,
                Genre = request.Genre,
                Synopsis = request.Synopsis,
                Tone = request.Tone,
                TargetAudience = request.TargetAudience,
                Characters = request.Characters?.Select(c => new CharacterSuggestion
                {
                    Name = c.Name,
                    Description = c.Description,
                    Role = c.Role,
                    Backstory = c.Backstory,
                    Appearance = c.Appearance,
                    Personality = c.Personality
                }).ToList(),
                Locations = request.Locations?.Select(l => new LocationSuggestion
                {
                    Name = l.Name,
                    Description = l.Description,
                    Geography = l.Geography,
                    Culture = l.Culture,
                    Significance = l.Significance
                }).ToList(),
                Plots = request.Plots?.Select(p => new PlotSuggestion
                {
                    Title = p.Title,
                    Description = p.Description,
                    Type = p.Type,
                    Resolution = p.Resolution
                }).ToList(),
                Chapters = request.Chapters?.Select(ch => new ChapterSuggestion
                {
                    Title = ch.Title,
                    Summary = ch.Summary,
                    Order = ch.Order
                }).ToList()
            };

            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsValid)
            {
                return BadRequest(new
                {
                    errors = result.ValidationErrors,
                    outline = result.Outline
                });
            }

            return Ok(result);
        }
        catch (OllamaConnectionException ex)
        {
            _logger.LogError(ex, "Ollama connection failed during outline generation");
            return StatusCode(503, new
            {
                error = "LLM não disponível. Verifique se o Ollama está rodando.",
                details = new
                {
                    endpoint = ex.Endpoint,
                    model = ex.Model,
                    retryAttempts = ex.RetryAttempts
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate outline");
            return StatusCode(500, new { error = "Erro interno ao gerar outline" });
        }
    }

    /// <summary>
    /// Health check - verifies Ollama connection
    /// </summary>
    [HttpGet("health")]
    public async Task<IActionResult> HealthCheck(CancellationToken cancellationToken)
    {
        try
        {
            var isHealthy = await _agentService.HealthCheckAsync(cancellationToken);
            
            return Ok(new 
            { 
                status = isHealthy ? "healthy" : "unhealthy",
                timestamp = DateTime.UtcNow
            });
        }
        catch (OllamaConnectionException ex)
        {
            _logger.LogError(ex, "Ollama connection failed during health check");
            return StatusCode(503, new 
            { 
                status = "error",
                message = "LLM não disponível. Verifique se o Ollama está rodando.",
                endpoint = ex.Endpoint,
                model = ex.Model,
                retryAttempts = ex.RetryAttempts
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed with unexpected error");
            return StatusCode(500, new { status = "error", message = "Erro interno ao verificar saúde do LLM" });
        }
    }

    /// <summary>
    /// Test completion - simple prompt without streaming
    /// </summary>
    [HttpPost("complete")]
    public async Task<IActionResult> Complete([FromBody] CompleteRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var simpleAgent = new SimpleAgentDefinition();
            
            var (response, _) = await _agentService.CompleteAsync(simpleAgent, request.Prompt, sessionJson: null, cancellationToken);
            return Ok(new { response, timestamp = DateTime.UtcNow });
        }
        catch (OllamaConnectionException ex)
        {
            _logger.LogError(ex, "Ollama connection failed during completion");
            return StatusCode(503, new 
            { 
                error = "LLM não disponível. Verifique se o Ollama está rodando.",
                details = new 
                {
                    endpoint = ex.Endpoint,
                    model = ex.Model,
                    retryAttempts = ex.RetryAttempts
                }
            });
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Completion cancelled by user");
            return StatusCode(499, new { error = "Requisição cancelada pelo usuário" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Completion failed with unexpected error");
            return StatusCode(500, new { error = "Erro interno ao processar completion" });
        }
    }

    /// <summary>
    /// Test streaming - returns SSE (Server-Sent Events)
    /// </summary>
    [HttpPost("stream")]
    public async Task StreamCompletion([FromBody] CompleteRequest request, CancellationToken cancellationToken)
    {
        Response.ContentType = "text/event-stream";
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");

        try
        {
            var simpleAgent = new SimpleAgentDefinition();
            await foreach (var (token, _) in _agentService.StreamCompletionAsync(simpleAgent, request.Prompt, sessionJson: null, cancellationToken))
            {
                if (!string.IsNullOrEmpty(token))
                {
                    await Response.WriteAsync($"data: {token}\n\n");
                    await Response.Body.FlushAsync(cancellationToken);
                }
            }
            
            await Response.WriteAsync("data: [DONE]\n\n");
            await Response.Body.FlushAsync(cancellationToken);
        }
        catch (OllamaConnectionException ex)
        {
            _logger.LogError(ex, "Ollama connection failed during streaming");
            var errorData = System.Text.Json.JsonSerializer.Serialize(new 
            { 
                error = "LLM não disponível. Verifique se o Ollama está rodando.",
                endpoint = ex.Endpoint,
                model = ex.Model,
                retryAttempts = ex.RetryAttempts
            });
            await Response.WriteAsync($"data: {errorData}\n\n");
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Streaming cancelled by user");
            await Response.WriteAsync("data: {\"error\": \"Streaming cancelado pelo usuário\"}\n\n");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Streaming failed with unexpected error");
            await Response.WriteAsync($"data: {{\"error\": \"Erro interno ao processar streaming\"}}\n\n");
        }
    }
}

public record CompleteRequest(string Prompt);

public record StartBrainstormRequest(string BookIdea);

public record GenerateOutlineRequest(
    string SessionId,
    string BookIdea,
    string? Title = null,
    string? Author = null,
    string? Genre = null,
    string? Synopsis = null,
    string? Tone = null,
    string? TargetAudience = null,
    List<CharacterSuggestionDto>? Characters = null,
    List<LocationSuggestionDto>? Locations = null,
    List<PlotSuggestionDto>? Plots = null,
    List<ChapterSuggestionDto>? Chapters = null
);

public record CharacterSuggestionDto(
    string Name,
    string? Description = null,
    string? Role = null,
    string? Backstory = null,
    string? Appearance = null,
    string? Personality = null
);

public record LocationSuggestionDto(
    string Name,
    string? Description = null,
    string? Geography = null,
    string? Culture = null,
    string? Significance = null
);

public record PlotSuggestionDto(
    string Title,
    string? Description = null,
    string? Type = null,
    string? Resolution = null
);

public record ChapterSuggestionDto(
    string Title,
    string? Summary = null,
    int Order = 0
);
