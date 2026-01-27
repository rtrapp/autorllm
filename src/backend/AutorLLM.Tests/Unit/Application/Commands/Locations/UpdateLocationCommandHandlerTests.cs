using FluentAssertions;
using AutorLLM.Application.Commands.Locations.UpdateLocation;
using AutorLLM.Domain.Aggregates.ProjectAggregate;
using AutorLLM.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace AutorLLM.Tests.Unit.Application.Commands.Locations;

public class UpdateLocationCommandHandlerTests
{
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<UpdateLocationCommandHandler>> _loggerMock;
    private readonly UpdateLocationCommandHandler _handler;

    public UpdateLocationCommandHandlerTests()
    {
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<UpdateLocationCommandHandler>>();
        _handler = new UpdateLocationCommandHandler(
            _projectRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Update_Location_Successfully()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = Project.Create("Test Project", "Test Author", "Test Synopsis");
        var location = project.AddLocation("Original Name", "Original Description");
        
        var command = new UpdateLocationCommand
        {
            ProjectId = projectId,
            LocationId = location.Id,
            Name = "Updated Name",
            Description = "Updated Description",
            Geography = "Mountains and valleys",
            Culture = "Elven culture",
            Significance = "Key meeting point"
        };

        _projectRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        location.Name.Should().Be("Updated Name");
        location.Description.Should().Be("Updated Description");
        location.Geography.Should().Be("Mountains and valleys");
        location.Culture.Should().Be("Elven culture");
        location.Significance.Should().Be("Key meeting point");
        
        _projectRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()),
            Times.Once);
        
        _unitOfWorkMock.Verify(
            x => x.CommitAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Project_Not_Found()
    {
        // Arrange
        var command = new UpdateLocationCommand
        {
            ProjectId = Guid.NewGuid(),
            LocationId = Guid.NewGuid(),
            Name = "Updated Name",
            Description = "Updated Description"
        };

        _projectRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*not found*");
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Location_Not_Found()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = Project.Create("Test Project", "Test Author", "Test Synopsis");
        
        var command = new UpdateLocationCommand
        {
            ProjectId = projectId,
            LocationId = Guid.NewGuid(), // Non-existent location
            Name = "Updated Name",
            Description = "Updated Description"
        };

        _projectRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
