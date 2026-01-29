using AutorLLM.Application.Commands.LLM.StartBrainstorm;
using FluentValidation.TestHelper;
using Xunit;

namespace AutorLLM.Tests.Unit.Application.Commands.LLM.StartBrainstorm;

public class StartBrainstormCommandValidatorTests
{
    private readonly StartBrainstormCommandValidator _validator;

    public StartBrainstormCommandValidatorTests()
    {
        _validator = new StartBrainstormCommandValidator();
    }

    [Fact]
    public void Validate_WhenBookIdeaIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new StartBrainstormCommand { BookIdea = string.Empty };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BookIdea)
            .WithErrorMessage("BookIdea is required");
    }

    [Fact]
    public void Validate_WhenBookIdeaIsTooShort_ShouldHaveValidationError()
    {
        // Arrange
        var command = new StartBrainstormCommand { BookIdea = "Too short" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BookIdea)
            .WithErrorMessage("BookIdea must be at least 20 characters");
    }

    [Fact]
    public void Validate_WhenBookIdeaIsTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var command = new StartBrainstormCommand 
        { 
            BookIdea = new string('a', 5001) 
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BookIdea)
            .WithErrorMessage("BookIdea must be under 5000 characters");
    }

    [Fact]
    public void Validate_WhenBookIdeaIsValid_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new StartBrainstormCommand 
        { 
            BookIdea = "Uma história sobre um jovem mago descobrindo seus poderes em um mundo pós-apocalíptico" 
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
