using MediatR;

namespace AutorLLM.Application.Commands.LLM.StartBrainstorm;

/// <summary>
/// Command para iniciar brainstorm com LLM para criação de outline.
/// </summary>
public record StartBrainstormCommand : IRequest<StartBrainstormResult>
{
    /// <summary>
    /// Descrição inicial da ideia do livro fornecida pelo autor.
    /// </summary>
    public string BookIdea { get; init; } = string.Empty;
}
