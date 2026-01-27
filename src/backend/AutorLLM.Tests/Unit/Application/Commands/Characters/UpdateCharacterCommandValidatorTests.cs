using FluentAssertions;
using AutorLLM.Application.Commands.Characters.UpdateCharacter;

namespace AutorLLM.Tests.Unit.Application.Commands.Characters;

public class UpdateCharacterCommandValidatorTests
{
    private readonly UpdateCharacterCommandValidator _validator;

    public UpdateCharacterCommandValidatorTests()
    {
        _validator = new UpdateCharacterCommandValidator();
    }

    [Fact]
    public void Should_Pass_When_Command_Is_Valid()
    {
        // Arrange
        var command = new UpdateCharacterCommand
        {
            ProjectId = Guid.NewGuid(),
            CharacterId = Guid.NewGuid(),
            Name = "Frodo Baggins",
            Description = "A hobbit from the Shire",
            Role = "Protagonist"
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
        var command = new UpdateCharacterCommand
        {
            ProjectId = Guid.Empty,
            CharacterId = Guid.NewGuid(),
            Name = "Frodo",
            Description = "A hobbit",
            Role = "Protagonist"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ProjectId");
    }

    [Fact]
    public void Should_Fail_When_CharacterId_Is_Empty()
    {
        // Arrange
        var command = new UpdateCharacterCommand
        {
            ProjectId = Guid.NewGuid(),
            CharacterId = Guid.Empty,
            Name = "Frodo",
            Description = "A hobbit",
            Role = "Protagonist"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CharacterId");
    }

    [Fact]
    public void Should_Fail_When_Name_Is_Empty()
    {
        // Arrange
        var command = new UpdateCharacterCommand
        {
            ProjectId = Guid.NewGuid(),
            CharacterId = Guid.NewGuid(),
            Name = "",
            Description = "A hobbit",
            Role = "Protagonist"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Should_Fail_When_Name_Exceeds_Maximum_Length()
    {
        // Arrange
        var command = new UpdateCharacterCommand
        {
            ProjectId = Guid.NewGuid(),
            CharacterId = Guid.NewGuid(),
            Name = new string('a', 101),
            Description = "A hobbit",
            Role = "Protagonist"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Should_Fail_When_Role_Is_Invalid()
    {
        // Arrange
        var command = new UpdateCharacterCommand
        {
            ProjectId = Guid.NewGuid(),
            CharacterId = Guid.NewGuid(),
            Name = "Frodo",
            Description = "A hobbit",
            Role = "InvalidRole"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Role");
    }

    [Fact]
    public void Should_Pass_When_Optional_Fields_Are_Null()
    {
        // Arrange
        var command = new UpdateCharacterCommand
        {
            ProjectId = Guid.NewGuid(),
            CharacterId = Guid.NewGuid(),
            Name = "Frodo",
            Description = "A hobbit",
            Role = "Protagonist",
            Backstory = null,
            Appearance = null,
            Personality = null
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Fail_When_Backstory_Exceeds_Maximum_Length()
    {
        // Arrange
        var command = new UpdateCharacterCommand
        {
            ProjectId = Guid.NewGuid(),
            CharacterId = Guid.NewGuid(),
            Name = "Frodo",
            Description = "A hobbit",
            Role = "Protagonist",
            Backstory = new string('a', 5001)
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Backstory");
    }
}
