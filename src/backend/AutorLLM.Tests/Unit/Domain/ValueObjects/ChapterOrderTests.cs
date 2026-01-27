using AutorLLM.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace AutorLLM.Tests.Unit.Domain.ValueObjects;

public class ChapterOrderTests
{
    [Fact]
    public void Create_WithValidOrder_ShouldCreateChapterOrder()
    {
        // Arrange
        var value = 5;

        // Act
        var order = ChapterOrder.Create(value);

        // Assert
        order.Should().NotBeNull();
        order.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Create_WithInvalidOrder_ShouldThrowArgumentException(int value)
    {
        // Act
        var act = () => ChapterOrder.Create(value);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Chapter order must be greater than 0*");
    }

    [Fact]
    public void Next_ShouldReturnNextOrder()
    {
        // Arrange
        var order = ChapterOrder.Create(5);

        // Act
        var nextOrder = order.Next();

        // Assert
        nextOrder.Value.Should().Be(6);
    }

    [Fact]
    public void Previous_ShouldReturnPreviousOrder()
    {
        // Arrange
        var order = ChapterOrder.Create(5);

        // Act
        var previousOrder = order.Previous();

        // Assert
        previousOrder.Value.Should().Be(4);
    }

    [Fact]
    public void Previous_OnFirstChapter_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var order = ChapterOrder.Create(1);

        // Act
        var act = () => order.Previous();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot get previous order of the first chapter*");
    }

    [Fact]
    public void Equals_WithSameValue_ShouldReturnTrue()
    {
        // Arrange
        var order1 = ChapterOrder.Create(5);
        var order2 = ChapterOrder.Create(5);

        // Act & Assert
        order1.Should().Be(order2);
        (order1 == order2).Should().BeTrue();
    }

    [Fact]
    public void ImplicitConversion_ToInt_ShouldWork()
    {
        // Arrange
        var order = ChapterOrder.Create(5);

        // Act
        int value = order;

        // Assert
        value.Should().Be(5);
    }
}
