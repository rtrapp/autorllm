using FluentAssertions;
using AutorLLM.Application.Commands.Plots.UpdatePlot;

namespace AutorLLM.Tests.Unit.Application.Commands.Plots;

public class UpdatePlotCommandValidatorTests
{
    private readonly UpdatePlotCommandValidator _validator;

    public UpdatePlotCommandValidatorTests()
    {
        _validator = new UpdatePlotCommandValidator();
    }

    [Fact]
    public void Should_Pass_When_Command_Is_Valid()
    {
        // Arrange
        var command = new UpdatePlotCommand
        {
            ProjectId = Guid.NewGuid(),
            PlotId = Guid.NewGuid(),
            Title = "The Quest Updated",
            Description = "Updated description",
            Type = "Main",
            Resolution = "Ring destroyed",
            IsActive = true
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
        var command = new UpdatePlotCommand
        {
            ProjectId = Guid.Empty,
            PlotId = Guid.NewGuid(),
            Title = "The Quest",
            Description = "Description",
            Type = "Main"
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
        var command = new UpdatePlotCommand
        {
            ProjectId = Guid.NewGuid(),
            PlotId = Guid.Empty,
            Title = "The Quest",
            Description = "Description",
            Type = "Main"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PlotId");
    }

    [Fact]
    public void Should_Fail_When_Title_Exceeds_Maximum_Length()
    {
        // Arrange
        var command = new UpdatePlotCommand
        {
            ProjectId = Guid.NewGuid(),
            PlotId = Guid.NewGuid(),
            Title = new string('A', 201),
            Description = "Description",
            Type = "Main"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }
}
