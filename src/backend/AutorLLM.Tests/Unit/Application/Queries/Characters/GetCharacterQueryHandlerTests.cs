using FluentAssertions;
using AutorLLM.Application.Queries.Characters.GetCharacter;
using AutorLLM.Domain.Aggregates.ProjectAggregate;
using AutorLLM.Domain.Interfaces;
using AutorLLM.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;

namespace AutorLLM.Tests.Unit.Application.Queries.Characters;

public class GetCharacterQueryHandlerTests
{
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly Mock<ILogger<GetCharacterQueryHandler>> _loggerMock;
    private readonly GetCharacterQueryHandler _handler;

    public GetCharacterQueryHandlerTests()
    {
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _loggerMock = new Mock<ILogger<GetCharacterQueryHandler>>();
        _handler = new GetCharacterQueryHandler(
            _projectRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Character_When_Found()
    {
        // Arrange
        var project = Project.Create("Test Project", "Test Author", "Test Synopsis");
        var character = project.AddCharacter("Frodo", "A hobbit", CharacterRole.Protagonist);

        var query = new GetCharacterQuery
        {
            CharacterId = character.Id
        };

        _projectRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { project });

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(character.Id);
        result.Name.Should().Be("Frodo");
        result.Description.Should().Be("A hobbit");
        result.Role.Should().Be("Protagonist");
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Character_Not_Found()
    {
        // Arrange
        var query = new GetCharacterQuery
        {
            CharacterId = Guid.NewGuid()
        };

        _projectRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Project>());

        // Act
        var act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*not found*");
    }

    [Fact]
    public async Task Handle_Should_Return_Character_With_Optional_Fields()
    {
        // Arrange
        var project = Project.Create("Test Project", "Test Author", "Test Synopsis");
        var character = project.AddCharacter("Frodo", "A hobbit", CharacterRole.Protagonist);
        character.UpdateBackstory("Born in the Shire");
        character.UpdateAppearance("Short with curly hair");
        character.UpdatePersonality("Brave");

        var query = new GetCharacterQuery
        {
            CharacterId = character.Id
        };

        _projectRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { project });

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Backstory.Should().Be("Born in the Shire");
        result.Appearance.Should().Be("Short with curly hair");
        result.Personality.Should().Be("Brave");
    }
}
