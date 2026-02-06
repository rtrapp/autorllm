using AutorLLM.Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutorLLM.Application.Commands.LLM.StartBrainstorm;

public class StartBrainstormCommandHandler
    : IRequestHandler<StartBrainstormCommand, StartBrainstormResult>
{
    private readonly IAgentService _agentService;
    private readonly AutorLLM.Application.AgentDefinitions.BrainstormAgentDefinition _brainstormAgent;
    private readonly ILogger<StartBrainstormCommandHandler> _logger;

    public StartBrainstormCommandHandler(
        IAgentService agentService,
        AutorLLM.Application.AgentDefinitions.BrainstormAgentDefinition brainstormAgent,
        ILogger<StartBrainstormCommandHandler> logger)
    {
        _agentService = agentService;
        _brainstormAgent = brainstormAgent;
        _logger = logger;
    }

    public async Task<StartBrainstormResult> Handle(
        StartBrainstormCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting brainstorm session for book idea");

        try
        {
            // No need for custom prompt - agent has instructions in BrainstormAgentDefinition
            var (response, sessionJson) = await _agentService.CompleteAsync(
                _brainstormAgent, 
                command.BookIdea, 
                sessionJson: null, // First message, no history yet
                cancellationToken);

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
}
