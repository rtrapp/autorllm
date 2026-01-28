using FluentAssertions;
using AutorLLM.Application.Commands.Characters.CreateCharacter;

namespace AutorLLM.Tests.Unit.Application.Commands.Characters;

public class CreateCharacterCommandValidatorTests
{
    private readonly CreateCharacterCommandValidator _validator;

    public CreateCharacterCommandValidatorTests()
    {
        _validator = new CreateCharacterCommandValidator();
    }

    [Fact]
    public void Should_Pass_When_Command_Is_Valid()
    {
        // Arrange
        var command = new CreateCharacterCommand
        {
            ProjectId = Guid.NewGuid(),
            Name = "Frodo Baggins",
            Role = "Protagonist",
            Description = "A brave hobbit",
            Backstory = "A hobbit from the Shire"
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
        var command = new CreateCharacterCommand
        {
            ProjectId = Guid.Empty,
            Name = "Frodo Baggins",
            Role = "Protagonist",
            Description = "A brave hobbit",
            Backstory = "A hobbit from the Shire"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ProjectId" && e.ErrorMessage == "ProjectId is required");
    }

    [Fact]
    public void Should_Fail_When_Name_Is_Empty()
    {
        // Arrange
        var command = new CreateCharacterCommand
        {
            ProjectId = Guid.NewGuid(),
            Name = "",
            Role = "Protagonist",
            Description = "A brave hobbit",
            Backstory = "A hobbit from the Shire"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorMessage == "Name is required");
    }

    [Fact]
    public void Should_Fail_When_Name_Exceeds_Maximum_Length()
    {
        // Arrange
        var command = new CreateCharacterCommand
        {
            ProjectId = Guid.NewGuid(),
            Name = new string('a', 101),
            Role = "Protagonist",
            Description = "A brave hobbit",
            Backstory = "A hobbit from the Shire"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorMessage == "Name must be under 100 characters");
    }

    [Fact]
    public void Should_Fail_When_Role_Is_Empty()
    {
        // Arrange
        var command = new CreateCharacterCommand
        {
            ProjectId = Guid.NewGuid(),
            Name = "Frodo Baggins",
            Role = "",
            Description = "A brave hobbit",
            Backstory = "A hobbit from the Shire"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Role" && e.ErrorMessage == "Role is required");
    }

    [Fact]
    public void Should_Fail_When_Role_Exceeds_Maximum_Length()
    {
        // Arrange
        var command = new CreateCharacterCommand
        {
            ProjectId = Guid.NewGuid(),
            Name = "Frodo Baggins",
            Role = new string('a', 51),
            Description = "A brave hobbit",
            Backstory = "A hobbit from the Shire"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Role" && e.ErrorMessage == "Role must be under 50 characters");
    }

    [Fact]
    public void Should_Fail_When_Backstory_Exceeds_Maximum_Length()
    {
        // Arrange
        var command = new CreateCharacterCommand
        {
            ProjectId = Guid.NewGuid(),
            Name = "Frodo Baggins",
            Role = "Protagonist",
            Description = "A brave hobbit",
            Backstory = new string('a', 5001)
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Backstory" && e.ErrorMessage == "Backstory must be under 5000 characters");
    }

    [Fact]
    public void Should_Pass_When_Backstory_Is_Empty()
    {
        // Arrange
        var command = new CreateCharacterCommand
        {
            ProjectId = Guid.NewGuid(),
            Name = "Frodo Baggins",
            Role = "Protagonist",
            Description = "A brave hobbit",
            Backstory = ""
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
