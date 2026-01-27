using FluentAssertions;
using AutorLLM.Application.Commands.Plots.CreatePlot;

namespace AutorLLM.Tests.Unit.Application.Commands.Plots;

public class CreatePlotCommandValidatorTests
{
    private readonly CreatePlotCommandValidator _validator;

    public CreatePlotCommandValidatorTests()
    {
        _validator = new CreatePlotCommandValidator();
    }

    [Fact]
    public void Should_Pass_When_Command_Is_Valid()
    {
        // Arrange
        var command = new CreatePlotCommand
        {
            ProjectId = Guid.NewGuid(),
            Title = "The Quest",
            Description = "A journey to destroy the ring",
            Type = "Main"
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
        var command = new CreatePlotCommand
        {
            ProjectId = Guid.Empty,
            Title = "The Quest",
            Description = "A journey to destroy the ring",
            Type = "Main"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ProjectId" && e.ErrorMessage == "ProjectId is required");
    }

    [Fact]
    public void Should_Fail_When_Title_Is_Empty()
    {
        // Arrange
        var command = new CreatePlotCommand
        {
            ProjectId = Guid.NewGuid(),
            Title = "",
            Description = "A journey to destroy the ring",
            Type = "Main"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title" && e.ErrorMessage == "Title is required");
    }

    [Fact]
    public void Should_Fail_When_Title_Exceeds_Maximum_Length()
    {
        // Arrange
        var command = new CreatePlotCommand
        {
            ProjectId = Guid.NewGuid(),
            Title = new string('A', 201),
            Description = "A journey to destroy the ring",
            Type = "Main"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title" && e.ErrorMessage == "Title must be under 200 characters");
    }

    [Fact]
    public void Should_Fail_When_Description_Exceeds_Maximum_Length()
    {
        // Arrange
        var command = new CreatePlotCommand
        {
            ProjectId = Guid.NewGuid(),
            Title = "The Quest",
            Description = new string('A', 2001),
            Type = "Main"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description" && e.ErrorMessage == "Description must be under 2000 characters");
    }

    [Fact]
    public void Should_Fail_When_Type_Is_Empty()
    {
        // Arrange
        var command = new CreatePlotCommand
        {
            ProjectId = Guid.NewGuid(),
            Title = "The Quest",
            Description = "A journey to destroy the ring",
            Type = ""
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Type" && e.ErrorMessage == "Type is required");
    }

    [Fact]
    public void Should_Fail_When_Type_Exceeds_Maximum_Length()
    {
        // Arrange
        var command = new CreatePlotCommand
        {
            ProjectId = Guid.NewGuid(),
            Title = "The Quest",
            Description = "A journey to destroy the ring",
            Type = new string('A', 51)
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Type" && e.ErrorMessage == "Type must be under 50 characters");
    }
}
