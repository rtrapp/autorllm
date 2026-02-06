using AutorLLM.Application.Commands.Brainstorm;
using FluentAssertions;

namespace AutorLLM.Tests.Unit.Application.Commands;

public class GenerateOutlineCommandTests
{
    [Fact]
    public void Command_ShouldImplementIRequest()
    {
        // Arrange & Act
        var command = new GenerateOutlineCommand
        {
            SessionId = "test-session-id",
            BookIdea = "Test idea",
            Title = "Test Title",
            Author = "Test Author"
        };

        // Assert
        command.Should().BeAssignableTo<MediatR.IRequest<GenerateOutlineResult>>();
    }

    [Fact]
    public void Command_ShouldHaveAllRequiredProperties()
    {
        // Arrange
        var sessionId = "test-session-id";
        var bookIdea = "Test book idea";
        var title = "Test Title";
        var author = "Test Author";
        var genre = "Fantasy";
        var synopsis = "Test synopsis";

        // Act
        var command = new GenerateOutlineCommand
        {
            SessionId = sessionId,
            BookIdea = bookIdea,
            Title = title,
            Author = author,
            Genre = genre,
            Synopsis = synopsis
        };

        // Assert
        command.SessionId.Should().Be(sessionId);
        command.BookIdea.Should().Be(bookIdea);
        command.Title.Should().Be(title);
        command.Author.Should().Be(author);
        command.Genre.Should().Be(genre);
        command.Synopsis.Should().Be(synopsis);
    }

    [Fact]
    public void Command_ShouldBeImmutable_WhenUsingRecordType()
    {
        // Arrange
        var command = new GenerateOutlineCommand
        {
            SessionId = "test-session-id",
            BookIdea = "Test idea",
            Title = "Test Title",
            Author = "Test Author"
        };

        // Act & Assert
        command.Should().NotBeNull();
        command.GetType().Should().Match(t => t.Name.Contains("GenerateOutlineCommand"));
        
        // Records are immutable by default - verify with-expression creates new instance
        var modified = command with { Title = "New Title" };
        modified.Should().NotBeSameAs(command);
        modified.Title.Should().Be("New Title");
        command.Title.Should().Be("Test Title");
    }

    [Fact]
    public void Command_ShouldSupportOptionalCollections()
    {
        // Arrange & Act
        var command = new GenerateOutlineCommand
        {
            SessionId = "test-session-id",
            BookIdea = "Test idea",
            Title = "Test Title",
            Author = "Test Author",
            Characters = new List<CharacterSuggestion>
            {
                new()
                {
                    Name = "Alice",
                    Description = "Main protagonist",
                    Role = "Protagonist"
                }
            },
            Locations = new List<LocationSuggestion>
            {
                new()
                {
                    Name = "London",
                    Description = "Victorian era city"
                }
            }
        };

        // Assert
        command.Characters.Should().HaveCount(1);
        command.Characters![0].Name.Should().Be("Alice");
        command.Locations.Should().HaveCount(1);
        command.Locations![0].Name.Should().Be("London");
    }

    [Fact]
    public void CharacterSuggestion_ShouldHaveCorrectProperties()
    {
        // Arrange & Act
        var suggestion = new CharacterSuggestion
        {
            Name = "Alice",
            Description = "The hero",
            Role = "Protagonist",
            Backstory = "Born in London",
            Appearance = "Tall and brave",
            Personality = "Courageous and kind"
        };

        // Assert
        suggestion.Name.Should().Be("Alice");
        suggestion.Description.Should().Be("The hero");
        suggestion.Role.Should().Be("Protagonist");
        suggestion.Backstory.Should().Be("Born in London");
        suggestion.Appearance.Should().Be("Tall and brave");
        suggestion.Personality.Should().Be("Courageous and kind");
    }

    [Fact]
    public void PlotSuggestion_ShouldHaveCorrectProperties()
    {
        // Arrange & Act
        var suggestion = new PlotSuggestion
        {
            Title = "The Great Adventure",
            Description = "A journey begins",
            Type = "Main"
        };

        // Assert
        suggestion.Title.Should().Be("The Great Adventure");
        suggestion.Description.Should().Be("A journey begins");
        suggestion.Type.Should().Be("Main");
    }

    [Fact]
    public void GenerateOutlineResult_ShouldContainOutlineData()
    {
        // Arrange
        var outlineData = new OutlineData
        {
            Title = "Test Book",
            Author = "Test Author",
            Synopsis = "Test synopsis",
            Genre = null,
            TargetWordCount = 50000,
            Characters = new List<CharacterData>(),
            Locations = new List<LocationData>(),
            Plots = new List<PlotData>(),
            Chapters = new List<ChapterData>()
        };

        // Act
        var result = new GenerateOutlineResult
        {
            Outline = outlineData,
            ValidationErrors = null!
        };

        // Assert
        result.Outline.Should().NotBeNull();
        result.Outline.Title.Should().Be("Test Book");
        result.ValidationErrors.Should().BeNull();
    }
}
