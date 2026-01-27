using AutorLLM.Domain.Events;
using FluentAssertions;
using Xunit;

namespace AutorLLM.Tests.Unit.Domain.Events;

public class CharacterCreatedEventTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateEvent()
    {
        // Arrange
        var characterId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var name = "Hero";

        // Act
        var domainEvent = new CharacterCreatedEvent(characterId, projectId, name);

        // Assert
        domainEvent.Should().NotBeNull();
        domainEvent.CharacterId.Should().Be(characterId);
        domainEvent.ProjectId.Should().Be(projectId);
        domainEvent.Name.Should().Be(name);
        domainEvent.EventId.Should().NotBeEmpty();
        domainEvent.OccurredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_ShouldInheritFromDomainEvent()
    {
        // Arrange
        var characterId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var name = "Hero";

        // Act
        var domainEvent = new CharacterCreatedEvent(characterId, projectId, name);

        // Assert
        domainEvent.Should().BeAssignableTo<AutorLLM.Domain.Common.DomainEvent>();
    }

    [Fact]
    public void Create_WithDifferentNames_ShouldPreserveValues()
    {
        // Arrange
        var characterId1 = Guid.NewGuid();
        var characterId2 = Guid.NewGuid();
        var projectId = Guid.NewGuid();

        // Act
        var event1 = new CharacterCreatedEvent(characterId1, projectId, "Hero");
        var event2 = new CharacterCreatedEvent(characterId2, projectId, "Villain");

        // Assert
        event1.Name.Should().Be("Hero");
        event2.Name.Should().Be("Villain");
        event1.CharacterId.Should().NotBe(event2.CharacterId);
    }

    [Fact]
    public void Create_MultipleEvents_ShouldHaveUniqueEventIds()
    {
        // Arrange
        var characterId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var name = "Hero";

        // Act
        var event1 = new CharacterCreatedEvent(characterId, projectId, name);
        var event2 = new CharacterCreatedEvent(characterId, projectId, name);

        // Assert
        event1.EventId.Should().NotBe(event2.EventId);
    }
}
