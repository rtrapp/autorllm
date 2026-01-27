namespace AutorLLM.Infrastructure.Exceptions;

/// <summary>
/// Exception lançada quando há falha de comunicação com o Ollama.
/// </summary>
public class OllamaConnectionException : Exception
{
    public OllamaConnectionException()
        : base("LLM não disponível. Verifique se o Ollama está rodando.")
    {
    }

    public OllamaConnectionException(string message)
        : base(message)
    {
    }

    public OllamaConnectionException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Endpoint do Ollama que falhou.
    /// </summary>
    public string? Endpoint { get; init; }

    /// <summary>
    /// Modelo que estava sendo usado.
    /// </summary>
    public string? Model { get; init; }

    /// <summary>
    /// Número de tentativas antes da falha.
    /// </summary>
    public int RetryAttempts { get; init; }
}
