using FluentAssertions;
using AutorLLM.Application.Commands.PlotPoints.CreatePlotPoint;

namespace AutorLLM.Tests.Unit.Application.Commands.PlotPoints;

public class CreatePlotPointCommandValidatorTests
{
    private readonly CreatePlotPointCommandValidator _validator;

    public CreatePlotPointCommandValidatorTests()
    {
        _validator = new CreatePlotPointCommandValidator();
    }

    [Fact]
    public void Should_Pass_When_Command_Is_Valid()
    {
        // Arrange
        var command = new CreatePlotPointCommand
        {
            ProjectId = Guid.NewGuid(),
            PlotId = Guid.NewGuid(),
            ChapterId = Guid.NewGuid(),
            Description = "Climax moment",
            Intensity = 10
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Should_Fail_When_ProjectId_Is_Empty()
    {
        // Arrange
        var command = new CreatePlotPointCommand
        {
            ProjectId = Guid.Empty,
            PlotId = Guid.NewGuid(),
            ChapterId = Guid.NewGuid(),
            Description = "Climax moment",
            Intensity = 10
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ProjectId");
    }

    [Fact]
    public void Should_Fail_When_PlotId_Is_Empty()
    {
        // Arrange
        var command = new CreatePlotPointCommand
        {
            ProjectId = Guid.NewGuid(),
            PlotId = Guid.Empty,
            ChapterId = Guid.NewGuid(),
            Description = "Climax moment",
            Intensity = 10
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PlotId");
    }

    [Fact]
    public void Should_Fail_When_ChapterId_Is_Empty()
    {
        // Arrange
        var command = new CreatePlotPointCommand
        {
            ProjectId = Guid.NewGuid(),
            PlotId = Guid.NewGuid(),
            ChapterId = Guid.Empty,
            Description = "Climax moment",
            Intensity = 10
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ChapterId");
    }

    [Fact]
    public void Should_Fail_When_Description_Is_Empty()
    {
        // Arrange
        var command = new CreatePlotPointCommand
        {
            ProjectId = Guid.NewGuid(),
            PlotId = Guid.NewGuid(),
            ChapterId = Guid.NewGuid(),
            Description = "",
            Intensity = 10
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }

    [Fact]
    public void Should_Fail_When_Description_Exceeds_Maximum_Length()
    {
        // Arrange
        var command = new CreatePlotPointCommand
        {
            ProjectId = Guid.NewGuid(),
            PlotId = Guid.NewGuid(),
            ChapterId = Guid.NewGuid(),
            Description = new string('A', 501),
            Intensity = 10
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }

    [Fact]
    public void Should_Fail_When_Intensity_Is_Below_Minimum()
    {
        // Arrange
        var command = new CreatePlotPointCommand
        {
            ProjectId = Guid.NewGuid(),
            PlotId = Guid.NewGuid(),
            ChapterId = Guid.NewGuid(),
            Description = "Climax moment",
            Intensity = -1
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Intensity");
    }

    [Fact]
    public void Should_Fail_When_Intensity_Is_Above_Maximum()
    {
        // Arrange
        var command = new CreatePlotPointCommand
        {
            ProjectId = Guid.NewGuid(),
            PlotId = Guid.NewGuid(),
            ChapterId = Guid.NewGuid(),
            Description = "Climax moment",
            Intensity = 11
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Intensity");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(10)]
    public void Should_Pass_When_Intensity_Is_Within_Valid_Range(int intensity)
    {
        // Arrange
        var command = new CreatePlotPointCommand
        {
            ProjectId = Guid.NewGuid(),
            PlotId = Guid.NewGuid(),
            ChapterId = Guid.NewGuid(),
            Description = "Climax moment",
            Intensity = intensity
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
