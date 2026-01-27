using FluentAssertions;
using AutorLLM.Application.Commands.Chapters.DeleteChapter;

namespace AutorLLM.Tests.Unit.Application.Commands.Chapters;

public class DeleteChapterCommandValidatorTests
{
    private readonly DeleteChapterCommandValidator _validator;

    public DeleteChapterCommandValidatorTests()
    {
        _validator = new DeleteChapterCommandValidator();
    }

    [Fact]
    public void Should_Pass_When_Command_Is_Valid()
    {
        // Arrange
        var command = new DeleteChapterCommand
        {
            ProjectId = Guid.NewGuid(),
            ChapterId = Guid.NewGuid()
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
        var command = new DeleteChapterCommand
        {
            ProjectId = Guid.Empty,
            ChapterId = Guid.NewGuid()
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
        var command = new DeleteChapterCommand
        {
            ProjectId = Guid.NewGuid(),
            ChapterId = Guid.Empty
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ChapterId" && e.ErrorMessage == "ChapterId is required");
    }
}
