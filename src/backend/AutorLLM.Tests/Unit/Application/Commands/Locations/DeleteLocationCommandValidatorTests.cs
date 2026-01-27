using FluentAssertions;
using AutorLLM.Application.Commands.Locations.DeleteLocation;

namespace AutorLLM.Tests.Unit.Application.Commands.Locations;

public class DeleteLocationCommandValidatorTests
{
    private readonly DeleteLocationCommandValidator _validator;

    public DeleteLocationCommandValidatorTests()
    {
        _validator = new DeleteLocationCommandValidator();
    }

    [Fact]
    public void Should_Pass_When_Command_Is_Valid()
    {
        // Arrange
        var command = new DeleteLocationCommand
        {
            ProjectId = Guid.NewGuid(),
            LocationId = Guid.NewGuid()
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
        var command = new DeleteLocationCommand
        {
            ProjectId = Guid.Empty,
            LocationId = Guid.NewGuid()
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "ProjectId");
    }

    [Fact]
    public void Should_Fail_When_LocationId_Is_Empty()
    {
        // Arrange
        var command = new DeleteLocationCommand
        {
            ProjectId = Guid.NewGuid(),
            LocationId = Guid.Empty
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "LocationId");
    }

    [Fact]
    public void Should_Fail_When_Both_Ids_Are_Empty()
    {
        // Arrange
        var command = new DeleteLocationCommand
        {
            ProjectId = Guid.Empty,
            LocationId = Guid.Empty
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
    }
}
