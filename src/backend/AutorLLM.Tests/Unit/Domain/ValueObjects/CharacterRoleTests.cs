using AutorLLM.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace AutorLLM.Tests.Unit.Domain.ValueObjects;

public class CharacterRoleTests
{
    [Fact]
    public void Create_WithValidValue_ShouldCreateCharacterRole()
    {
        // Arrange
        var value = "Hero";

        // Act
        var role = CharacterRole.Create(value);

        // Assert
        role.Should().NotBeNull();
        role.Value.Should().Be(value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithInvalidValue_ShouldThrowArgumentException(string? value)
    {
        // Act
        var act = () => CharacterRole.Create(value!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Character role cannot be empty*");
    }

    [Fact]
    public void Create_WithValueTooLong_ShouldThrowArgumentException()
    {
        // Arrange
        var value = new string('A', 51);

        // Act
        var act = () => CharacterRole.Create(value);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Character role cannot exceed 50 characters*");
    }

    [Fact]
    public void Equals_WithSameValue_ShouldReturnTrue()
    {
        // Arrange
        var role1 = CharacterRole.Create("Hero");
        var role2 = CharacterRole.Create("Hero");

        // Act & Assert
        role1.Should().Be(role2);
        (role1 == role2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentValue_ShouldReturnFalse()
    {
        // Arrange
        var role1 = CharacterRole.Create("Hero");
        var role2 = CharacterRole.Create("Villain");

        // Act & Assert
        role1.Should().NotBe(role2);
        (role1 != role2).Should().BeTrue();
    }

    [Fact]
    public void PredefinedRoles_ShouldBeAvailable()
    {
        // Assert
        CharacterRole.Protagonist.Value.Should().Be("Protagonist");
        CharacterRole.Antagonist.Value.Should().Be("Antagonist");
        CharacterRole.Supporting.Value.Should().Be("Supporting");
        CharacterRole.Minor.Value.Should().Be("Minor");
    }
}
