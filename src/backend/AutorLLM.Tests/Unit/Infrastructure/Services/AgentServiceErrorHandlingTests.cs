using AutorLLM.Application.Services;
using AutorLLM.Infrastructure.Configuration;
using AutorLLM.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Agents.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace AutorLLM.Tests.Unit.Infrastructure.Services;

/// <summary>
/// Testes focados em error handling do AgentService.
/// Nota: AIAgent não pode ser mockado facilmente pois não é virtual.
/// Testes de integração devem ser usados para validar comportamento com Ollama real.
/// </summary>
public class AgentServiceErrorHandlingTests
{
    private readonly Mock<ILogger<AgentService>> _mockLogger;
    private readonly IOptions<AgentFrameworkOptions> _options;

    public AgentServiceErrorHandlingTests()
    {
        _mockLogger = new Mock<ILogger<AgentService>>();
        _options = Options.Create(new AgentFrameworkOptions
        {
            Ollama = new OllamaOptions
            {
                Endpoint = "http://localhost:11434",
                Model = "gpt-oss:20b",
                TimeoutSeconds = 60
            },
            Resilience = new ResilienceOptions
            {
                MaxRetryAttempts = 3,
                InitialBackoffSeconds = 2,
                CircuitBreakerFailureThreshold = 5,
                CircuitBreakerDurationSeconds = 30
            }
        });
    }

    [Fact]
    public void CompleteAsync_WithNullPrompt_ShouldThrowArgumentException()
    {
        // Arrange
        var mockAgent = new Mock<AIAgent>();
        var service = new AgentService(mockAgent.Object, _options, _mockLogger.Object);

        // Act
        var act = () => service.CompleteAsync(null!);

        // Assert
        act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("prompt");
    }

    [Fact]
    public void CompleteAsync_WithEmptyPrompt_ShouldThrowArgumentException()
    {
        // Arrange
        var mockAgent = new Mock<AIAgent>();
        var service = new AgentService(mockAgent.Object, _options, _mockLogger.Object);

        // Act
        var act = () => service.CompleteAsync("");

        // Assert
        act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("prompt");
    }

    [Fact]
    public async Task StreamCompletionAsync_WithNullPrompt_ShouldThrowArgumentException()
    {
        // Arrange
        var mockAgent = new Mock<AIAgent>();
        var service = new AgentService(mockAgent.Object, _options, _mockLogger.Object);

        // Act & Assert - ArgumentNullException is a subtype of ArgumentException
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await foreach (var _ in service.StreamCompletionAsync(null!))
            {
                // Should not reach here
            }
        });
    }

    [Fact]
    public async Task StreamCompletionAsync_WithEmptyPrompt_ShouldThrowArgumentException()
    {
        // Arrange
        var mockAgent = new Mock<AIAgent>();
        var service = new AgentService(mockAgent.Object, _options, _mockLogger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await foreach (var _ in service.StreamCompletionAsync(""))
            {
                // Should not reach here
            }
        });
    }
}
