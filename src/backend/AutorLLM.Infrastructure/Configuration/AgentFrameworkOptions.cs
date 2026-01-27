namespace AutorLLM.Infrastructure.Configuration;

/// <summary>
/// Configurações do Microsoft Agent Framework e integração com Ollama.
/// </summary>
public class AgentFrameworkOptions
{
    public const string SectionName = "AgentFramework";

    public OllamaOptions Ollama { get; set; } = new();
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
