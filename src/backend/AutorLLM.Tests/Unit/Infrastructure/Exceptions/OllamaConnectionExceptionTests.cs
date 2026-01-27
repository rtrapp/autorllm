using AutorLLM.Infrastructure.Exceptions;
using FluentAssertions;
using Xunit;

namespace AutorLLM.Tests.Unit.Infrastructure.Exceptions;

public class OllamaConnectionExceptionTests
{
    [Fact]
    public void Constructor_WithoutParameters_ShouldSetDefaultMessage()
    {
        // Act
        var exception = new OllamaConnectionException();

        // Assert
        exception.Message.Should().Be("LLM não disponível. Verifique se o Ollama está rodando.");
    }

    [Fact]
    public void Constructor_WithMessage_ShouldSetCustomMessage()
    {
        // Arrange
        var customMessage = "Custom error message";

        // Act
        var exception = new OllamaConnectionException(customMessage);

        // Assert
        exception.Message.Should().Be(customMessage);
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_ShouldSetBoth()
    {
        // Arrange
        var customMessage = "Custom error message";
        var innerException = new InvalidOperationException("Inner error");

        // Act
        var exception = new OllamaConnectionException(customMessage, innerException);

        // Assert
        exception.Message.Should().Be(customMessage);
        exception.InnerException.Should().Be(innerException);
    }

    [Fact]
    public void Properties_ShouldBeSettableViaInitializer()
    {
        // Arrange
        var endpoint = "http://localhost:11434";
        var model = "gpt-oss:20b";
        var retryAttempts = 3;

        // Act
        var exception = new OllamaConnectionException("Error")
        {
            Endpoint = endpoint,
            Model = model,
            RetryAttempts = retryAttempts
        };

        // Assert
        exception.Endpoint.Should().Be(endpoint);
        exception.Model.Should().Be(model);
        exception.RetryAttempts.Should().Be(retryAttempts);
    }

    [Fact]
    public void Properties_ShouldBeNullableAndDefault()
    {
        // Act
        var exception = new OllamaConnectionException();

        // Assert
        exception.Endpoint.Should().BeNull();
        exception.Model.Should().BeNull();
        exception.RetryAttempts.Should().Be(0);
    }
}
