using FluentAssertions;
using AutorLLM.Application.Commands.PlotPoints.UpdatePlotPoint;

namespace AutorLLM.Tests.Unit.Application.Commands.PlotPoints;

public class UpdatePlotPointCommandValidatorTests
{
    private readonly UpdatePlotPointCommandValidator _validator;

    public UpdatePlotPointCommandValidatorTests()
    {
        _validator = new UpdatePlotPointCommandValidator();
    }

    [Fact]
    public void Should_Pass_When_Command_Is_Valid()
    {
        // Arrange
        var command = new UpdatePlotPointCommand
        {
            ProjectId = Guid.NewGuid(),
            PlotId = Guid.NewGuid(),
            PlotPointId = Guid.NewGuid(),
            Description = "Updated moment",
            Intensity = 8
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
        var command = new UpdatePlotPointCommand
        {
            ProjectId = Guid.Empty,
            PlotId = Guid.NewGuid(),
            PlotPointId = Guid.NewGuid(),
            Description = "Updated moment",
            Intensity = 8
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
        var command = new UpdatePlotPointCommand
        {
            ProjectId = Guid.NewGuid(),
            PlotId = Guid.Empty,
            PlotPointId = Guid.NewGuid(),
            Description = "Updated moment",
            Intensity = 8
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PlotId");
    }

    [Fact]
    public void Should_Fail_When_PlotPointId_Is_Empty()
    {
        // Arrange
        var command = new UpdatePlotPointCommand
        {
            ProjectId = Guid.NewGuid(),
            PlotId = Guid.NewGuid(),
            PlotPointId = Guid.Empty,
            Description = "Updated moment",
            Intensity = 8
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PlotPointId");
    }

    [Fact]
    public void Should_Fail_When_Description_Is_Empty()
    {
        // Arrange
        var command = new UpdatePlotPointCommand
        {
            ProjectId = Guid.NewGuid(),
            PlotId = Guid.NewGuid(),
            PlotPointId = Guid.NewGuid(),
            Description = "",
            Intensity = 8
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
        var command = new UpdatePlotPointCommand
        {
            ProjectId = Guid.NewGuid(),
            PlotId = Guid.NewGuid(),
            PlotPointId = Guid.NewGuid(),
            Description = new string('A', 501),
            Intensity = 8
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
        var command = new UpdatePlotPointCommand
        {
            ProjectId = Guid.NewGuid(),
            PlotId = Guid.NewGuid(),
            PlotPointId = Guid.NewGuid(),
            Description = "Updated moment",
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
        var command = new UpdatePlotPointCommand
        {
            ProjectId = Guid.NewGuid(),
            PlotId = Guid.NewGuid(),
            PlotPointId = Guid.NewGuid(),
            Description = "Updated moment",
            Intensity = 11
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Intensity");
    }
}
