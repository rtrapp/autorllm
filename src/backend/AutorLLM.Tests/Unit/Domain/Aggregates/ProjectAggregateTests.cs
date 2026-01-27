using AutorLLM.Domain.Aggregates.ProjectAggregate;
using AutorLLM.Domain.Exceptions;
using AutorLLM.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace AutorLLM.Tests.Unit.Domain.Aggregates;

public class ProjectAggregateTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateProject()
    {
        // Arrange
        var title = "My Novel";
        var author = "John Doe";
        var synopsis = "A story about...";

        // Act
        var project = Project.Create(title, author, synopsis);

        // Assert
        project.Should().NotBeNull();
        project.Id.Should().NotBeEmpty();
        project.Title.Should().Be(title);
        project.Author.Should().Be(author);
        project.Synopsis.Should().Be(synopsis);
        project.TargetWordCount.Should().Be(50000);
        project.Characters.Should().BeEmpty();
        project.Chapters.Should().BeEmpty();
        project.Plots.Should().BeEmpty();
        project.Locations.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithInvalidTitle_ShouldThrowArgumentException(string? title)
    {
        // Arrange
        var author = "John Doe";
        var synopsis = "A story about...";

        // Act
        var act = () => Project.Create(title!, author, synopsis);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Project title cannot be empty*");
    }

    [Fact]
    public void AddCharacter_WithValidData_ShouldAddCharacterToProject()
    {
        // Arrange
        var project = Project.Create("My Novel", "John Doe", "A story");
        var characterName = "Hero";
        var description = "The main hero";
        var role = CharacterRole.Protagonist;

        // Act
        var character = project.AddCharacter(characterName, description, role);

        // Assert
        project.Characters.Should().HaveCount(1);
        project.Characters.Should().Contain(character);
        character.Name.Should().Be(characterName);
        character.ProjectId.Should().Be(project.Id);
    }

    [Fact]
    public void AddCharacter_WithDuplicateName_ShouldThrowDuplicateCharacterNameException()
    {
        // Arrange
        var project = Project.Create("My Novel", "John Doe", "A story");
        var characterName = "Hero";
        project.AddCharacter(characterName, "Description", CharacterRole.Protagonist);

        // Act
        var act = () => project.AddCharacter(characterName, "Another description", CharacterRole.Supporting);

        // Assert
        act.Should().Throw<DuplicateCharacterNameException>()
            .WithMessage($"*{characterName}*");
    }

    [Fact]
    public void RemoveCharacter_WithExistingCharacter_ShouldRemoveCharacter()
    {
        // Arrange
        var project = Project.Create("My Novel", "John Doe", "A story");
        var character = project.AddCharacter("Hero", "Description", CharacterRole.Protagonist);

        // Act
        project.RemoveCharacter(character.Id);

        // Assert
        project.Characters.Should().BeEmpty();
    }

    [Fact]
    public void RemoveCharacter_WithNonExistingCharacter_ShouldThrowCharacterNotFoundException()
    {
        // Arrange
        var project = Project.Create("My Novel", "John Doe", "A story");
        var nonExistingId = Guid.NewGuid();

        // Act
        var act = () => project.RemoveCharacter(nonExistingId);

        // Assert
        act.Should().Throw<CharacterNotFoundException>();
    }

    [Fact]
    public void AddChapter_WithValidTitle_ShouldAddChapterWithCorrectOrder()
    {
        // Arrange
        var project = Project.Create("My Novel", "John Doe", "A story");

        // Act
        var chapter1 = project.AddChapter("Chapter 1");
        var chapter2 = project.AddChapter("Chapter 2");
        var chapter3 = project.AddChapter("Chapter 3");

        // Assert
        project.Chapters.Should().HaveCount(3);
        chapter1.Order.Value.Should().Be(1);
        chapter2.Order.Value.Should().Be(2);
        chapter3.Order.Value.Should().Be(3);
    }

    [Fact]
    public void ReorderChapters_WithValidIds_ShouldReorderChapters()
    {
        // Arrange
        var project = Project.Create("My Novel", "John Doe", "A story");
        var chapter1 = project.AddChapter("Chapter 1");
        var chapter2 = project.AddChapter("Chapter 2");
        var chapter3 = project.AddChapter("Chapter 3");

        var newOrder = new List<Guid> { chapter3.Id, chapter1.Id, chapter2.Id };

        // Act
        project.ReorderChapters(newOrder);

        // Assert
        chapter3.Order.Value.Should().Be(1);
        chapter1.Order.Value.Should().Be(2);
        chapter2.Order.Value.Should().Be(3);
    }

    [Fact]
    public void ReorderChapters_WithIncompleteList_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var project = Project.Create("My Novel", "John Doe", "A story");
        var chapter1 = project.AddChapter("Chapter 1");
        var chapter2 = project.AddChapter("Chapter 2");
        var chapter3 = project.AddChapter("Chapter 3");

        var incompleteOrder = new List<Guid> { chapter1.Id, chapter2.Id }; // Missing chapter3

        // Act
        var act = () => project.ReorderChapters(incompleteOrder);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*All chapters must be included in reordering*");
    }

    [Fact]
    public void RemoveChapter_ShouldRecalculateWordCount()
    {
        // Arrange
        var project = Project.Create("My Novel", "John Doe", "A story");
        var chapter1 = project.AddChapter("Chapter 1");
        var chapter2 = project.AddChapter("Chapter 2");

        chapter1.UpdateContent("This has five words exactly");
        chapter2.UpdateContent("This has four words");

        // Act
        project.RemoveChapter(chapter1.Id);

        // Assert
        project.Chapters.Should().HaveCount(1);
        chapter2.Order.Value.Should().Be(1); // Should be reordered
    }

    [Fact]
    public void AddPlot_WithValidData_ShouldAddPlotToProject()
    {
        // Arrange
        var project = Project.Create("My Novel", "John Doe", "A story");
        var plotTitle = "Main Quest";
        var description = "The hero's journey";
        var plotType = PlotType.Main;

        // Act
        var plot = project.AddPlot(plotTitle, description, plotType);

        // Assert
        project.Plots.Should().HaveCount(1);
        project.Plots.Should().Contain(plot);
        plot.Title.Should().Be(plotTitle);
        plot.Type.Should().Be(plotType);
    }

    [Fact]
    public void GetActivePlots_ShouldReturnOnlyActivePlots()
    {
        // Arrange
        var project = Project.Create("My Novel", "John Doe", "A story");
        var plot1 = project.AddPlot("Plot 1", "Active plot", PlotType.Main);
        var plot2 = project.AddPlot("Plot 2", "Inactive plot", PlotType.Subplot);
        
        plot2.Deactivate();

        // Act
        var activePlots = project.GetActivePlots();

        // Assert
        activePlots.Should().HaveCount(1);
        activePlots.Should().Contain(plot1);
        activePlots.Should().NotContain(plot2);
    }

    [Fact]
    public void AddLocation_WithValidData_ShouldAddLocationToProject()
    {
        // Arrange
        var project = Project.Create("My Novel", "John Doe", "A story");
        var locationName = "Rivendell";
        var description = "An elven outpost";

        // Act
        var location = project.AddLocation(locationName, description);

        // Assert
        project.Locations.Should().HaveCount(1);
        project.Locations.Should().Contain(location);
        location.Name.Should().Be(locationName);
    }

    [Fact]
    public void UpdateTitle_WithValidTitle_ShouldUpdateTitle()
    {
        // Arrange
        var project = Project.Create("My Novel", "John Doe", "A story");
        var newTitle = "My Epic Novel";

        // Act
        project.UpdateTitle(newTitle);

        // Assert
        project.Title.Should().Be(newTitle);
    }

    [Fact]
    public void SetTargetWordCount_WithValidCount_ShouldUpdateTarget()
    {
        // Arrange
        var project = Project.Create("My Novel", "John Doe", "A story");
        var targetWordCount = 100000;

        // Act
        project.SetTargetWordCount(targetWordCount);

        // Assert
        project.TargetWordCount.Should().Be(targetWordCount);
    }
}
