using FluentAssertions;
using AutorLLM.Application.Commands.Plots.DeletePlot;

namespace AutorLLM.Tests.Unit.Application.Commands.Plots;

public class DeletePlotCommandValidatorTests
{
    private readonly DeletePlotCommandValidator _validator;

    public DeletePlotCommandValidatorTests()
    {
        _validator = new DeletePlotCommandValidator();
    }

    [Fact]
    public void Should_Pass_When_Command_Is_Valid()
    {
        // Arrange
        var command = new DeletePlotCommand
        {
            ProjectId = Guid.NewGuid(),
            PlotId = Guid.NewGuid()
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
        var command = new DeletePlotCommand
        {
            ProjectId = Guid.Empty,
            PlotId = Guid.NewGuid()
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
        var command = new DeletePlotCommand
        {
            ProjectId = Guid.NewGuid(),
            PlotId = Guid.Empty
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PlotId");
    }
}
