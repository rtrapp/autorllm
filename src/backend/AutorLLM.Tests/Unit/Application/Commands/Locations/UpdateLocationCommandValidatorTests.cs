using FluentAssertions;
using AutorLLM.Application.Commands.Locations.UpdateLocation;

namespace AutorLLM.Tests.Unit.Application.Commands.Locations;

public class UpdateLocationCommandValidatorTests
{
    private readonly UpdateLocationCommandValidator _validator;

    public UpdateLocationCommandValidatorTests()
    {
        _validator = new UpdateLocationCommandValidator();
    }

    [Fact]
    public void Should_Pass_When_Command_Is_Valid()
    {
        // Arrange
        var command = new UpdateLocationCommand
        {
            ProjectId = Guid.NewGuid(),
            LocationId = Guid.NewGuid(),
            Name = "The Shire",
            Description = "A peaceful land of hobbits",
            Geography = "Rolling hills",
            Culture = "Hobbit culture",
            Significance = "Home of Frodo"
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
        var command = new UpdateLocationCommand
        {
            ProjectId = Guid.Empty,
            LocationId = Guid.NewGuid(),
            Name = "The Shire",
            Description = "A peaceful land"
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
        var command = new UpdateLocationCommand
        {
            ProjectId = Guid.NewGuid(),
            LocationId = Guid.Empty,
            Name = "The Shire",
            Description = "A peaceful land"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "LocationId");
    }

    [Fact]
    public void Should_Fail_When_Name_Is_Empty()
    {
        // Arrange
        var command = new UpdateLocationCommand
        {
            ProjectId = Guid.NewGuid(),
            LocationId = Guid.NewGuid(),
            Name = "",
            Description = "Test"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Should_Fail_When_Geography_Exceeds_Max_Length()
    {
        // Arrange
        var command = new UpdateLocationCommand
        {
            ProjectId = Guid.NewGuid(),
            LocationId = Guid.NewGuid(),
            Name = "Test Location",
            Description = "Test",
            Geography = new string('a', 2001) // 2001 characters
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Geography");
    }

    [Fact]
    public void Should_Fail_When_Culture_Exceeds_Max_Length()
    {
        // Arrange
        var command = new UpdateLocationCommand
        {
            ProjectId = Guid.NewGuid(),
            LocationId = Guid.NewGuid(),
            Name = "Test Location",
            Description = "Test",
            Culture = new string('a', 2001) // 2001 characters
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Culture");
    }

    [Fact]
    public void Should_Fail_When_Significance_Exceeds_Max_Length()
    {
        // Arrange
        var command = new UpdateLocationCommand
        {
            ProjectId = Guid.NewGuid(),
            LocationId = Guid.NewGuid(),
            Name = "Test Location",
            Description = "Test",
            Significance = new string('a', 1001) // 1001 characters
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Significance");
    }

    [Fact]
    public void Should_Pass_When_Optional_Fields_Are_Null()
    {
        // Arrange
        var command = new UpdateLocationCommand
        {
            ProjectId = Guid.NewGuid(),
            LocationId = Guid.NewGuid(),
            Name = "Test Location",
            Description = "Test",
            Geography = null,
            Culture = null,
            Significance = null
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
