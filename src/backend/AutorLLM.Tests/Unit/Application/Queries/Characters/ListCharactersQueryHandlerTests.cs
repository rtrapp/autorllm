using FluentAssertions;
using AutorLLM.Application.Queries.Characters.ListCharacters;
using AutorLLM.Domain.Aggregates.ProjectAggregate;
using AutorLLM.Domain.Interfaces;
using AutorLLM.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;

namespace AutorLLM.Tests.Unit.Application.Queries.Characters;

public class ListCharactersQueryHandlerTests
{
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly Mock<ILogger<ListCharactersQueryHandler>> _loggerMock;
    private readonly ListCharactersQueryHandler _handler;

    public ListCharactersQueryHandlerTests()
    {
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _loggerMock = new Mock<ILogger<ListCharactersQueryHandler>>();
        _handler = new ListCharactersQueryHandler(
            _projectRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_All_Characters_For_Project()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = Project.Create("Test Project", "Test Author", "Test Synopsis");
        project.AddCharacter("Frodo", "A hobbit", CharacterRole.Protagonist);
        project.AddCharacter("Gandalf", "A wizard", CharacterRole.Supporting);
        project.AddCharacter("Sauron", "The Dark Lord", CharacterRole.Antagonist);

        var query = new ListCharactersQuery
        {
            ProjectId = projectId
        };

        _projectRepositoryMock
            .Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Select(c => c.Name).Should().Contain(new[] { "Frodo", "Gandalf", "Sauron" });
    }

    [Fact]
    public async Task Handle_Should_Return_Empty_List_When_No_Characters()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = Project.Create("Test Project", "Test Author", "Test Synopsis");

        var query = new ListCharactersQuery
        {
            ProjectId = projectId
        };

        _projectRepositoryMock
            .Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Project_Not_Found()
    {
        // Arrange
        var query = new ListCharactersQuery
        {
            ProjectId = Guid.NewGuid()
        };

        _projectRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);

        // Act
        var act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*not found*");
    }

    [Fact]
    public async Task Handle_Should_Map_All_Character_Properties_Correctly()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = Project.Create("Test Project", "Test Author", "Test Synopsis");
        var character = project.AddCharacter("Frodo", "A brave hobbit", CharacterRole.Protagonist);
        character.UpdateBackstory("Born in the Shire");

        var query = new ListCharactersQuery
        {
            ProjectId = projectId
        };

        _projectRepositoryMock
            .Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var dto = result.First();
        dto.Id.Should().Be(character.Id);
        dto.ProjectId.Should().Be(character.ProjectId);
        dto.Name.Should().Be("Frodo");
        dto.Description.Should().Be("A brave hobbit");
        dto.Role.Should().Be("Protagonist");
        dto.Backstory.Should().Be("Born in the Shire");
    }
}
