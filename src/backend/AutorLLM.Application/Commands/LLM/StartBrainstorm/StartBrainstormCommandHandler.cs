using AutorLLM.Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutorLLM.Application.Commands.LLM.StartBrainstorm;

public class StartBrainstormCommandHandler
    : IRequestHandler<StartBrainstormCommand, StartBrainstormResult>
{
    private readonly IAgentService _agentService;
    private readonly ILogger<StartBrainstormCommandHandler> _logger;

    public StartBrainstormCommandHandler(
        IAgentService agentService,
        ILogger<StartBrainstormCommandHandler> logger)
    {
        _agentService = agentService;
        _logger = logger;
    }

    public async Task<StartBrainstormResult> Handle(
        StartBrainstormCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting brainstorm session for book idea");

        try
        {
            var prompt = BuildBrainstormPrompt(command.BookIdea);
            
            var response = await _agentService.CompleteAsync(prompt, cancellationToken);

            var sessionId = Guid.NewGuid().ToString();

            _logger.LogInformation("Brainstorm session {SessionId} started successfully", sessionId);

            return new StartBrainstormResult
            {
                SessionId = sessionId,
                InitialResponse = response,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start brainstorm session");
            throw;
        }
    }

    private static string BuildBrainstormPrompt(string bookIdea)
    {
        return $"""
            Você é um assistente especializado em ajudar autores a desenvolverem suas ideias de livros.

            O autor descreveu a seguinte ideia:
            {bookIdea}

            Analise a ideia e responda de forma amigável e encorajadora. Confirme que você entendeu a essência da história e faça 3-5 perguntas focadas para ajudar a expandir e clarificar os seguintes aspectos:

            1. **Gênero e Tom**: Qual é o gênero da história? Que tom/atmosfera você imagina?
            2. **Protagonista**: Quem é o personagem principal? Quais são suas motivações e conflitos internos?
            3. **Conflito Central**: Qual é o principal obstáculo ou desafio que a história aborda?
            4. **Ambientação**: Onde e quando a história se passa?
            5. **Tema**: Que mensagem ou reflexão você quer transmitir aos leitores?

            Seja específico mas não excessivamente técnico. O objetivo é ajudar o autor a clarificar sua visão antes de criar o outline estruturado.
            """;
    }
}
