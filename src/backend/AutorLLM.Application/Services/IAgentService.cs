using Microsoft.Agents.AI;

namespace AutorLLM.Application.Services;

/// <summary>
/// Interface para interação com LLM via Microsoft Agent Framework.
/// </summary>
public interface IAgentService
{
    /// <summary>
    /// Gera completion de texto com streaming de tokens.
    /// </summary>
    /// <param name="prompt">Prompt para o LLM.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Stream assíncrono de tokens de texto.</returns>
    IAsyncEnumerable<string> StreamCompletionAsync(
        BaseAgent agent,
        string prompt,
        AgentThread? thread,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gera completion de texto completo (sem streaming).
    /// </summary>
    /// <param name="prompt">Prompt para o LLM.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Texto completo gerado pelo LLM.</returns>
    Task<string> CompleteAsync(
        BaseAgent agent,
        string prompt,
        AgentThread? thread,
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
        BaseAgent agent,
        string prompt,
        AgentThread? thread,
        CancellationToken cancellationToken = default
    ) where T : class;

    /// <summary>
    /// Verifica se o LLM está acessível e funcionando.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>True se o LLM responder corretamente, false caso contrário.</returns>
    Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default);
}

