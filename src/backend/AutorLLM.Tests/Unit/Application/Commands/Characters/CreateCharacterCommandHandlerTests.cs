using FluentAssertions;
using AutorLLM.Application.Commands.Characters.CreateCharacter;
using AutorLLM.Domain.Aggregates.ProjectAggregate;
using AutorLLM.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace AutorLLM.Tests.Unit.Application.Commands.Characters;

public class CreateCharacterCommandHandlerTests
{
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<CreateCharacterCommandHandler>> _loggerMock;
    private readonly CreateCharacterCommandHandler _handler;

    public CreateCharacterCommandHandlerTests()
    {
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<CreateCharacterCommandHandler>>();
        _handler = new CreateCharacterCommandHandler(
            _projectRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Create_Character_Successfully()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = Project.Create("Test Project", "Test Author", "Test Synopsis");
        
        var command = new CreateCharacterCommand
        {
            ProjectId = projectId,
            Name = "Frodo Baggins",
            Role = "Protagonist",
            Biography = "A hobbit from the Shire"
        };

        _projectRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.CharacterId.Should().NotBeEmpty();
        
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
        var command = new CreateCharacterCommand
        {
            ProjectId = Guid.NewGuid(),
            Name = "Frodo Baggins",
            Role = "Protagonist",
            Biography = "A hobbit from the Shire"
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
    public async Task Handle_Should_Use_Aggregate_AddCharacter_Method()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = Project.Create("Test Project", "Test Author", "Test Synopsis");
        
        var command = new CreateCharacterCommand
        {
            ProjectId = projectId,
            Name = "Gandalf",
            Role = "Supporting",
            Biography = "A wizard"
        };

        _projectRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        project.Characters.Should().HaveCount(1);
        project.Characters.First().Name.Should().Be("Gandalf");
        project.Characters.First().Role.ToString().Should().Be("Supporting");
    }
}
