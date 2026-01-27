using FluentAssertions;
using AutorLLM.Application.Commands.Locations.DeleteLocation;
using AutorLLM.Domain.Aggregates.ProjectAggregate;
using AutorLLM.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace AutorLLM.Tests.Unit.Application.Commands.Locations;

public class DeleteLocationCommandHandlerTests
{
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<DeleteLocationCommandHandler>> _loggerMock;
    private readonly DeleteLocationCommandHandler _handler;

    public DeleteLocationCommandHandlerTests()
    {
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<DeleteLocationCommandHandler>>();
        _handler = new DeleteLocationCommandHandler(
            _projectRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Delete_Location_Successfully()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = Project.Create("Test Project", "Test Author", "Test Synopsis");
        var location = project.AddLocation("The Shire", "A peaceful land");
        
        var command = new DeleteLocationCommand
        {
            ProjectId = projectId,
            LocationId = location.Id
        };

        _projectRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        project.Locations.Should().BeEmpty();
        
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
        var command = new DeleteLocationCommand
        {
            ProjectId = Guid.NewGuid(),
            LocationId = Guid.NewGuid()
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
    public async Task Handle_Should_Not_Throw_When_Location_Not_Found()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = Project.Create("Test Project", "Test Author", "Test Synopsis");
        
        var command = new DeleteLocationCommand
        {
            ProjectId = projectId,
            LocationId = Guid.NewGuid() // Non-existent location
        };

        _projectRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        // Act - Should not throw (idempotent delete)
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
    }
}
