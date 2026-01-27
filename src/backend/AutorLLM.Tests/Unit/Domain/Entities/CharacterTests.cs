using AutorLLM.Domain.Entities;
using AutorLLM.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace AutorLLM.Tests.Unit.Domain.Entities;

public class CharacterTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateCharacter()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var name = "John Doe";
        var description = "A brave knight";
        var role = CharacterRole.Protagonist;

        // Act
        var character = Character.Create(projectId, name, description, role);

        // Assert
        character.Should().NotBeNull();
        character.Id.Should().NotBeEmpty();
        character.ProjectId.Should().Be(projectId);
        character.Name.Should().Be(name);
        character.Description.Should().Be(description);
        character.Role.Should().Be(role);
        character.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_WithEmptyProjectId_ShouldThrowArgumentException()
    {
        // Arrange
        var projectId = Guid.Empty;
        var name = "John Doe";
        var description = "A brave knight";
        var role = CharacterRole.Protagonist;

        // Act
        var act = () => Character.Create(projectId, name, description, role);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*ProjectId cannot be empty*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithInvalidName_ShouldThrowArgumentException(string? name)
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var description = "A brave knight";
        var role = CharacterRole.Protagonist;

        // Act
        var act = () => Character.Create(projectId, name!, description, role);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Character name cannot be empty*");
    }

    [Fact]
    public void Create_WithNameTooLong_ShouldThrowArgumentException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var name = new string('A', 101); // 101 characters
        var description = "A brave knight";
        var role = CharacterRole.Protagonist;

        // Act
        var act = () => Character.Create(projectId, name, description, role);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Character name cannot exceed 100 characters*");
    }

    [Fact]
    public void Create_WithDescriptionTooLong_ShouldThrowArgumentException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var name = "John Doe";
        var description = new string('A', 1001); // 1001 characters
        var role = CharacterRole.Protagonist;

        // Act
        var act = () => Character.Create(projectId, name, description, role);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Character description cannot exceed 1000 characters*");
    }

    [Fact]
    public void UpdateDetails_WithValidData_ShouldUpdateCharacter()
    {
        // Arrange
        var character = Character.Create(
            Guid.NewGuid(),
            "John Doe",
            "A brave knight",
            CharacterRole.Protagonist);

        var originalUpdatedAt = character.UpdatedAt;
        Thread.Sleep(10); // Ensure time difference

        var newName = "Jane Doe";
        var newDescription = "A cunning thief";
        var newRole = CharacterRole.Antagonist;

        // Act
        character.UpdateDetails(newName, newDescription, newRole);

        // Assert
        character.Name.Should().Be(newName);
        character.Description.Should().Be(newDescription);
        character.Role.Should().Be(newRole);
        character.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void UpdateBackstory_WithValidData_ShouldUpdateBackstory()
    {
        // Arrange
        var character = Character.Create(
            Guid.NewGuid(),
            "John Doe",
            "A brave knight",
            CharacterRole.Protagonist);

        var backstory = "Born in a small village...";

        // Act
        character.UpdateBackstory(backstory);

        // Assert
        character.Backstory.Should().Be(backstory);
    }

    [Fact]
    public void UpdateBackstory_WithTooLongText_ShouldThrowArgumentException()
    {
        // Arrange
        var character = Character.Create(
            Guid.NewGuid(),
            "John Doe",
            "A brave knight",
            CharacterRole.Protagonist);

        var backstory = new string('A', 5001); // 5001 characters

        // Act
        var act = () => character.UpdateBackstory(backstory);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Backstory cannot exceed 5000 characters*");
    }

    [Fact]
    public void UpdateAppearance_WithValidData_ShouldUpdateAppearance()
    {
        // Arrange
        var character = Character.Create(
            Guid.NewGuid(),
            "John Doe",
            "A brave knight",
            CharacterRole.Protagonist);

        var appearance = "Tall with dark hair";

        // Act
        character.UpdateAppearance(appearance);

        // Assert
        character.Appearance.Should().Be(appearance);
    }

    [Fact]
    public void UpdatePersonality_WithValidData_ShouldUpdatePersonality()
    {
        // Arrange
        var character = Character.Create(
            Guid.NewGuid(),
            "John Doe",
            "A brave knight",
            CharacterRole.Protagonist);

        var personality = "Brave and loyal";

        // Act
        character.UpdatePersonality(personality);

        // Assert
        character.Personality.Should().Be(personality);
    }
}
