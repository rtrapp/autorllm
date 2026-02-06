using AutorLLM.Application.AgentDefinitions;
using AutorLLM.Application.Services;
using AutorLLM.Infrastructure.Configuration;
using AutorLLM.Infrastructure.Exceptions;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using System.Runtime.CompilerServices;
using System.Text.Json;

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

    private AIAgent GetChatClientAgent(BaseAgentDefinition agent)
    {
        return _chatClient.AsAIAgent(agent.Name, agent.Instructions);
    }

    private AIAgent GetChatClientAgent<T>(BaseAgentDefinition agent)
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

    public async IAsyncEnumerable<(string token, string? sessionJson)> StreamCompletionAsync(
        BaseAgentDefinition agent,
        string prompt,
        string? sessionJson = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prompt, nameof(prompt));

        _logger.LogInformation("Streaming completion for prompt length: {Length}, Has session: {HasSession}", 
            prompt.Length, sessionJson != null);

        await foreach (var result in StreamInternalAsync(agent, prompt, sessionJson, cancellationToken))
        {
            yield return result;
        }
    }

    private async IAsyncEnumerable<(string token, string? sessionJson)> StreamInternalAsync(
        BaseAgentDefinition agent,
        string prompt,
        string? sessionJson,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var tokenCount = 0;
        
        // DEBUG: Log instructions being used
        _logger.LogInformation("Agent: {AgentName}, Instructions (first 300 chars): {Instructions}", 
            agent.Name, 
            agent.Instructions.Length > 300 ? agent.Instructions.Substring(0, 300) + "..." : agent.Instructions);
        
        var aiAgent = GetChatClientAgent(agent);

        // Deserialize session if provided, otherwise create new
        AgentSession session;
        if (!string.IsNullOrEmpty(sessionJson))
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(sessionJson);
            session = await aiAgent.DeserializeSessionAsync(jsonElement);
            _logger.LogInformation("Resumed session with history");
        }
        else
        {
            session = await aiAgent.GetNewSessionAsync(cancellationToken);
            prompt = $@"
                {agent.Instructions}

                {prompt}
            ";
            _logger.LogInformation("Created new session");
        }

        await foreach (var update in aiAgent.RunStreamingAsync(prompt, session, cancellationToken: cancellationToken))
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                tokenCount++;
                yield return (update.Text, null);
            }
        }

        // Serialize session after completion
        var serializedSession = session.Serialize();
        var updatedSessionJson = System.Text.Json.JsonSerializer.Serialize(serializedSession);
        
        _logger.LogInformation("Streaming completion finished. Tokens streamed: {TokenCount}", tokenCount);
        
        // Yield final result with updated session
        yield return (string.Empty, updatedSessionJson);
    }

    public async Task<(string response, string sessionJson)> CompleteAsync(
        BaseAgentDefinition agent,
        string prompt,
        string? sessionJson = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prompt, nameof(prompt));

        _logger.LogInformation("Running completion for prompt length: {Length}, Has session: {HasSession}", 
            prompt.Length, sessionJson != null);

        try
        {
            var aiAgent = GetChatClientAgent(agent);
            
            // Deserialize session if provided, otherwise create new
            AgentSession session;
            if (!string.IsNullOrEmpty(sessionJson))
            {

                var jsonElement = JsonSerializer.Deserialize<JsonElement>(sessionJson);
                session = await aiAgent.DeserializeSessionAsync(jsonElement);
                _logger.LogInformation("Resumed session with history");
            }
            else
            {
                session = await aiAgent.GetNewSessionAsync(cancellationToken);
                _logger.LogInformation("Created new session");
            }
            
            var response = await aiAgent.RunAsync(prompt, session, cancellationToken: cancellationToken);
            var result = response.Text ?? string.Empty;

            // Serialize session after completion
            var serializedSession = session.Serialize();
            var updatedSessionJson = System.Text.Json.JsonSerializer.Serialize(serializedSession);

            _logger.LogInformation("Completion finished. Result length: {Length}", result.Length);

            return (result, updatedSessionJson);
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
        BaseAgentDefinition agent,
        string prompt,
        CancellationToken cancellationToken = default) where T : class
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prompt, nameof(prompt));

        _logger.LogInformation("Running structured completion for type {Type}, prompt length: {Length}",
            typeof(T).Name, prompt.Length);

        try
        {
            var aiAgent = GetChatClientAgent<T>(agent);
            
            // Get new session for structured response
            var session = await aiAgent.GetNewSessionAsync(cancellationToken);
            
            var response = await aiAgent.RunAsync(prompt, session, cancellationToken: cancellationToken);
            
            // Try to deserialize from response.Text if it's JSON
            if (!string.IsNullOrEmpty(response.Text))
            {
                return JsonSerializer.Deserialize<T>(response.Text) 
                    ?? throw new InvalidOperationException($"Failed to deserialize response to {typeof(T).Name}");
            }
            
            throw new InvalidOperationException("LLM returned empty response");
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
