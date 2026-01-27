using FluentAssertions;
using AutorLLM.Application.Commands.Chapters.CreateChapter;

namespace AutorLLM.Tests.Unit.Application.Commands.Chapters;

public class CreateChapterCommandValidatorTests
{
    private readonly CreateChapterCommandValidator _validator;

    public CreateChapterCommandValidatorTests()
    {
        _validator = new CreateChapterCommandValidator();
    }

    [Fact]
    public void Should_Pass_When_Command_Is_Valid()
    {
        // Arrange
        var command = new CreateChapterCommand
        {
            ProjectId = Guid.NewGuid(),
            Title = "Chapter 1: The Beginning"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Should_Fail_When_ProjectId_Is_Empty()
    {
        // Arrange
        var command = new CreateChapterCommand
        {
            ProjectId = Guid.Empty,
            Title = "Chapter 1: The Beginning"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ProjectId" && e.ErrorMessage == "ProjectId is required");
    }

    [Fact]
    public void Should_Fail_When_Title_Is_Empty()
    {
        // Arrange
        var command = new CreateChapterCommand
        {
            ProjectId = Guid.NewGuid(),
            Title = ""
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title" && e.ErrorMessage == "Title is required");
    }

    [Fact]
    public void Should_Fail_When_Title_Exceeds_Maximum_Length()
    {
        // Arrange
        var command = new CreateChapterCommand
        {
            ProjectId = Guid.NewGuid(),
            Title = new string('A', 201) // Exceeds 200 char limit
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title" && e.ErrorMessage == "Title cannot exceed 200 characters");
    }

    [Fact]
    public void Should_Pass_When_Title_Is_At_Maximum_Length()
    {
        // Arrange
        var command = new CreateChapterCommand
        {
            ProjectId = Guid.NewGuid(),
            Title = new string('A', 200) // Exactly 200 chars
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
