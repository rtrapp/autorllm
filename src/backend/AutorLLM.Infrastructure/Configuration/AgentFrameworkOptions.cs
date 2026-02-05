namespace AutorLLM.Infrastructure.Configuration;

/// <summary>
/// Configurações do Microsoft Agent Framework e integração com provedores LLM.
/// </summary>
public class AgentFrameworkOptions
{
    public const string SectionName = "AgentFramework";

    /// <summary>
    /// Provedor LLM ativo: "Ollama" ou "LMStudio".
    /// </summary>
    public string ActiveProvider { get; set; } = "LMStudio";

    public OllamaOptions Ollama { get; set; } = new();
    public LMStudioOptions LMStudio { get; set; } = new();
    public ResilienceOptions Resilience { get; set; } = new();
}

/// <summary>
/// Configurações específicas do provedor Ollama.
/// </summary>
public class OllamaOptions
{
    /// <summary>
    /// Endpoint HTTP do Ollama (ex: http://localhost:11434).
    /// </summary>
    public string Endpoint { get; set; } = "http://localhost:11434";

    /// <summary>
    /// Nome do modelo a ser utilizado (ex: gpt-oss:20b, llama3.1:70b, qwen2.5:32b).
    /// </summary>
    public string Model { get; set; } = "gpt-oss:20b";

    /// <summary>
    /// Timeout em segundos para requisições ao Ollama.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 60;
}

/// <summary>
/// Configurações específicas do provedor LM Studio.
/// </summary>
public class LMStudioOptions
{
    /// <summary>
    /// Endpoint HTTP do LM Studio (ex: http://172.23.16.1:1234).
    /// </summary>
    public string Endpoint { get; set; } = "http://172.23.16.1:1234";

    /// <summary>
    /// Nome do modelo a ser utilizado (ex: openai/gpt-oss-20b).
    /// </summary>
    public string Model { get; set; } = "openai/gpt-oss-20b";

    /// <summary>
    /// Timeout em segundos para requisições ao LM Studio.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 60;
}

/// <summary>
/// Configurações de resiliência (retry, circuit breaker, timeout).
/// </summary>
public class ResilienceOptions
{
    /// <summary>
    /// Número máximo de tentativas de retry.
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Delay inicial para backoff exponencial (em segundos).
    /// </summary>
    public int InitialBackoffSeconds { get; set; } = 2;

    /// <summary>
    /// Número de falhas consecutivas antes de abrir o circuit breaker.
    /// </summary>
    public int CircuitBreakerFailureThreshold { get; set; } = 5;

    /// <summary>
    /// Duração em segundos que o circuit breaker permanece aberto.
    /// </summary>
    public int CircuitBreakerDurationSeconds { get; set; } = 30;
}
