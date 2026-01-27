using AutorLLM.Domain.Aggregates.ProjectAggregate;
using AutorLLM.Domain.Entities;
using AutorLLM.Domain.Services;
using AutorLLM.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace AutorLLM.Tests.Unit.Domain.Services;

public class CharacterConsistencyServiceTests
{
    private readonly CharacterConsistencyService _service;

    public CharacterConsistencyServiceTests()
    {
        _service = new CharacterConsistencyService();
    }

    [Fact]
    public void ValidateCharacterConsistency_WithValidCharacter_ShouldReturnTrue()
    {
        // Arrange
        var project = Project.Create("Test Novel", "Author", "Synopsis");
        var character = project.AddCharacter("Hero", "Main character", CharacterRole.Protagonist);
        var chapters = CreateChapters(project.Id, 10);

        // Act
        var result = _service.ValidateCharacterConsistency(character, chapters);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void HasAdequatePresence_WithAnyCharacter_ShouldReturnTrue()
    {
        // Arrange (note: ChapterAppearances not yet implemented, so service returns true)
        var project = Project.Create("Test Novel", "Author", "Synopsis");
        var character = project.AddCharacter("Hero", "Main character", CharacterRole.Protagonist);

        // Act
        var result = _service.HasAdequatePresence(character, 10);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void SuggestChaptersForCharacter_ProtagonistWithNoAppearances_ShouldSuggestAllChapters()
    {
        // Arrange
        var project = Project.Create("Test Novel", "Author", "Synopsis");
        var character = project.AddCharacter("Hero", "Main character", CharacterRole.Protagonist);
        var chapters = CreateChapters(project.Id, 5);

        // Act
        var suggestions = _service.SuggestChaptersForCharacter(character, chapters);

        // Assert
        suggestions.Should().HaveCount(5);
        suggestions.Should().Contain(new[] { 1, 2, 3, 4, 5 });
    }

    [Fact]
    public void SuggestChaptersForCharacter_Antagonist_ShouldSuggestKeyChapters()
    {
        // Arrange
        var project = Project.Create("Test Novel", "Author", "Synopsis");
        var character = project.AddCharacter("Villain", "Antagonist", CharacterRole.Antagonist);
        var chapters = CreateChapters(project.Id, 10);

        // Act
        var suggestions = _service.SuggestChaptersForCharacter(character, chapters);

        // Assert
        suggestions.Should().NotBeEmpty();
        suggestions.Should().Contain(2);  // Introduction
        suggestions.Should().Contain(5);  // Middle
        suggestions.Should().Contain(8);  // Climax (75% of 10)
    }

    [Fact]
    public void SuggestChaptersForCharacter_SupportingCharacter_ShouldSuggestKeyMoments()
    {
        // Arrange
        var project = Project.Create("Test Novel", "Author", "Synopsis");
        var character = project.AddCharacter("Mentor", "Supporting character", CharacterRole.Supporting);
        var chapters = CreateChapters(project.Id, 12);

        // Act
        var suggestions = _service.SuggestChaptersForCharacter(character, chapters);

        // Assert
        suggestions.Should().NotBeEmpty();
        suggestions.Should().Contain(3);  // 25% of 12
        suggestions.Should().Contain(6);  // 50% of 12
        suggestions.Should().Contain(9);  // 75% of 12
    }

    [Fact]
    public void ValidateCharacterConsistency_WithNoChapters_ShouldReturnTrue()
    {
        // Arrange
        var project = Project.Create("Test Novel", "Author", "Synopsis");
        var character = project.AddCharacter("Hero", "Main character", CharacterRole.Protagonist);
        var chapters = new List<Chapter>();

        // Act
        var result = _service.ValidateCharacterConsistency(character, chapters);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ValidateCharacterConsistency_WithNullCharacter_ShouldThrowArgumentNullException()
    {
        // Arrange
        var chapters = new List<Chapter>();

        // Act
        var act = () => _service.ValidateCharacterConsistency(null!, chapters);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("character");
    }

    [Fact]
    public void HasAdequatePresence_WithNullCharacter_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () => _service.HasAdequatePresence(null!, 10);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("character");
    }

    [Fact]
    public void SuggestChaptersForCharacter_WithNoChapters_ShouldReturnEmptyList()
    {
        // Arrange
        var project = Project.Create("Test Novel", "Author", "Synopsis");
        var character = project.AddCharacter("Hero", "Main character", CharacterRole.Protagonist);
        var chapters = new List<Chapter>();

        // Act
        var suggestions = _service.SuggestChaptersForCharacter(character, chapters);

        // Assert
        suggestions.Should().BeEmpty();
    }

    [Fact]
    public void SuggestChaptersForCharacter_WithNullCharacter_ShouldThrowArgumentNullException()
    {
        // Arrange
        var chapters = new List<Chapter>();

        // Act
        var act = () => _service.SuggestChaptersForCharacter(null!, chapters);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("character");
    }

    private List<Chapter> CreateChapters(Guid projectId, int count)
    {
        var chapters = new List<Chapter>();
        for (int i = 1; i <= count; i++)
        {
            chapters.Add(Chapter.Create(projectId, $"Chapter {i}", i));
        }
        return chapters;
    }
}
