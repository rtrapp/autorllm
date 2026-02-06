using AutorLLM.Application.AgentDefinitions;
using AutorLLM.Application.Commands.Brainstorm;
using AutorLLM.Application.Commands.LLM.StartBrainstorm;
using AutorLLM.Application.Services;
using MediatR;
using Microsoft.Agents.AI;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace AutorLLM.Api.Hubs;

/// <summary>
/// SignalR Hub para streaming de respostas LLM em tempo real.
/// </summary>
public class LLMHub : Hub
{
    private readonly IAgentService _agentService;
    private readonly AutorLLM.Application.AgentDefinitions.BrainstormAgentDefinition _brainstormAgent;
    private readonly IMediator _mediator;
    private readonly ILogger<LLMHub> _logger;
    
    // In-memory session storage using Agent Framework's AgentSession (serialized as JSON)
    // TODO: Move to distributed cache for production
    private static readonly Dictionary<string, string> _sessions = new();
    private static readonly object _lock = new();

    public LLMHub(
        IAgentService agentService,
        AutorLLM.Application.AgentDefinitions.BrainstormAgentDefinition brainstormAgent,
        IMediator mediator,
        ILogger<LLMHub> logger)
    {
        _agentService = agentService ?? throw new ArgumentNullException(nameof(agentService));
        _brainstormAgent = brainstormAgent ?? throw new ArgumentNullException(nameof(brainstormAgent));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Inicia sessão de brainstorm com streaming de resposta da LLM.
    /// </summary>
    /// <param name="sessionId">ID da sessão gerado pelo cliente</param>
    /// <param name="bookIdea">Descrição inicial da ideia do livro</param>
    public async Task StartBrainstorm(string sessionId, string bookIdea)
    {
        _logger.LogInformation("Starting brainstorm session {SessionId} with book idea: {BookIdea}", 
            sessionId, bookIdea.Substring(0, Math.Min(50, bookIdea.Length)));

        try
        {           
            // Stream response from LLM (creates new AgentSession internally)
            string? updatedSessionJson = null;

            await foreach (var (token, sessionJson) in _agentService.StreamCompletionAsync(
                _brainstormAgent,
                bookIdea,
                sessionJson: null, // New conversation
                Context.ConnectionAborted))
            {
                if (!string.IsNullOrEmpty(token))
                {
                    await Clients.Caller.SendAsync("OnBrainstormToken", token, Context.ConnectionAborted);
                }
                else if (sessionJson != null)
                {
                    // Last item contains updated session
                    updatedSessionJson = sessionJson;
                }
            }

            // Save session for future continuations
            if (updatedSessionJson != null)
            {
                lock (_lock)
                {
                    _sessions[sessionId] = updatedSessionJson;
                }
                _logger.LogInformation("Brainstorm session {SessionId} saved", sessionId);
            }

            await Clients.Caller.SendAsync("OnBrainstormComplete", Context.ConnectionAborted);
            
            _logger.LogInformation("Brainstorm session {SessionId} completed", sessionId);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Brainstorm request cancelled by client {SessionId}", sessionId);
            await Clients.Caller.SendAsync("OnCancelled", "Request was cancelled");
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("LLM service is unavailable"))
        {
            _logger.LogError(ex, "LLM service unavailable for brainstorm {SessionId}", sessionId);
            await Clients.Caller.SendAsync("OnError", "LLM não disponível. Verifique se o Ollama está rodando.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing brainstorm request {SessionId}", sessionId);
            await Clients.Caller.SendAsync("OnError", ex.Message);
        }
    }

    /// <summary>
    /// Envia resposta do autor e continua conversa de brainstorm.
    /// </summary>
    /// <param name="sessionId">ID da sessão de brainstorm</param>
    /// <param name="userResponse">Resposta do autor às perguntas da LLM</param>
    public async Task ContinueBrainstorm(string sessionId, string userResponse)
    {
        _logger.LogInformation("Continuing brainstorm session {SessionId}", sessionId);

        try
        {
            // Retrieve serialized session
            string? existingSessionJson;
            bool sessionFound;
            
            lock (_lock)
            {
                sessionFound = _sessions.TryGetValue(sessionId, out existingSessionJson);
            }
            
            if (!sessionFound)
            {
                _logger.LogWarning("Session {SessionId} not found", sessionId);
                await Clients.Caller.SendAsync("OnError", "Session not found. Please start a new brainstorm.");
                return;
            }

            // Stream response with existing session
            string? updatedSessionJson = null;

            await foreach (var (token, newSessionJson) in _agentService.StreamCompletionAsync(
                _brainstormAgent,
                userResponse,
                existingSessionJson,
                Context.ConnectionAborted))
            {
                if (!string.IsNullOrEmpty(token))
                {
                    await Clients.Caller.SendAsync("OnBrainstormToken", token, Context.ConnectionAborted);
                }
                else if (newSessionJson != null)
                {
                    // Last item contains updated session
                    updatedSessionJson = newSessionJson;
                }
            }

            // Update saved session
            if (updatedSessionJson != null)
            {
                lock (_lock)
                {
                    _sessions[sessionId] = updatedSessionJson;
                }
                _logger.LogInformation("Brainstorm session {SessionId} updated", sessionId);
            }

            await Clients.Caller.SendAsync("OnBrainstormComplete", Context.ConnectionAborted);
            
            _logger.LogInformation("Continue brainstorm session {SessionId} completed", sessionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error continuing brainstorm session {SessionId}", sessionId);
            await Clients.Caller.SendAsync("OnError", ex.Message);
        }
    }

    /// <summary>
    /// Recebe requisição para reescrita de texto usando LLM.
    /// </summary>
    /// <param name="chapterId">ID do capítulo sendo editado</param>
    /// <param name="selectedText">Texto selecionado pelo usuário</param>
    /// <param name="command">Comando/instrução para o LLM</param>
    public async Task RequestRewrite(string chapterId, string selectedText, string command)
    {
        _logger.LogInformation(
            "Received rewrite request for chapter {ChapterId}. Command: {Command}",
            chapterId,
            command
        );

        try
        {
            var prompt = BuildPrompt(selectedText, command);
            var simpleAgent = new SimpleAgentDefinition();

            await foreach (var (token, _) in _agentService.StreamCompletionAsync(simpleAgent, prompt, sessionJson: null, Context.ConnectionAborted))
            {
                if (!string.IsNullOrEmpty(token))
                {
                    await Clients.Caller.SendAsync("OnTokenReceived", token, Context.ConnectionAborted);
                }
            }

            await Clients.Caller.SendAsync("OnComplete", Context.ConnectionAborted);
            
            _logger.LogInformation("Rewrite request completed for chapter {ChapterId}", chapterId);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Rewrite request cancelled by client for chapter {ChapterId}", chapterId);
            await Clients.Caller.SendAsync("OnCancelled", "Request was cancelled");
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("LLM service is unavailable"))
        {
            _logger.LogError(ex, "LLM service unavailable for chapter {ChapterId}", chapterId);
            await Clients.Caller.SendAsync("OnError", "LLM não disponível. Verifique se o Ollama está rodando.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing rewrite request for chapter {ChapterId}", chapterId);
            await Clients.Caller.SendAsync("OnError", ex.Message);
        }
    }

    private static string BuildPrompt(string selectedText, string command)
    {
        return $"""
            Você é um assistente de escrita criativa.
            
            Texto selecionado:
            {selectedText}
            
            Instrução:
            {command}
            
            Reescreva o texto seguindo a instrução fornecida, mantendo coerência narrativa.
            """;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var sessionId = Context.ConnectionId;
        
        if (exception != null)
        {
            _logger.LogError(exception, "Client disconnected with error: {ConnectionId}", sessionId);
        }
        else
        {
            _logger.LogInformation("Client disconnected: {ConnectionId}", sessionId);
        }
        
        // Cleanup session for this connection
        lock (_lock)
        {
            if (_sessions.Remove(sessionId))
            {
                _logger.LogInformation("Cleared session for {SessionId}", sessionId);
            }
        }
        
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Gera outline estruturado baseado no contexto acumulado do brainstorm.
    /// Usa Microsoft Agent Framework via CQRS.
    /// </summary>
    /// <param name="contextJson">JSON com BrainstormContext completo</param>
    public async Task GenerateOutline(string contextJson)
    {
        _logger.LogInformation("Generating outline from context (length: {Length})", contextJson.Length);

        try
        {
            // Deserializar contexto do frontend
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var context = JsonSerializer.Deserialize<BrainstormContextDto>(contextJson, options);

            if (context == null)
            {
                throw new InvalidOperationException("Failed to deserialize context JSON");
            }

            // Criar command CQRS
            var command = new GenerateOutlineCommand
            {
                SessionId = Context.ConnectionId,
                BookIdea = context.BookIdea,
                Title = context.Title,
                Author = context.Author,
                Genre = context.Genre,
                Synopsis = context.Synopsis,
                Tone = context.Tone,
                TargetAudience = context.TargetAudience,
                Characters = context.Characters?.Select(c => new CharacterSuggestion
                {
                    Name = c.Name,
                    Description = c.Description,
                    Role = c.Role,
                    Backstory = c.Backstory,
                    Appearance = c.Appearance,
                    Personality = c.Personality
                }).ToList(),
                Locations = context.Locations?.Select(l => new LocationSuggestion
                {
                    Name = l.Name,
                    Description = l.Description,
                    Geography = l.Geography,
                    Culture = l.Culture,
                    Significance = l.Significance
                }).ToList(),
                Plots = context.Plots?.Select(p => new PlotSuggestion
                {
                    Title = p.Title,
                    Description = p.Description,
                    Type = p.Type,
                    Resolution = p.Resolution
                }).ToList(),
                Chapters = context.Chapters?.Select(ch => new ChapterSuggestion
                {
                    Title = ch.Title,
                    Summary = ch.Summary,
                    Order = ch.Order
                }).ToList()
            };

            _logger.LogInformation("Dispatching GenerateOutlineCommand via MediatR...");

            // Executar command via MediatR (usa Agent Framework internamente)
            var result = await _mediator.Send(command, Context.ConnectionAborted);

            if (!result.IsValid)
            {
                _logger.LogWarning("Outline validation failed: {Errors}", string.Join(", ", result.ValidationErrors));
                await Clients.Caller.SendAsync("OnError", $"Validation errors: {string.Join("; ", result.ValidationErrors)}");
                return;
            }

            // Serializar outline gerado para JSON
            var outlineJson = JsonSerializer.Serialize(result.Outline, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            _logger.LogInformation("Outline generated successfully. Sending to client...");

            // Enviar outline completo de uma vez
            await Clients.Caller.SendAsync("OnOutlineGenerated", outlineJson);

            _logger.LogInformation("Outline generation completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating outline");
            await Clients.Caller.SendAsync("OnError", $"Failed to generate outline: {ex.Message}");
        }
    }
}

/// <summary>
/// DTO para receber contexto do frontend (espelho do BrainstormContext do TypeScript).
/// </summary>
public class BrainstormContextDto
{
    public string BookIdea { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Synopsis { get; set; }
    public string? Genre { get; set; }
    public int? TargetWordCount { get; set; }
    public string? Tone { get; set; }
    public string? TargetAudience { get; set; }
    public List<CharacterDto>? Characters { get; set; }
    public List<LocationDto>? Locations { get; set; }
    public List<PlotDto>? Plots { get; set; }
    public List<ChapterDto>? Chapters { get; set; }
}

public class CharacterDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Role { get; set; }
    public string? Backstory { get; set; }
    public string? Appearance { get; set; }
    public string? Personality { get; set; }
}

public class LocationDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Geography { get; set; }
    public string? Culture { get; set; }
    public string? Significance { get; set; }
}

public class PlotDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Type { get; set; }
    public string? Resolution { get; set; }
}

public class ChapterDto
{
    public string Title { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public int Order { get; set; }
}
