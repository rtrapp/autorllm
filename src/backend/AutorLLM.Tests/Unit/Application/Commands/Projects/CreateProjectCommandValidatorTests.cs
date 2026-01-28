using FluentAssertions;
using AutorLLM.Application.Commands.Projects.CreateProject;
using AutorLLM.Domain.Interfaces;
using Moq;
using AutorLLM.Domain.Aggregates.ProjectAggregate;

namespace AutorLLM.Tests.Unit.Application.Commands.Projects;

public class CreateProjectCommandValidatorTests
{
    private readonly CreateProjectCommandValidator _validator;
    private readonly Mock<IProjectRepository> _mockRepository;

    public CreateProjectCommandValidatorTests()
    {
        _mockRepository = new Mock<IProjectRepository>();
        
        // Setup default behavior: no duplicate titles
        _mockRepository
            .Setup(x => x.GetByTitleAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);
        
        _validator = new CreateProjectCommandValidator(_mockRepository.Object);
    }

    [Fact]
    public async Task Should_Pass_When_Command_Is_Valid()
    {
        // Arrange
        var command = new CreateProjectCommand
        {
            Title = "My Novel",
            Author = "John Doe",
            Synopsis = "A great story about adventures"
        };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_Fail_When_Title_Is_Empty()
    {
        // Arrange
        var command = new CreateProjectCommand
        {
            Title = "",
            Author = "John Doe",
            Synopsis = "A great story"
        };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title" && e.ErrorMessage == "Title is required");
    }

    [Fact]
    public async Task Should_Fail_When_Title_Exceeds_Maximum_Length()
    {
        // Arrange
        var command = new CreateProjectCommand
        {
            Title = new string('a', 201),
            Author = "John Doe",
            Synopsis = "A great story"
        };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title" && e.ErrorMessage == "Title must be under 200 characters");
    }

    [Fact]
    public async Task Should_Fail_When_Author_Is_Empty()
    {
        // Arrange
        var command = new CreateProjectCommand
        {
            Title = "My Novel",
            Author = "",
            Synopsis = "A great story"
        };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Author" && e.ErrorMessage == "Author is required");
    }

    [Fact]
    public async Task Should_Fail_When_Author_Exceeds_Maximum_Length()
    {
        // Arrange
        var command = new CreateProjectCommand
        {
            Title = "My Novel",
            Author = new string('a', 101),
            Synopsis = "A great story"
        };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Author" && e.ErrorMessage == "Author must be under 100 characters");
    }

    [Fact]
    public async Task Should_Fail_When_Synopsis_Exceeds_Maximum_Length()
    {
        // Arrange
        var command = new CreateProjectCommand
        {
            Title = "My Novel",
            Author = "John Doe",
            Synopsis = new string('a', 2001)
        };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Synopsis" && e.ErrorMessage == "Synopsis must be under 2000 characters");
    }

    [Fact]
    public async Task Should_Pass_When_Synopsis_Is_Empty()
    {
        // Arrange
        var command = new CreateProjectCommand
        {
            Title = "My Novel",
            Author = "John Doe",
            Synopsis = ""
        };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Should_Fail_When_Title_Already_Exists()
    {
        // Arrange
        var existingProject = Project.Create("Duplicate Title", "Jane Doe", "Existing synopsis");
        
        _mockRepository
            .Setup(x => x.GetByTitleAsync("Duplicate Title", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProject);

        var command = new CreateProjectCommand
        {
            Title = "Duplicate Title",
            Author = "John Doe",
            Synopsis = "New synopsis"
        };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Title" && 
            e.ErrorMessage == "A project with this title already exists");
    }
}
