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
        string prompt,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gera completion de texto completo (sem streaming).
    /// </summary>
    /// <param name="prompt">Prompt para o LLM.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Texto completo gerado pelo LLM.</returns>
    Task<string> CompleteAsync(
        string prompt,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Verifica se o LLM está acessível e funcionando.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>True se o LLM responder corretamente, false caso contrário.</returns>
    Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default);
}
