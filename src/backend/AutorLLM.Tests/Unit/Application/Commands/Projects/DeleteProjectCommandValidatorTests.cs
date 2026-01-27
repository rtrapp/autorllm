using FluentAssertions;
using AutorLLM.Application.Commands.Projects.DeleteProject;

namespace AutorLLM.Tests.Unit.Application.Commands.Projects;

public class DeleteProjectCommandValidatorTests
{
    private readonly DeleteProjectCommandValidator _validator;

    public DeleteProjectCommandValidatorTests()
    {
        _validator = new DeleteProjectCommandValidator();
    }

    [Fact]
    public void Should_Pass_When_Command_Is_Valid()
    {
        // Arrange
        var command = new DeleteProjectCommand
        {
            ProjectId = Guid.NewGuid()
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
        var command = new DeleteProjectCommand
        {
            ProjectId = Guid.Empty
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ProjectId" && e.ErrorMessage == "ProjectId is required");
    }
}
