namespace AutorLLM.Application.Commands.LLM.StartBrainstorm;

/// <summary>
/// Resultado do comando StartBrainstorm.
/// </summary>
public record StartBrainstormResult
{
    /// <summary>
    /// ID da sessão de brainstorm criada.
    /// </summary>
    public string SessionId { get; init; } = string.Empty;

    /// <summary>
    /// Resposta inicial da LLM após processar a ideia.
    /// </summary>
    public string InitialResponse { get; init; } = string.Empty;

    /// <summary>
    /// Indica se o brainstorm foi iniciado com sucesso.
    /// </summary>
    public bool Success { get; init; }
}
