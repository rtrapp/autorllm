using FluentAssertions;
using AutorLLM.Application.Commands.Characters.DeleteCharacter;

namespace AutorLLM.Tests.Unit.Application.Commands.Characters;

public class DeleteCharacterCommandValidatorTests
{
    private readonly DeleteCharacterCommandValidator _validator;

    public DeleteCharacterCommandValidatorTests()
    {
        _validator = new DeleteCharacterCommandValidator();
    }

    [Fact]
    public void Should_Pass_When_Command_Is_Valid()
    {
        // Arrange
        var command = new DeleteCharacterCommand
        {
            ProjectId = Guid.NewGuid(),
            CharacterId = Guid.NewGuid()
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
        var command = new DeleteCharacterCommand
        {
            ProjectId = Guid.Empty,
            CharacterId = Guid.NewGuid()
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ProjectId" && e.ErrorMessage == "ProjectId is required");
    }

    [Fact]
    public void Should_Fail_When_CharacterId_Is_Empty()
    {
        // Arrange
        var command = new DeleteCharacterCommand
        {
            ProjectId = Guid.NewGuid(),
            CharacterId = Guid.Empty
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CharacterId" && e.ErrorMessage == "CharacterId is required");
    }

    [Fact]
    public void Should_Fail_When_Both_Ids_Are_Empty()
    {
        // Arrange
        var command = new DeleteCharacterCommand
        {
            ProjectId = Guid.Empty,
            CharacterId = Guid.Empty
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
        result.Errors.Should().Contain(e => e.PropertyName == "ProjectId");
        result.Errors.Should().Contain(e => e.PropertyName == "CharacterId");
    }
}
