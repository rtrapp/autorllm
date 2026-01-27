using FluentAssertions;
using AutorLLM.Application.Commands.Locations.CreateLocation;

namespace AutorLLM.Tests.Unit.Application.Commands.Locations;

public class CreateLocationCommandValidatorTests
{
    private readonly CreateLocationCommandValidator _validator;

    public CreateLocationCommandValidatorTests()
    {
        _validator = new CreateLocationCommandValidator();
    }

    [Fact]
    public void Should_Pass_When_Command_Is_Valid()
    {
        // Arrange
        var command = new CreateLocationCommand
        {
            ProjectId = Guid.NewGuid(),
            Name = "The Shire",
            Description = "A peaceful land of hobbits"
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
        var command = new CreateLocationCommand
        {
            ProjectId = Guid.Empty,
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
    public void Should_Fail_When_Name_Is_Empty()
    {
        // Arrange
        var command = new CreateLocationCommand
        {
            ProjectId = Guid.NewGuid(),
            Name = "",
            Description = "A peaceful land"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Should_Fail_When_Name_Exceeds_Max_Length()
    {
        // Arrange
        var command = new CreateLocationCommand
        {
            ProjectId = Guid.NewGuid(),
            Name = new string('a', 101), // 101 characters
            Description = "Test"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Should_Fail_When_Description_Exceeds_Max_Length()
    {
        // Arrange
        var command = new CreateLocationCommand
        {
            ProjectId = Guid.NewGuid(),
            Name = "Test Location",
            Description = new string('a', 1001) // 1001 characters
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Description");
    }

    [Fact]
    public void Should_Pass_When_Description_Is_Empty()
    {
        // Arrange
        var command = new CreateLocationCommand
        {
            ProjectId = Guid.NewGuid(),
            Name = "Test Location",
            Description = ""
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
