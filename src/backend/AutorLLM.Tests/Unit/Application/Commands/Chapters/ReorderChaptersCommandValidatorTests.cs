using FluentAssertions;
using AutorLLM.Application.Commands.Chapters.ReorderChapters;

namespace AutorLLM.Tests.Unit.Application.Commands.Chapters;

public class ReorderChaptersCommandValidatorTests
{
    private readonly ReorderChaptersCommandValidator _validator;

    public ReorderChaptersCommandValidatorTests()
    {
        _validator = new ReorderChaptersCommandValidator();
    }

    [Fact]
    public void Should_Pass_When_Command_Is_Valid()
    {
        // Arrange
        var command = new ReorderChaptersCommand
        {
            ProjectId = Guid.NewGuid(),
            ChapterIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() }
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
        var command = new ReorderChaptersCommand
        {
            ProjectId = Guid.Empty,
            ChapterIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ProjectId" && e.ErrorMessage == "ProjectId is required");
    }

    [Fact]
    public void Should_Fail_When_ChapterIds_Is_Empty()
    {
        // Arrange
        var command = new ReorderChaptersCommand
        {
            ProjectId = Guid.NewGuid(),
            ChapterIds = new List<Guid>()
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ChapterIds" && e.ErrorMessage == "ChapterIds list cannot be empty");
    }
}
