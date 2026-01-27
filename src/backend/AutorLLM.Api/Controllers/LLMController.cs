using AutorLLM.Application.Services;
using AutorLLM.Infrastructure.Exceptions;
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
            var response = await _agentService.CompleteAsync(request.Prompt, cancellationToken);
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
