using AutorLLM.Application.Commands.Brainstorm;
using FluentValidation.TestHelper;

namespace AutorLLM.Tests.Unit.Application.Commands;

public class GenerateOutlineCommandValidatorTests
{
    private readonly GenerateOutlineCommandValidator _validator = new();

    [Fact]
    public void Validator_ShouldPass_WhenAllRequiredFieldsAreValid()
    {
        // Arrange
        var command = new GenerateOutlineCommand
        {
            SessionId = "test-session-id",
            BookIdea = "A thrilling mystery novel",
            Title = "The Silent Echo",
            Author = "John Doe"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validator_ShouldFail_WhenSessionIdIsEmpty()
    {
        // Arrange
        var command = new GenerateOutlineCommand
        {
            SessionId = "",
            BookIdea = "A thrilling mystery novel",
            Title = "The Silent Echo",
            Author = "John Doe"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SessionId)
            .WithErrorMessage("SessionId is required");
    }

    [Fact]
    public void Validator_ShouldFail_WhenBookIdeaIsEmpty()
    {
        // Arrange
        var command = new GenerateOutlineCommand
        {
            SessionId = "test-session-id",
            BookIdea = "",
            Title = "The Silent Echo",
            Author = "John Doe"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BookIdea)
            .WithErrorMessage("BookIdea is required");
    }

    [Fact]
    public void Validator_ShouldFail_WhenTitleExceedsMaxLength()
    {
        // Arrange
        var command = new GenerateOutlineCommand
        {
            SessionId = "test-session-id",
            BookIdea = "A thrilling mystery novel",
            Title = new string('A', 201),
            Author = "John Doe"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title cannot exceed 200 characters");
    }

    [Fact]
    public void Validator_ShouldFail_WhenAuthorExceedsMaxLength()
    {
        // Arrange
        var command = new GenerateOutlineCommand
        {
            SessionId = "test-session-id",
            BookIdea = "A thrilling mystery novel",
            Title = "The Silent Echo",
            Author = new string('A', 101)
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Author)
            .WithErrorMessage("Author cannot exceed 100 characters");
    }

    [Fact]
    public void Validator_ShouldFail_WhenGenreExceedsMaxLength()
    {
        // Arrange
        var command = new GenerateOutlineCommand
        {
            SessionId = "test-session-id",
            BookIdea = "A thrilling mystery novel",
            Title = "The Silent Echo",
            Author = "John Doe",
            Genre = new string('A', 51)
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Genre)
            .WithErrorMessage("Genre cannot exceed 50 characters");
    }

    [Fact]
    public void Validator_ShouldFail_WhenSynopsisExceedsMaxLength()
    {
        // Arrange
        var command = new GenerateOutlineCommand
        {
            SessionId = "test-session-id",
            BookIdea = "A thrilling mystery novel",
            Title = "The Silent Echo",
            Author = "John Doe",
            Synopsis = new string('A', 5001)
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Synopsis)
            .WithErrorMessage("Synopsis cannot exceed 5000 characters");
    }

    [Fact]
    public void Validator_ShouldFail_WhenCharacterNameIsEmpty()
    {
        // Arrange
        var command = new GenerateOutlineCommand
        {
            SessionId = "test-session-id",
            BookIdea = "A thrilling mystery novel",
            Title = "The Silent Echo",
            Author = "John Doe",
            Characters = new List<CharacterSuggestion>
            {
                new()
                {
                    Name = "",
                    Description = "Main protagonist",
                    Role = "Protagonist"
                }
            }
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor("Characters[0].Name")
            .WithErrorMessage("Character name is required");
    }

    [Fact]
    public void Validator_ShouldFail_WhenCharacterNameExceedsMaxLength()
    {
        // Arrange
        var command = new GenerateOutlineCommand
        {
            SessionId = "test-session-id",
            BookIdea = "A thrilling mystery novel",
            Title = "The Silent Echo",
            Author = "John Doe",
            Characters = new List<CharacterSuggestion>
            {
                new()
                {
                    Name = new string('A', 101),
                    Description = "Main protagonist",
                    Role = "Protagonist"
                }
            }
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor("Characters[0].Name")
            .WithErrorMessage("Character name cannot exceed 100 characters");
    }

    [Fact]
    public void Validator_ShouldFail_WhenPlotTitleIsEmpty()
    {
        // Arrange
        var command = new GenerateOutlineCommand
        {
            SessionId = "test-session-id",
            BookIdea = "A thrilling mystery novel",
            Title = "The Silent Echo",
            Author = "John Doe",
            Plots = new List<PlotSuggestion>
            {
                new()
                {
                    Title = "",
                    Description = "The main conflict",
                    Type = "Main"
                }
            }
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor("Plots[0].Title")
            .WithErrorMessage("Plot title is required");
    }

    [Fact]
    public void Validator_ShouldPass_WhenOptionalFieldsAreNull()
    {
        // Arrange
        var command = new GenerateOutlineCommand
        {
            SessionId = "test-session-id",
            BookIdea = "A thrilling mystery novel",
            Title = null,
            Author = null,
            Genre = null,
            Synopsis = null,
            Characters = null,
            Locations = null,
            Plots = null,
            Chapters = null
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
