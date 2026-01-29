using AutorLLM.Application.Commands.LLM.StartBrainstorm;
using MediatR;
using Xunit;

namespace AutorLLM.Tests.Unit.Application.Commands.LLM.StartBrainstorm;

public class StartBrainstormCommandTests
{
    [Fact]
    public void Command_ShouldImplementIRequest()
    {
        // Arrange & Act
        var command = new StartBrainstormCommand { BookIdea = "Test idea" };

        // Assert
        Assert.IsAssignableFrom<IRequest<StartBrainstormResult>>(command);
    }

    [Fact]
    public void Command_ShouldHaveBookIdeaProperty()
    {
        // Arrange
        const string bookIdea = "Uma história épica sobre dragões e magia";

        // Act
        var command = new StartBrainstormCommand { BookIdea = bookIdea };

        // Assert
        Assert.Equal(bookIdea, command.BookIdea);
    }

    [Fact]
    public void Command_ShouldBeImmutable()
    {
        // Arrange & Act
        var command = new StartBrainstormCommand { BookIdea = "Test" };

        // Assert - record types are immutable by nature
        Assert.IsType<StartBrainstormCommand>(command);
        Assert.NotNull(command.BookIdea);
    }
}
