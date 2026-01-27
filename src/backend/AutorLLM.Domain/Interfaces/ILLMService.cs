namespace AutorLLM.Domain.Interfaces;

/// <summary>
/// Interface para serviços de LLM (Large Language Model).
/// </summary>
/// <remarks>
/// TODO: Esta interface será substituída nas US072-075 para usar:
/// - Microsoft.Extensions.AI (v10.0)
/// - Semantic Kernel (v1.x) para RAG + Context Management
/// - Microsoft Agents Framework para orquestração
/// - Integração real com Ollama (http://localhost:11434)
/// </remarks>
public interface ILLMService
{
    /// <summary>
    /// Gera resposta do LLM de forma streaming (token por token).
    /// </summary>
    /// <param name="prompt">Prompt a ser enviado ao LLM</param>
    /// <param name="onToken">Callback chamado para cada token recebido</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Task assíncrona</returns>
    Task StreamResponseAsync(
        string prompt,
        Func<string, Task> onToken,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Verifica se o serviço LLM está disponível e funcionando.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>True se o serviço está saudável, false caso contrário</returns>
    Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default);
}
