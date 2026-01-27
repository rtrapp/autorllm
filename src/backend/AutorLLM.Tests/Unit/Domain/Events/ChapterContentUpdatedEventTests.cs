using AutorLLM.Domain.Events;
using FluentAssertions;
using Xunit;

namespace AutorLLM.Tests.Unit.Domain.Events;

public class ChapterContentUpdatedEventTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateEvent()
    {
        // Arrange
        var chapterId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var wordCount = 1500;

        // Act
        var domainEvent = new ChapterContentUpdatedEvent(chapterId, projectId, wordCount);

        // Assert
        domainEvent.Should().NotBeNull();
        domainEvent.ChapterId.Should().Be(chapterId);
        domainEvent.ProjectId.Should().Be(projectId);
        domainEvent.WordCount.Should().Be(wordCount);
        domainEvent.EventId.Should().NotBeEmpty();
        domainEvent.OccurredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_ShouldInheritFromDomainEvent()
    {
        // Arrange
        var chapterId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var wordCount = 1500;

        // Act
        var domainEvent = new ChapterContentUpdatedEvent(chapterId, projectId, wordCount);

        // Assert
        domainEvent.Should().BeAssignableTo<AutorLLM.Domain.Common.DomainEvent>();
    }

    [Fact]
    public void Create_WithDifferentWordCounts_ShouldPreserveValues()
    {
        // Arrange
        var chapterId = Guid.NewGuid();
        var projectId = Guid.NewGuid();

        // Act
        var event1 = new ChapterContentUpdatedEvent(chapterId, projectId, 100);
        var event2 = new ChapterContentUpdatedEvent(chapterId, projectId, 5000);

        // Assert
        event1.WordCount.Should().Be(100);
        event2.WordCount.Should().Be(5000);
    }
}
