using FluentAssertions;
using AutorLLM.Application.Commands.Locations.CreateLocation;
using AutorLLM.Domain.Aggregates.ProjectAggregate;
using AutorLLM.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace AutorLLM.Tests.Unit.Application.Commands.Locations;

public class CreateLocationCommandHandlerTests
{
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<CreateLocationCommandHandler>> _loggerMock;
    private readonly CreateLocationCommandHandler _handler;

    public CreateLocationCommandHandlerTests()
    {
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<CreateLocationCommandHandler>>();
        _handler = new CreateLocationCommandHandler(
            _projectRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Create_Location_Successfully()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = Project.Create("Test Project", "Test Author", "Test Synopsis");
        
        var command = new CreateLocationCommand
        {
            ProjectId = projectId,
            Name = "The Shire",
            Description = "A peaceful land of hobbits"
        };

        _projectRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.LocationId.Should().NotBeEmpty();
        
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
        var command = new CreateLocationCommand
        {
            ProjectId = Guid.NewGuid(),
            Name = "The Shire",
            Description = "A peaceful land"
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
    public async Task Handle_Should_Use_Aggregate_AddLocation_Method()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = Project.Create("Test Project", "Test Author", "Test Synopsis");
        
        var command = new CreateLocationCommand
        {
            ProjectId = projectId,
            Name = "Rivendell",
            Description = "An elven sanctuary"
        };

        _projectRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        project.Locations.Should().HaveCount(1);
        project.Locations.First().Name.Should().Be("Rivendell");
    }
}
