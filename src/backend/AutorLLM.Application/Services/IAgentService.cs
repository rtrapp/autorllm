using AutorLLM.Application.AgentDefinitions;

namespace AutorLLM.Application.Services;

/// <summary>
/// Interface para interação com LLM via Microsoft Agent Framework.
/// </summary>
public interface IAgentService
{
    /// <summary>
    /// Gera completion de texto com streaming de tokens.
    /// </summary>
    /// <param name="agent">Definição do agente com instruções.</param>
    /// <param name="prompt">Mensagem do usuário (nova entrada).</param>
    /// <param name="sessionJson">Sessão serializada (JSON) para continuar conversa. Null para nova sessão.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Stream assíncrono de tokens de texto e sessão serializada atualizada.</returns>
    IAsyncEnumerable<(string token, string? sessionJson)> StreamCompletionAsync(
        BaseAgentDefinition agent,
        string prompt,
        string? sessionJson = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gera completion de texto completo (sem streaming).
    /// </summary>
    /// <param name="agent">Definição do agente com instruções.</param>
    /// <param name="prompt">Mensagem do usuário (nova entrada).</param>
    /// <param name="sessionJson">Sessão serializada (JSON) para continuar conversa. Null para nova sessão.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Resposta completa e sessão serializada atualizada.</returns>
    Task<(string response, string sessionJson)> CompleteAsync(
        BaseAgentDefinition agent,
        string prompt,
        string? sessionJson = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gera completion estruturado usando JSON schema.
    /// O LLM retornará um objeto do tipo T automaticamente serializado.
    /// </summary>
    /// <typeparam name="T">Tipo do objeto esperado na resposta.</typeparam>
    /// <param name="prompt">Prompt para o LLM.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Objeto deserializado do tipo T.</returns>
    Task<T> CompleteStructuredAsync<T>(
        BaseAgentDefinition agent,
        string prompt,
        CancellationToken cancellationToken = default
    ) where T : class;

    /// <summary>
    /// Verifica se o LLM está acessível e funcionando.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>True se o LLM responder corretamente, false caso contrário.</returns>
    Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default);
}

