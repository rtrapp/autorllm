using AutorLLM.Domain.Common;
using FluentAssertions;
using Xunit;

namespace AutorLLM.Tests.Unit.Domain.Common;

public class DomainEventTests
{
    private class TestDomainEvent : DomainEvent
    {
        public string TestData { get; }

        public TestDomainEvent(string testData)
        {
            TestData = testData;
        }
    }

    [Fact]
    public void DomainEvent_WhenCreated_ShouldHaveEventId()
    {
        // Act
        var domainEvent = new TestDomainEvent("test");

        // Assert
        domainEvent.EventId.Should().NotBeEmpty();
    }

    [Fact]
    public void DomainEvent_WhenCreated_ShouldHaveOccurredAt()
    {
        // Act
        var before = DateTime.UtcNow;
        var domainEvent = new TestDomainEvent("test");
        var after = DateTime.UtcNow;

        // Assert
        domainEvent.OccurredAt.Should().BeOnOrAfter(before);
        domainEvent.OccurredAt.Should().BeOnOrBefore(after);
    }

    [Fact]
    public void DomainEvent_WhenCreatedMultipleTimes_ShouldHaveUniqueEventIds()
    {
        // Act
        var event1 = new TestDomainEvent("test1");
        var event2 = new TestDomainEvent("test2");

        // Assert
        event1.EventId.Should().NotBe(event2.EventId);
    }

    [Fact]
    public void DomainEvent_ShouldPreserveCustomProperties()
    {
        // Arrange
        var testData = "test data";

        // Act
        var domainEvent = new TestDomainEvent(testData);

        // Assert
        domainEvent.TestData.Should().Be(testData);
    }
}
