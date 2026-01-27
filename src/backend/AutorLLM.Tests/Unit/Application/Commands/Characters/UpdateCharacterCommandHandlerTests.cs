using FluentAssertions;
using AutorLLM.Application.Commands.Characters.UpdateCharacter;
using AutorLLM.Domain.Aggregates.ProjectAggregate;
using AutorLLM.Domain.Interfaces;
using AutorLLM.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;

namespace AutorLLM.Tests.Unit.Application.Commands.Characters;

public class UpdateCharacterCommandHandlerTests
{
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<UpdateCharacterCommandHandler>> _loggerMock;
    private readonly UpdateCharacterCommandHandler _handler;

    public UpdateCharacterCommandHandlerTests()
    {
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<UpdateCharacterCommandHandler>>();
        _handler = new UpdateCharacterCommandHandler(
            _projectRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Update_Character_Successfully()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = Project.Create("Test Project", "Test Author", "Test Synopsis");
        var character = project.AddCharacter("Frodo", "A hobbit", CharacterRole.Protagonist);

        var command = new UpdateCharacterCommand
        {
            ProjectId = projectId,
            CharacterId = character.Id,
            Name = "Frodo Baggins",
            Description = "Updated description",
            Role = "Supporting",
            Backstory = "New backstory"
        };

        _projectRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        character.Name.Should().Be("Frodo Baggins");
        character.Description.Should().Be("Updated description");
        character.Role.Should().Be(CharacterRole.Supporting);
        character.Backstory.Should().Be("New backstory");
        
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
        var command = new UpdateCharacterCommand
        {
            ProjectId = Guid.NewGuid(),
            CharacterId = Guid.NewGuid(),
            Name = "Frodo",
            Description = "A hobbit",
            Role = "Protagonist"
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
    public async Task Handle_Should_Update_Optional_Fields_When_Provided()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = Project.Create("Test Project", "Test Author", "Test Synopsis");
        var character = project.AddCharacter("Frodo", "A hobbit", CharacterRole.Protagonist);

        var command = new UpdateCharacterCommand
        {
            ProjectId = projectId,
            CharacterId = character.Id,
            Name = "Frodo",
            Description = "A hobbit",
            Role = "Protagonist",
            Appearance = "Short with curly hair",
            Personality = "Brave and determined"
        };

        _projectRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        character.Appearance.Should().Be("Short with curly hair");
        character.Personality.Should().Be("Brave and determined");
    }
}
