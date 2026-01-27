using AutorLLM.Api.Hubs;
using AutorLLM.Application.Services;
using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;

namespace AutorLLM.Tests.Unit.Api;

public class LLMHubTests
{
    private readonly Mock<IAgentService> _agentServiceMock;
    private readonly Mock<ILogger<LLMHub>> _loggerMock;
    private readonly Mock<IHubCallerClients> _clientsMock;
    private readonly Mock<ISingleClientProxy> _callerMock;
    private readonly Mock<HubCallerContext> _contextMock;
    private readonly LLMHub _hub;

    public LLMHubTests()
    {
        _agentServiceMock = new Mock<IAgentService>();
        _loggerMock = new Mock<ILogger<LLMHub>>();
        _clientsMock = new Mock<IHubCallerClients>();
        _callerMock = new Mock<ISingleClientProxy>();
        _contextMock = new Mock<HubCallerContext>();

        _clientsMock.Setup(c => c.Caller).Returns(_callerMock.Object);

        _hub = new LLMHub(_agentServiceMock.Object, _loggerMock.Object)
        {
            Clients = _clientsMock.Object,
            Context = _contextMock.Object
        };

        _contextMock.Setup(c => c.ConnectionAborted).Returns(CancellationToken.None);
        _contextMock.Setup(c => c.ConnectionId).Returns("test-connection-id");
    }

    [Fact]
    public async Task RequestRewrite_ShouldCallAgentService_WithBuiltPrompt()
    {
        // Arrange
        var chapterId = Guid.NewGuid().ToString();
        var selectedText = "Texto original";
        var command = "Reescreva com mais emoção";

        _agentServiceMock
            .Setup(s => s.StreamCompletionAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            ))
            .Returns(AsyncEnumerable.Empty<string>());

        // Act
        await _hub.RequestRewrite(chapterId, selectedText, command);

        // Assert
        _agentServiceMock.Verify(
            s => s.StreamCompletionAsync(
                It.Is<string>(prompt => 
                    prompt.Contains(selectedText) && 
                    prompt.Contains(command)
                ),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task RequestRewrite_ShouldStreamTokensToCaller_WhenLLMResponds()
    {
        // Arrange
        var chapterId = Guid.NewGuid().ToString();
        var selectedText = "Texto";
        var command = "Comando";
        var tokens = new[] { "Token1 ", "Token2 ", "Token3" };
        var receivedTokens = new List<string>();

        _agentServiceMock
            .Setup(s => s.StreamCompletionAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            ))
            .Returns(tokens.ToAsyncEnumerable());

        _callerMock
            .Setup(c => c.SendCoreAsync(
                "OnTokenReceived",
                It.IsAny<object[]>(),
                It.IsAny<CancellationToken>()
            ))
            .Callback<string, object[], CancellationToken>((_, args, _) => 
            {
                receivedTokens.Add((string)args[0]);
            })
            .Returns(Task.CompletedTask);

        // Act
        await _hub.RequestRewrite(chapterId, selectedText, command);

        // Assert
        receivedTokens.Should().HaveCount(3);
        receivedTokens.Should().ContainInOrder("Token1 ", "Token2 ", "Token3");
    }

    [Fact]
    public async Task RequestRewrite_ShouldSendOnComplete_AfterStreaming()
    {
        // Arrange
        var chapterId = Guid.NewGuid().ToString();
        var selectedText = "Texto";
        var command = "Comando";

        _agentServiceMock
            .Setup(s => s.StreamCompletionAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            ))
            .Returns(AsyncEnumerable.Empty<string>());

        // Act
        await _hub.RequestRewrite(chapterId, selectedText, command);

        // Assert
        _callerMock.Verify(
            c => c.SendCoreAsync(
                "OnComplete",
                It.IsAny<object[]>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task RequestRewrite_ShouldSendOnError_WhenExceptionOccurs()
    {
        // Arrange
        var chapterId = Guid.NewGuid().ToString();
        var selectedText = "Texto";
        var command = "Comando";
        var errorMessage = "LLM service unavailable";

        _agentServiceMock
            .Setup(s => s.StreamCompletionAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            ))
            .Throws(new InvalidOperationException(errorMessage));

        // Act
        await _hub.RequestRewrite(chapterId, selectedText, command);

        // Assert
        _callerMock.Verify(
            c => c.SendCoreAsync(
                "OnError",
                It.Is<object[]>(args => args[0].ToString() == errorMessage),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task RequestRewrite_ShouldLogInformation_WhenRequestStarts()
    {
        // Arrange
        var chapterId = Guid.NewGuid().ToString();
        var selectedText = "Texto";
        var command = "Comando";

        _agentServiceMock
            .Setup(s => s.StreamCompletionAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            ))
            .Returns(AsyncEnumerable.Empty<string>());

        // Act
        await _hub.RequestRewrite(chapterId, selectedText, command);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Received rewrite request")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce
        );
    }

    [Fact]
    public async Task OnConnectedAsync_ShouldLogConnectionId()
    {
        // Act
        await _hub.OnConnectedAsync();

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Client connected")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once
        );
    }

    [Fact]
    public async Task OnDisconnectedAsync_ShouldLogDisconnection_WhenNoException()
    {
        // Act
        await _hub.OnDisconnectedAsync(null);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Client disconnected")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once
        );
    }

    [Fact]
    public async Task OnDisconnectedAsync_ShouldLogError_WhenExceptionProvided()
    {
        // Arrange
        var exception = new Exception("Connection error");

        // Act
        await _hub.OnDisconnectedAsync(exception);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Client disconnected with error")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once
        );
    }
}
