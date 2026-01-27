using FluentAssertions;
using AutorLLM.Application.Commands.Characters.DeleteCharacter;
using AutorLLM.Domain.Aggregates.ProjectAggregate;
using AutorLLM.Domain.Interfaces;
using AutorLLM.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;

namespace AutorLLM.Tests.Unit.Application.Commands.Characters;

public class DeleteCharacterCommandHandlerTests
{
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<DeleteCharacterCommandHandler>> _loggerMock;
    private readonly DeleteCharacterCommandHandler _handler;

    public DeleteCharacterCommandHandlerTests()
    {
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<DeleteCharacterCommandHandler>>();
        _handler = new DeleteCharacterCommandHandler(
            _projectRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Delete_Character_Successfully()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = Project.Create("Test Project", "Test Author", "Test Synopsis");
        var character = project.AddCharacter("Frodo", "A hobbit", CharacterRole.Protagonist);

        var command = new DeleteCharacterCommand
        {
            ProjectId = projectId,
            CharacterId = character.Id
        };

        _projectRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        project.Characters.Should().BeEmpty();
        
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
        var command = new DeleteCharacterCommand
        {
            ProjectId = Guid.NewGuid(),
            CharacterId = Guid.NewGuid()
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
    public async Task Handle_Should_Use_Aggregate_RemoveCharacter_Method()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = Project.Create("Test Project", "Test Author", "Test Synopsis");
        var character1 = project.AddCharacter("Frodo", "A hobbit", CharacterRole.Protagonist);
        var character2 = project.AddCharacter("Gandalf", "A wizard", CharacterRole.Supporting);

        var command = new DeleteCharacterCommand
        {
            ProjectId = projectId,
            CharacterId = character1.Id
        };

        _projectRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        project.Characters.Should().HaveCount(1);
        project.Characters.First().Id.Should().Be(character2.Id);
    }
}
