using AutorLLM.Infrastructure.Configuration;
using AutorLLM.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace AutorLLM.Tests.Unit.Infrastructure.Services;

/// <summary>
/// Testes unitários para AgentService - US073
/// Verifica streaming de respostas LLM via IAsyncEnumerable
/// 
/// NOTA: Testes de integração com Microsoft.Agents.AI são realizados
/// separadamente devido à complexidade de mockar o framework.
/// Estes testes focam em validações de entrada e estrutura da API.
/// </summary>
public class AgentServiceTests
{
    private readonly Mock<ILogger<AgentService>> _loggerMock;
    private readonly IOptions<AgentFrameworkOptions> _options;

    public AgentServiceTests()
    {
        _loggerMock = new Mock<ILogger<AgentService>>();
        _options = Options.Create(new AgentFrameworkOptions
        {
            Ollama = new OllamaOptions
            {
                Endpoint = "http://localhost:11434",
                Model = "test-model",
                TimeoutSeconds = 60
            }
        });
    }

    [Fact]
    public void Constructor_WithNullAgent_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () => new AgentService(null!, _options, _loggerMock.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>("agent é obrigatório");
    }

    [Fact]
    public void Constructor_WithNullOptions_ShouldThrowArgumentNullException()
    {
        // Arrange
        var mockChatClient = new Mock<Microsoft.Extensions.AI.IChatClient>();

        // Act
        var act = () => new AgentService(mockChatClient.Object, null!, _loggerMock.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>("options é obrigatório");
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange
        var mockChatClient = new Mock<Microsoft.Extensions.AI.IChatClient>();

        // Act
        var act = () => new AgentService(mockChatClient.Object, _options, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>("logger é obrigatório");
    }

    [Fact]
    public async Task StreamCompletionAsync_WithNullPrompt_ShouldThrowArgumentException()
    {
        // Arrange
        var mockChatClient = new Mock<Microsoft.Extensions.AI.IChatClient>();
        var sut = new AgentService(mockChatClient.Object, _options, _loggerMock.Object);
        var agent = new AutorLLM.Application.AgentDefinitions.SimpleAgentDefinition();
        string? prompt = null;

        // Act
        var act = async () =>
        {
            await foreach (var token in sut.StreamCompletionAsync(agent, prompt!))
            {
                // Should not reach here
            }
        };

        // Assert
        await act.Should().ThrowAsync<ArgumentException>("null prompt é inválido");
    }

    [Fact]
    public async Task StreamCompletionAsync_WithEmptyPrompt_ShouldThrowArgumentException()
    {
        // Arrange
        var mockChatClient = new Mock<Microsoft.Extensions.AI.IChatClient>();
        var sut = new AgentService(mockChatClient.Object, _options, _loggerMock.Object);
        var agent = new AutorLLM.Application.AgentDefinitions.SimpleAgentDefinition();
        var prompt = "";

        // Act
        var act = async () =>
        {
            await foreach (var token in sut.StreamCompletionAsync(agent, prompt))
            {
                // Should not reach here
            }
        };

        // Assert
        await act.Should().ThrowAsync<ArgumentException>("empty prompt é inválido");
    }

    [Fact]
    public async Task StreamCompletionAsync_WithWhitespacePrompt_ShouldThrowArgumentException()
    {
        // Arrange
        var mockChatClient = new Mock<Microsoft.Extensions.AI.IChatClient>();
        var sut = new AgentService(mockChatClient.Object, _options, _loggerMock.Object);
        var agent = new AutorLLM.Application.AgentDefinitions.SimpleAgentDefinition();
        var prompt = "   ";

        // Act
        var act = async () =>
        {
            await foreach (var token in sut.StreamCompletionAsync(agent, prompt))
            {
                // Should not reach here
            }
        };

        // Assert
        await act.Should().ThrowAsync<ArgumentException>("whitespace prompt é inválido");
    }

    [Fact]
    public async Task CompleteAsync_WithNullPrompt_ShouldThrowArgumentException()
    {
        // Arrange
        var mockChatClient = new Mock<Microsoft.Extensions.AI.IChatClient>();
        var sut = new AgentService(mockChatClient.Object, _options, _loggerMock.Object);
        var agent = new AutorLLM.Application.AgentDefinitions.SimpleAgentDefinition();
        string? prompt = null;

        // Act
        var act = () => sut.CompleteAsync(agent, prompt!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>("null prompt é inválido");
    }

    [Fact]
    public async Task CompleteAsync_WithEmptyPrompt_ShouldThrowArgumentException()
    {
        // Arrange
        var mockChatClient = new Mock<Microsoft.Extensions.AI.IChatClient>();
        var sut = new AgentService(mockChatClient.Object, _options, _loggerMock.Object);
        var agent = new AutorLLM.Application.AgentDefinitions.SimpleAgentDefinition();
        var prompt = "";

        // Act
        var act = () => sut.CompleteAsync(agent, prompt);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>("empty prompt é inválido");
    }

    [Fact]
    public void AgentFrameworkOptions_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var options = new OllamaOptions();

        // Assert
        options.Endpoint.Should().Be("http://localhost:11434", "default endpoint está configurado");
        options.Model.Should().Be("gpt-oss:20b", "default model está configurado");
        options.TimeoutSeconds.Should().Be(60, "default timeout está configurado");
    }
}

