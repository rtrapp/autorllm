using AutorLLM.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutorLLM.Api.Controllers;

/// <summary>
/// LLM API Controller - for testing Agent Framework integration
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class LLMController : ControllerBase
{
    private readonly IAgentService _agentService;
    private readonly ILogger<LLMController> _logger;

    public LLMController(IAgentService agentService, ILogger<LLMController> logger)
    {
        _agentService = agentService;
        _logger = logger;
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return StatusCode(500, new { status = "error", message = ex.Message });
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
            var response = await _agentService.CompleteAsync(request.Prompt, cancellationToken);
            return Ok(new { response, timestamp = DateTime.UtcNow });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Completion failed");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Test streaming - returns SSE (Server-Sent Events)
    /// </summary>
    [HttpPost("stream")]
    public async Task StreamCompletion([FromBody] CompleteRequest request, CancellationToken cancellationToken)
    {
        Response.ContentType = "text/event-stream";
        Response.Headers.Add("Cache-Control", "no-cache");
        Response.Headers.Add("Connection", "keep-alive");

        try
        {
            await foreach (var token in _agentService.StreamCompletionAsync(request.Prompt, cancellationToken))
            {
                await Response.WriteAsync($"data: {token}\n\n");
                await Response.Body.FlushAsync(cancellationToken);
            }
            
            await Response.WriteAsync("data: [DONE]\n\n");
            await Response.Body.FlushAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Streaming failed");
            await Response.WriteAsync($"data: {{\"error\": \"{ex.Message}\"}}\n\n");
        }
    }
}

public record CompleteRequest(string Prompt);
