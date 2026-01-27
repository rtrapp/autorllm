using AutorLLM.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace AutorLLM.Tests.Unit.Domain.ValueObjects;

public class PlotTypeTests
{
    [Fact]
    public void Create_WithValidValue_ShouldCreatePlotType()
    {
        // Arrange
        var value = "Adventure";

        // Act
        var plotType = PlotType.Create(value);

        // Assert
        plotType.Should().NotBeNull();
        plotType.Value.Should().Be(value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithInvalidValue_ShouldThrowArgumentException(string? value)
    {
        // Act
        var act = () => PlotType.Create(value!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Plot type cannot be empty*");
    }

    [Fact]
    public void Create_WithValueTooLong_ShouldThrowArgumentException()
    {
        // Arrange
        var value = new string('A', 51);

        // Act
        var act = () => PlotType.Create(value);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Plot type cannot exceed 50 characters*");
    }

    [Fact]
    public void Equals_WithSameValue_ShouldReturnTrue()
    {
        // Arrange
        var plotType1 = PlotType.Create("Mystery");
        var plotType2 = PlotType.Create("Mystery");

        // Act & Assert
        plotType1.Should().Be(plotType2);
        (plotType1 == plotType2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentValue_ShouldReturnFalse()
    {
        // Arrange
        var plotType1 = PlotType.Create("Mystery");
        var plotType2 = PlotType.Create("Romance");

        // Act & Assert
        plotType1.Should().NotBe(plotType2);
        (plotType1 != plotType2).Should().BeTrue();
    }

    [Fact]
    public void PredefinedPlotTypes_ShouldBeAvailable()
    {
        // Assert
        PlotType.Main.Value.Should().Be("Main");
        PlotType.Subplot.Value.Should().Be("Subplot");
        PlotType.Character.Value.Should().Be("Character Arc");
        PlotType.Romance.Value.Should().Be("Romance");
        PlotType.Mystery.Value.Should().Be("Mystery");
    }

    [Fact]
    public void GetHashCode_WithSameValue_ShouldReturnSameHash()
    {
        // Arrange
        var plotType1 = PlotType.Create("Thriller");
        var plotType2 = PlotType.Create("Thriller");

        // Act & Assert
        plotType1.GetHashCode().Should().Be(plotType2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var value = "Horror";
        var plotType = PlotType.Create(value);

        // Act
        var result = plotType.ToString();

        // Assert
        result.Should().Be(value);
    }
}
