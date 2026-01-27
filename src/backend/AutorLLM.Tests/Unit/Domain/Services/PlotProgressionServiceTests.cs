using AutorLLM.Domain.Aggregates.ProjectAggregate;
using AutorLLM.Domain.Entities;
using AutorLLM.Domain.Services;
using AutorLLM.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace AutorLLM.Tests.Unit.Domain.Services;

public class PlotProgressionServiceTests
{
    private readonly PlotProgressionService _service;

    public PlotProgressionServiceTests()
    {
        _service = new PlotProgressionService();
    }

    [Fact]
    public void ValidatePlotProgression_WithValidProgression_ShouldReturnTrue()
    {
        // Arrange
        var project = Project.Create("Test Novel", "Author", "Synopsis");
        var plot = project.AddPlot("Main Plot", "Description", PlotType.Main);
        
        var chapters = new List<Chapter>
        {
            Chapter.Create(project.Id, "Chapter 1", 1),
            Chapter.Create(project.Id, "Chapter 2", 2),
            Chapter.Create(project.Id, "Chapter 3", 3),
            Chapter.Create(project.Id, "Chapter 4", 4),
            Chapter.Create(project.Id, "Chapter 5", 5),
            Chapter.Create(project.Id, "Chapter 6", 6),
            Chapter.Create(project.Id, "Chapter 7", 7),
            Chapter.Create(project.Id, "Chapter 8", 8)
        };

        // Add plot points with proper progression
        var pp1 = PlotPoint.Create(plot.Id, chapters[0].Id, "Beginning", 2, 0);
        var pp2 = PlotPoint.Create(plot.Id, chapters[2].Id, "Rising", 5, 1);
        var pp3 = PlotPoint.Create(plot.Id, chapters[4].Id, "Climax", 10, 2);
        var pp4 = PlotPoint.Create(plot.Id, chapters[6].Id, "Resolution", 3, 3);
        
        plot.AddPlotPoint(pp1);
        plot.AddPlotPoint(pp2);
        plot.AddPlotPoint(pp3);
        plot.AddPlotPoint(pp4);

        // Act
        var result = _service.ValidatePlotProgression(plot, chapters);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ValidatePlotProgression_WithTooFewPoints_ShouldReturnFalse()
    {
        // Arrange
        var project = Project.Create("Test Novel", "Author", "Synopsis");
        var plot = project.AddPlot("Main Plot", "Description", PlotType.Main);
        
        var chapters = new List<Chapter>
        {
            Chapter.Create(project.Id, "Chapter 1", 1),
            Chapter.Create(project.Id, "Chapter 2", 2)
        };

        // Only 2 plot points (needs at least 3)
        var pp1 = PlotPoint.Create(plot.Id, chapters[0].Id, "Beginning", 2, 0);
        var pp2 = PlotPoint.Create(plot.Id, chapters[1].Id, "End", 5, 1);
        
        plot.AddPlotPoint(pp1);
        plot.AddPlotPoint(pp2);

        // Act
        var result = _service.ValidatePlotProgression(plot, chapters);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ValidatePlotProgression_WithNoChapters_ShouldReturnFalse()
    {
        // Arrange
        var project = Project.Create("Test Novel", "Author", "Synopsis");
        var plot = project.AddPlot("Main Plot", "Description", PlotType.Main);
        var chapters = new List<Chapter>();

        // Act
        var result = _service.ValidatePlotProgression(plot, chapters);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CalculateIntensityProgression_WithMultiplePoints_ShouldReturnNormalizedValues()
    {
        // Arrange
        var project = Project.Create("Test Novel", "Author", "Synopsis");
        var plot = project.AddPlot("Main Plot", "Description", PlotType.Main);
        var chapter = Chapter.Create(project.Id, "Chapter", 1);
        
        var pp1 = PlotPoint.Create(plot.Id, chapter.Id, "Beginning", 1, 0);  // 1/10 = 10%
        var pp2 = PlotPoint.Create(plot.Id, chapter.Id, "Rising", 5, 1);     // 5/10 = 50%
        var pp3 = PlotPoint.Create(plot.Id, chapter.Id, "Climax", 10, 2);    // 10/10 = 100%
        var pp4 = PlotPoint.Create(plot.Id, chapter.Id, "Falling", 3, 3);    // 3/10 = 30%
        
        plot.AddPlotPoint(pp1);
        plot.AddPlotPoint(pp2);
        plot.AddPlotPoint(pp3);
        plot.AddPlotPoint(pp4);

        // Act
        var result = _service.CalculateIntensityProgression(plot);

        // Assert
        result.Should().HaveCount(4);
        result[0].Should().Be(10.0);
        result[1].Should().Be(50.0);
        result[2].Should().Be(100.0);
        result[3].Should().Be(30.0);
    }

    [Fact]
    public void CalculateIntensityProgression_WithNoPoints_ShouldReturnEmptyDictionary()
    {
        // Arrange
        var project = Project.Create("Test Novel", "Author", "Synopsis");
        var plot = project.AddPlot("Main Plot", "Description", PlotType.Main);

        // Act
        var result = _service.CalculateIntensityProgression(plot);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void FindClimaxChapter_WithMultiplePoints_ShouldReturnHighestIntensity()
    {
        // Arrange
        var project = Project.Create("Test Novel", "Author", "Synopsis");
        var plot = project.AddPlot("Main Plot", "Description", PlotType.Main);
        var chapter = Chapter.Create(project.Id, "Chapter", 1);
        
        var pp1 = PlotPoint.Create(plot.Id, chapter.Id, "Beginning", 2, 0);
        var pp2 = PlotPoint.Create(plot.Id, chapter.Id, "Rising", 5, 1);
        var pp3 = PlotPoint.Create(plot.Id, chapter.Id, "Climax", 10, 2);
        var pp4 = PlotPoint.Create(plot.Id, chapter.Id, "Falling", 3, 3);
        
        plot.AddPlotPoint(pp1);
        plot.AddPlotPoint(pp2);
        plot.AddPlotPoint(pp3);
        plot.AddPlotPoint(pp4);

        // Act
        var climax = _service.FindClimaxChapter(plot);

        // Assert
        climax.Should().Be(2); // Order of the climax point
    }

    [Fact]
    public void FindClimaxChapter_WithNoPoints_ShouldReturnNull()
    {
        // Arrange
        var project = Project.Create("Test Novel", "Author", "Synopsis");
        var plot = project.AddPlot("Main Plot", "Description", PlotType.Main);

        // Act
        var climax = _service.FindClimaxChapter(plot);

        // Assert
        climax.Should().BeNull();
    }

    [Fact]
    public void ValidatePlotProgression_WithNullPlot_ShouldThrowArgumentNullException()
    {
        // Arrange
        var chapters = new List<Chapter>();

        // Act
        var act = () => _service.ValidatePlotProgression(null!, chapters);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("plot");
    }

    [Fact]
    public void CalculateIntensityProgression_WithNullPlot_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () => _service.CalculateIntensityProgression(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("plot");
    }
}
