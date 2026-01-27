using AutorLLM.Domain.Entities;
using AutorLLM.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace AutorLLM.Tests.Unit.Domain.Entities;

public class ChapterTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateChapter()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var title = "Chapter 1";
        var order = 1;

        // Act
        var chapter = Chapter.Create(projectId, title, order);

        // Assert
        chapter.Should().NotBeNull();
        chapter.Id.Should().NotBeEmpty();
        chapter.ProjectId.Should().Be(projectId);
        chapter.Title.Should().Be(title);
        chapter.Order.Value.Should().Be(order);
        chapter.Content.Should().BeEmpty();
        chapter.WordCount.Should().Be(0);
    }

    [Fact]
    public void Create_WithEmptyProjectId_ShouldThrowArgumentException()
    {
        // Arrange
        var projectId = Guid.Empty;
        var title = "Chapter 1";
        var order = 1;

        // Act
        var act = () => Chapter.Create(projectId, title, order);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*ProjectId cannot be empty*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithInvalidTitle_ShouldThrowArgumentException(string? title)
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var order = 1;

        // Act
        var act = () => Chapter.Create(projectId, title!, order);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Chapter title cannot be empty*");
    }

    [Fact]
    public void Create_WithInvalidOrder_ShouldThrowArgumentException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var title = "Chapter 1";
        var order = 0;

        // Act
        var act = () => Chapter.Create(projectId, title, order);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Chapter order must be greater than 0*");
    }

    [Fact]
    public void UpdateContent_WithValidData_ShouldUpdateContentAndWordCount()
    {
        // Arrange
        var chapter = Chapter.Create(Guid.NewGuid(), "Chapter 1", 1);
        var content = "This is a test content with multiple words.";

        // Act
        chapter.UpdateContent(content);

        // Assert
        chapter.Content.Should().Be(content);
        chapter.WordCount.Should().Be(8); // "This is a test content with multiple words"
    }

    [Fact]
    public void UpdateContent_WithEmptyString_ShouldHaveZeroWordCount()
    {
        // Arrange
        var chapter = Chapter.Create(Guid.NewGuid(), "Chapter 1", 1);
        chapter.UpdateContent("Some initial content");

        // Act
        chapter.UpdateContent("");

        // Assert
        chapter.Content.Should().BeEmpty();
        chapter.WordCount.Should().Be(0);
    }

    [Fact]
    public void UpdateTitle_WithValidTitle_ShouldUpdateTitle()
    {
        // Arrange
        var chapter = Chapter.Create(Guid.NewGuid(), "Chapter 1", 1);
        var newTitle = "The Beginning";

        // Act
        chapter.UpdateTitle(newTitle);

        // Assert
        chapter.Title.Should().Be(newTitle);
    }

    [Fact]
    public void UpdateTitle_WithEmptyTitle_ShouldThrowArgumentException()
    {
        // Arrange
        var chapter = Chapter.Create(Guid.NewGuid(), "Chapter 1", 1);

        // Act
        var act = () => chapter.UpdateTitle("");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Chapter title cannot be empty*");
    }

    [Fact]
    public void UpdateSummary_WithValidSummary_ShouldUpdateSummary()
    {
        // Arrange
        var chapter = Chapter.Create(Guid.NewGuid(), "Chapter 1", 1);
        var summary = "This chapter introduces the main character.";

        // Act
        chapter.UpdateSummary(summary);

        // Assert
        chapter.Summary.Should().Be(summary);
    }

    [Fact]
    public void UpdateOrder_WithValidOrder_ShouldUpdateOrder()
    {
        // Arrange
        var chapter = Chapter.Create(Guid.NewGuid(), "Chapter 1", 1);
        var newOrder = 5;

        // Act
        chapter.UpdateOrder(newOrder);

        // Assert
        chapter.Order.Value.Should().Be(newOrder);
    }
}
