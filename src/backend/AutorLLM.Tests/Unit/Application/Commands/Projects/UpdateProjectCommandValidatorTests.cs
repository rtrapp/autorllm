using FluentAssertions;
using AutorLLM.Application.Commands.Projects.UpdateProject;

namespace AutorLLM.Tests.Unit.Application.Commands.Projects;

public class UpdateProjectCommandValidatorTests
{
    private readonly UpdateProjectCommandValidator _validator;

    public UpdateProjectCommandValidatorTests()
    {
        _validator = new UpdateProjectCommandValidator();
    }

    [Fact]
    public void Should_Pass_When_Command_Is_Valid()
    {
        // Arrange
        var command = new UpdateProjectCommand
        {
            ProjectId = Guid.NewGuid(),
            Title = "Updated Title",
            Author = "Updated Author"
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
        var command = new UpdateProjectCommand
        {
            ProjectId = Guid.Empty,
            Title = "Updated Title"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ProjectId" && e.ErrorMessage == "ProjectId is required");
    }

    [Fact]
    public void Should_Fail_When_Title_Exceeds_Maximum_Length()
    {
        // Arrange
        var command = new UpdateProjectCommand
        {
            ProjectId = Guid.NewGuid(),
            Title = new string('a', 201)
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title" && e.ErrorMessage == "Title must be under 200 characters");
    }

    [Fact]
    public void Should_Fail_When_Author_Exceeds_Maximum_Length()
    {
        // Arrange
        var command = new UpdateProjectCommand
        {
            ProjectId = Guid.NewGuid(),
            Author = new string('a', 101)
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Author" && e.ErrorMessage == "Author must be under 100 characters");
    }

    [Fact]
    public void Should_Fail_When_Synopsis_Exceeds_Maximum_Length()
    {
        // Arrange
        var command = new UpdateProjectCommand
        {
            ProjectId = Guid.NewGuid(),
            Synopsis = new string('a', 5001)
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Synopsis" && e.ErrorMessage == "Synopsis must be under 5000 characters");
    }

    [Fact]
    public void Should_Fail_When_Genre_Exceeds_Maximum_Length()
    {
        // Arrange
        var command = new UpdateProjectCommand
        {
            ProjectId = Guid.NewGuid(),
            Genre = new string('a', 51)
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Genre" && e.ErrorMessage == "Genre must be under 50 characters");
    }

    [Fact]
    public void Should_Fail_When_TargetWordCount_Is_Negative()
    {
        // Arrange
        var command = new UpdateProjectCommand
        {
            ProjectId = Guid.NewGuid(),
            TargetWordCount = -1
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "TargetWordCount" && e.ErrorMessage == "Target word count must be greater than or equal to 0");
    }

    [Fact]
    public void Should_Pass_When_All_Fields_Are_Null()
    {
        // Arrange - only ProjectId is required
        var command = new UpdateProjectCommand
        {
            ProjectId = Guid.NewGuid()
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
