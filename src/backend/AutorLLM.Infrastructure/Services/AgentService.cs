using AutorLLM.Application.Services;
using AutorLLM.Infrastructure.Configuration;
using Microsoft.Agents.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;

namespace AutorLLM.Infrastructure.Services;

/// <summary>
/// Implementação do IAgentService usando Microsoft Agent Framework com Ollama.
/// Usa OllamaSharp.AsAIAgent() conforme exemplo oficial do Microsoft Agent Framework.
/// </summary>
public class AgentService : IAgentService
{
    private readonly AIAgent _agent;
    private readonly ILogger<AgentService> _logger;
    private readonly AgentFrameworkOptions _options;

    public AgentService(
        AIAgent agent,
        IOptions<AgentFrameworkOptions> options,
        ILogger<AgentService> logger)
    {
        ArgumentNullException.ThrowIfNull(agent);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(logger);

        _agent = agent;
        _logger = logger;
        _options = options.Value;
    }

    public async IAsyncEnumerable<string> StreamCompletionAsync(
        string prompt,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prompt, nameof(prompt));

        _logger.LogInformation("Streaming completion for prompt length: {Length}", prompt.Length);

        await foreach (var update in _agent.RunStreamingAsync(prompt, cancellationToken: cancellationToken))
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                yield return update.Text;
            }
        }

        _logger.LogInformation("Streaming completion finished");
    }

    public async Task<string> CompleteAsync(
        string prompt,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prompt, nameof(prompt));

        _logger.LogInformation("Running completion for prompt length: {Length}", prompt.Length);

        var response = await _agent.RunAsync(prompt, cancellationToken: cancellationToken);
        var result = response.Text ?? string.Empty;

        _logger.LogInformation("Completion finished. Result length: {Length}", result.Length);

        return result;
    }

    public async Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Running health check for Ollama at {Endpoint}", _options.Ollama.Endpoint);

            var response = await _agent.RunAsync("Say 'OK' if you can read this.", cancellationToken: cancellationToken);
            var isHealthy = response.Text?.Contains("OK", StringComparison.OrdinalIgnoreCase) ?? false;

            _logger.LogInformation("Health check result: {IsHealthy}", isHealthy);

            return isHealthy;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed for Ollama at {Endpoint}", _options.Ollama.Endpoint);
            return false;
        }
    }
}
