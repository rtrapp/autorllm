using FluentAssertions;
using AutorLLM.Application.Commands.PlotPoints.DeletePlotPoint;

namespace AutorLLM.Tests.Unit.Application.Commands.PlotPoints;

public class DeletePlotPointCommandValidatorTests
{
    private readonly DeletePlotPointCommandValidator _validator;

    public DeletePlotPointCommandValidatorTests()
    {
        _validator = new DeletePlotPointCommandValidator();
    }

    [Fact]
    public void Should_Pass_When_Command_Is_Valid()
    {
        // Arrange
        var command = new DeletePlotPointCommand
        {
            ProjectId = Guid.NewGuid(),
            PlotId = Guid.NewGuid(),
            PlotPointId = Guid.NewGuid()
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
        var command = new DeletePlotPointCommand
        {
            ProjectId = Guid.Empty,
            PlotId = Guid.NewGuid(),
            PlotPointId = Guid.NewGuid()
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
        var command = new DeletePlotPointCommand
        {
            ProjectId = Guid.NewGuid(),
            PlotId = Guid.Empty,
            PlotPointId = Guid.NewGuid()
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
        var command = new DeletePlotPointCommand
        {
            ProjectId = Guid.NewGuid(),
            PlotId = Guid.NewGuid(),
            PlotPointId = Guid.Empty
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PlotPointId");
    }
}
