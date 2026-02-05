using AutorLLM.Application.Services;
using AutorLLM.Infrastructure.Configuration;
using AutorLLM.Infrastructure.Exceptions;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using System.Runtime.CompilerServices;

namespace AutorLLM.Infrastructure.Services;

/// <summary>
/// Implementação do IAgentService usando Microsoft Agent Framework com Ollama.
/// Usa OllamaSharp.AsAIAgent() conforme exemplo oficial do Microsoft Agent Framework.
/// </summary>
public class AgentService : IAgentService
{
    private readonly IChatClient _chatClient;
    private readonly ILogger<AgentService> _logger;
    private readonly AgentFrameworkOptions _options;

    public AgentService(
        IChatClient chatClient,
        IOptions<AgentFrameworkOptions> options,
        ILogger<AgentService> logger)
    {
        ArgumentNullException.ThrowIfNull(chatClient);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(logger);

        _chatClient = chatClient;
        _logger = logger;
        _options = options.Value;
    }

    private ChatClientAgent GetChatClientAgent(BaseAgent agent)
    {
        return _chatClient.AsAIAgent(agent.Name, agent.Instructions);
    }

    private ChatClientAgent GetChatClientAgent<T>(BaseAgent agent)
    {
        return _chatClient.AsAIAgent(new ChatClientAgentOptions()
        {
            Name = agent.Name,
            ChatOptions = new()
            {
                Instructions = agent.Instructions,
                ResponseFormat = Microsoft.Extensions.AI.ChatResponseFormat.ForJsonSchema<T>()
            }
        });
    }

    public async IAsyncEnumerable<string> StreamCompletionAsync(
        BaseAgent agent,
        string prompt,
        AgentThread? thread,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prompt, nameof(prompt));

        _logger.LogInformation("Streaming completion for prompt length: {Length}", prompt.Length);

        await foreach (var token in StreamInternalAsync(agent, prompt, thread, cancellationToken))
        {
            yield return token;
        }
    }

    private async IAsyncEnumerable<string> StreamInternalAsync(
        BaseAgent agent,
        string prompt,
        AgentThread? thread,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var tokenCount = 0;

        var aiAgent = GetChatClientAgent(agent);

        await foreach (var update in aiAgent.RunStreamingAsync(prompt, thread, cancellationToken: cancellationToken))
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                tokenCount++;
                yield return update.Text;
            }
        }

        _logger.LogInformation("Streaming completion finished. Tokens streamed: {TokenCount}", tokenCount);
    }

    public async Task<string> CompleteAsync(
        BaseAgent agent,
        string prompt,
        AgentThread? thread,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prompt, nameof(prompt));

        _logger.LogInformation("Running completion for prompt length: {Length}", prompt.Length);

        try
        {
            var aiAgent = GetChatClientAgent(agent);
            var response = await aiAgent.RunAsync(prompt, thread, cancellationToken: cancellationToken);
            var result = response.Text ?? string.Empty;

            _logger.LogInformation("Completion finished. Result length: {Length}", result.Length);

            return result;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Completion cancelled by user");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Completion failed. Endpoint: {Endpoint}, Model: {Model}",
                _options.Ollama.Endpoint,
                _options.Ollama.Model
            );

            throw new OllamaConnectionException(
                "LLM não disponível. Verifique se o Ollama está rodando.",
                ex
            )
            {
                Endpoint = _options.Ollama.Endpoint,
                Model = _options.Ollama.Model,
                RetryAttempts = 0
            };
        }
    }

    public async Task<T> CompleteStructuredAsync<T>(
        BaseAgent agent,
        string prompt,
        AgentThread? thread,
        CancellationToken cancellationToken = default) where T : class
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prompt, nameof(prompt));

        _logger.LogInformation("Running structured completion for type {Type}, prompt length: {Length}",
            typeof(T).Name, prompt.Length);

        try
        {
            var aiAgent = GetChatClientAgent<T>(agent);
            var response = await aiAgent.RunAsync<T>(message: prompt, thread: thread, cancellationToken: cancellationToken);

            return response.Result;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Structured completion cancelled by user");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Structured completion failed. Type: {Type}, Endpoint: {Endpoint}, Model: {Model}",
                typeof(T).Name,
                _options.Ollama.Endpoint,
                _options.Ollama.Model
            );

            throw new OllamaConnectionException(
                $"Falha ao obter resposta estruturada do tipo {typeof(T).Name}. Verifique se o Ollama está rodando.",
                ex
            )
            {
                Endpoint = _options.Ollama.Endpoint,
                Model = _options.Ollama.Model,
                RetryAttempts = 0
            };
        }
    }

    public async Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Running health check for Ollama at {Endpoint}", _options.Ollama.Endpoint);
            var agent = _chatClient.AsAIAgent();
            var response = await agent.RunAsync("Say 'OK' if you can read this.", cancellationToken: cancellationToken);
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
