using FluentAssertions;
using Moq;
using AutorLLM.Application.Commands.Projects.DeleteProject;
using AutorLLM.Domain.Interfaces;
using AutorLLM.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace AutorLLM.Tests.Unit.Application.Commands.Projects;

public class DeleteProjectCommandHandlerTests
{
    private readonly Mock<IProjectRepository> _mockRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ILogger<DeleteProjectCommandHandler>> _mockLogger;
    private readonly DeleteProjectCommandHandler _handler;

    public DeleteProjectCommandHandlerTests()
    {
        _mockRepository = new Mock<IProjectRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockLogger = new Mock<ILogger<DeleteProjectCommandHandler>>();
        _handler = new DeleteProjectCommandHandler(
            _mockRepository.Object,
            _mockUnitOfWork.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Should_Delete_Project_When_Exists()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var command = new DeleteProjectCommand { ProjectId = projectId };

        _mockRepository
            .Setup(r => r.ExistsAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockRepository
            .Setup(r => r.DeleteAsync(projectId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork
            .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(MediatR.Unit.Value);
        _mockRepository.Verify(r => r.ExistsAsync(projectId, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.DeleteAsync(projectId, It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_Throw_Exception_When_Project_Not_Found()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var command = new DeleteProjectCommand { ProjectId = projectId };

        _mockRepository
            .Setup(r => r.ExistsAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ProjectNotFoundException>()
            .WithMessage($"Project with ID '{projectId}' was not found.");
        
        _mockRepository.Verify(r => r.ExistsAsync(projectId, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Should_Not_Commit_When_Delete_Fails()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var command = new DeleteProjectCommand { ProjectId = projectId };

        _mockRepository
            .Setup(r => r.ExistsAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockRepository
            .Setup(r => r.DeleteAsync(projectId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Database error");
        _mockUnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
