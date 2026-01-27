using AutorLLM.Application.Services;
using Microsoft.AspNetCore.SignalR;

namespace AutorLLM.Api.Hubs;

/// <summary>
/// SignalR Hub para streaming de respostas LLM em tempo real.
/// </summary>
public class LLMHub : Hub
{
    private readonly IAgentService _agentService;
    private readonly ILogger<LLMHub> _logger;

    public LLMHub(IAgentService agentService, ILogger<LLMHub> logger)
    {
        _agentService = agentService ?? throw new ArgumentNullException(nameof(agentService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

            await foreach (var token in _agentService.StreamCompletionAsync(prompt, Context.ConnectionAborted))
            {
                await Clients.Caller.SendAsync("OnTokenReceived", token, Context.ConnectionAborted);
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
        if (exception != null)
        {
            _logger.LogError(exception, "Client disconnected with error: {ConnectionId}", Context.ConnectionId);
        }
        else
        {
            _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        }
        
        await base.OnDisconnectedAsync(exception);
    }
}

