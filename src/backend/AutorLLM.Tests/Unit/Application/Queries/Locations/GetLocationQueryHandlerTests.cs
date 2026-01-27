using FluentAssertions;
using AutorLLM.Application.Queries.Locations.GetLocation;
using AutorLLM.Domain.Aggregates.ProjectAggregate;
using AutorLLM.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace AutorLLM.Tests.Unit.Application.Queries.Locations;

public class GetLocationQueryHandlerTests
{
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly Mock<ILogger<GetLocationQueryHandler>> _loggerMock;
    private readonly GetLocationQueryHandler _handler;

    public GetLocationQueryHandlerTests()
    {
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _loggerMock = new Mock<ILogger<GetLocationQueryHandler>>();
        _handler = new GetLocationQueryHandler(
            _projectRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Location_When_Found()
    {
        // Arrange
        var project = Project.Create("Test Project", "Test Author", "Test Synopsis");
        var location = project.AddLocation("The Shire", "A peaceful land");

        var query = new GetLocationQuery
        {
            LocationId = location.Id
        };

        _projectRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { project });

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(location.Id);
        result.Name.Should().Be("The Shire");
        result.Description.Should().Be("A peaceful land");
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Location_Not_Found()
    {
        // Arrange
        var query = new GetLocationQuery
        {
            LocationId = Guid.NewGuid()
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
    public async Task Handle_Should_Return_Location_With_Optional_Fields()
    {
        // Arrange
        var project = Project.Create("Test Project", "Test Author", "Test Synopsis");
        var location = project.AddLocation("Rivendell", "An elven sanctuary");
        location.UpdateGeography("Hidden valley surrounded by mountains");
        location.UpdateCulture("Elven culture with ancient wisdom");
        location.UpdateSignificance("Key refuge for the Fellowship");

        var query = new GetLocationQuery
        {
            LocationId = location.Id
        };

        _projectRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { project });

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Geography.Should().Be("Hidden valley surrounded by mountains");
        result.Culture.Should().Be("Elven culture with ancient wisdom");
        result.Significance.Should().Be("Key refuge for the Fellowship");
    }

    [Fact]
    public async Task Handle_Should_Search_Across_Multiple_Projects()
    {
        // Arrange
        var project1 = Project.Create("Project 1", "Author 1", "Synopsis 1");
        var project2 = Project.Create("Project 2", "Author 2", "Synopsis 2");
        var location = project2.AddLocation("Mordor", "Dark land of evil");

        var query = new GetLocationQuery
        {
            LocationId = location.Id
        };

        _projectRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { project1, project2 });

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(location.Id);
        result.Name.Should().Be("Mordor");
    }
}
