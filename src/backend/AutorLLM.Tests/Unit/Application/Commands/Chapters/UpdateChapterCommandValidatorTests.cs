using FluentAssertions;
using AutorLLM.Application.Commands.Chapters.UpdateChapter;

namespace AutorLLM.Tests.Unit.Application.Commands.Chapters;

public class UpdateChapterCommandValidatorTests
{
    private readonly UpdateChapterCommandValidator _validator;

    public UpdateChapterCommandValidatorTests()
    {
        _validator = new UpdateChapterCommandValidator();
    }

    [Fact]
    public void Should_Pass_When_Command_Is_Valid()
    {
        // Arrange
        var command = new UpdateChapterCommand
        {
            ProjectId = Guid.NewGuid(),
            ChapterId = Guid.NewGuid(),
            Title = "Chapter 1: The Beginning",
            Summary = "The hero starts their journey",
            Content = "It was a dark and stormy night..."
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
        var command = new UpdateChapterCommand
        {
            ProjectId = Guid.Empty,
            ChapterId = Guid.NewGuid(),
            Title = "Chapter 1"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ProjectId" && e.ErrorMessage == "ProjectId is required");
    }

    [Fact]
    public void Should_Fail_When_ChapterId_Is_Empty()
    {
        // Arrange
        var command = new UpdateChapterCommand
        {
            ProjectId = Guid.NewGuid(),
            ChapterId = Guid.Empty,
            Title = "Chapter 1"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ChapterId" && e.ErrorMessage == "ChapterId is required");
    }

    [Fact]
    public void Should_Fail_When_Title_Exceeds_Maximum_Length()
    {
        // Arrange
        var command = new UpdateChapterCommand
        {
            ProjectId = Guid.NewGuid(),
            ChapterId = Guid.NewGuid(),
            Title = new string('a', 201)
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title" && e.ErrorMessage == "Title must be under 200 characters");
    }

    [Fact]
    public void Should_Pass_When_Title_Is_Null()
    {
        // Arrange
        var command = new UpdateChapterCommand
        {
            ProjectId = Guid.NewGuid(),
            ChapterId = Guid.NewGuid(),
            Title = null,
            Summary = "A summary"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Fail_When_Summary_Exceeds_Maximum_Length()
    {
        // Arrange
        var command = new UpdateChapterCommand
        {
            ProjectId = Guid.NewGuid(),
            ChapterId = Guid.NewGuid(),
            Summary = new string('a', 1001)
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Summary" && e.ErrorMessage == "Summary must be under 1000 characters");
    }

    [Fact]
    public void Should_Pass_When_Summary_Is_Null()
    {
        // Arrange
        var command = new UpdateChapterCommand
        {
            ProjectId = Guid.NewGuid(),
            ChapterId = Guid.NewGuid(),
            Summary = null,
            Content = "Some content"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Pass_When_Only_Content_Is_Updated()
    {
        // Arrange
        var command = new UpdateChapterCommand
        {
            ProjectId = Guid.NewGuid(),
            ChapterId = Guid.NewGuid(),
            Content = "New chapter content with lots of text..."
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
