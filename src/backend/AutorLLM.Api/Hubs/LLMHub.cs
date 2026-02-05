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
    /// Inicia sessão de brainstorm com streaming de resposta da LLM.
    /// </summary>
    /// <param name="bookIdea">Descrição inicial da ideia do livro</param>
    public async Task StartBrainstorm(string bookIdea)
    {
        _logger.LogInformation("Received brainstorm request with book idea: {BookIdea}", bookIdea.Substring(0, Math.Min(50, bookIdea.Length)));

        try
        {
            var prompt = BuildBrainstormPrompt(bookIdea);
            
            // LOG: Mostrar o prompt completo sendo enviado
            _logger.LogInformation("=== PROMPT BEING SENT TO LLM ===");
            _logger.LogInformation(prompt);
            _logger.LogInformation("=== END OF PROMPT ===");

            await foreach (var token in _agentService.StreamCompletionAsync(prompt, Context.ConnectionAborted))
            {
                await Clients.Caller.SendAsync("OnBrainstormToken", token, Context.ConnectionAborted);
            }

            await Clients.Caller.SendAsync("OnBrainstormComplete", Context.ConnectionAborted);
            
            _logger.LogInformation("Brainstorm session completed");
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Brainstorm request cancelled by client");
            await Clients.Caller.SendAsync("OnCancelled", "Request was cancelled");
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("LLM service is unavailable"))
        {
            _logger.LogError(ex, "LLM service unavailable for brainstorm");
            await Clients.Caller.SendAsync("OnError", "LLM não disponível. Verifique se o Ollama está rodando.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing brainstorm request");
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
            var prompt = BuildContinuePrompt(userResponse);

            await foreach (var token in _agentService.StreamCompletionAsync(prompt, Context.ConnectionAborted))
            {
                await Clients.Caller.SendAsync("OnBrainstormToken", token, Context.ConnectionAborted);
            }

            await Clients.Caller.SendAsync("OnBrainstormComplete", Context.ConnectionAborted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error continuing brainstorm session {SessionId}", sessionId);
            await Clients.Caller.SendAsync("OnError", ex.Message);
        }
    }

    private static string BuildBrainstormPrompt(string bookIdea)
    {
        return $"""
            SYSTEM: Você DEVE responder APENAS no formato especificado abaixo. Qualquer desvio será considerado erro.

            TAREFA: O autor descreveu esta ideia de livro:
            "{bookIdea}"

            FORMATO OBRIGATÓRIO DA RESPOSTA:

            [1-2 frases de encorajamento]

            (Gênero e Tom) [pergunta sobre gênero]
            (Protagonista) [pergunta sobre personagem principal]
            (Conflito Central) [pergunta sobre obstáculo]
            (Ambientação) [pergunta sobre mundo/época]
            (Tema) [pergunta sobre mensagem]

            EXEMPLO CORRETO:
            Que premissa intrigante! Mistério e vampiros sempre rendem ótimas histórias.

            (Gênero e Tom) Sua história terá um tom noir sombrio ou será mais uma aventura ágil?
            (Protagonista) Por que uma vampira se tornou detective e o que ela busca?
            (Conflito Central) Quem está por trás dessas máquinas assassinas?
            (Ambientação) Como é essa cidade steampunk e quando ela existe?
            (Tema) Que reflexão sobre tecnologia e humanidade você quer explorar?

            IMPORTANTE:
            - Use EXATAMENTE os 5 nomes de categoria mostrados
            - Cada categoria entre parênteses no início da linha
            - Uma pergunta por linha
            - NENHUM texto adicional após as 5 perguntas
            - NÃO use marcadores como *, -, números, bullets

            RESPONDA AGORA:
            """;
    }

    private static string BuildContinuePrompt(string userResponse)
    {
        return $"""
            O autor respondeu:
            {userResponse}

            Baseado nessa resposta, faça perguntas adicionais se necessário, ou confirme que você tem informações suficientes para gerar um outline estruturado do livro.
            """;
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

