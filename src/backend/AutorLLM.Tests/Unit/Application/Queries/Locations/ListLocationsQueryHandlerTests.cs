using FluentAssertions;
using AutorLLM.Application.Queries.Locations.ListLocations;
using AutorLLM.Domain.Aggregates.ProjectAggregate;
using AutorLLM.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace AutorLLM.Tests.Unit.Application.Queries.Locations;

public class ListLocationsQueryHandlerTests
{
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly Mock<ILogger<ListLocationsQueryHandler>> _loggerMock;
    private readonly ListLocationsQueryHandler _handler;

    public ListLocationsQueryHandlerTests()
    {
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _loggerMock = new Mock<ILogger<ListLocationsQueryHandler>>();
        _handler = new ListLocationsQueryHandler(
            _projectRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_All_Locations_For_Project()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = Project.Create("Test Project", "Test Author", "Test Synopsis");
        project.AddLocation("The Shire", "Hobbit homeland");
        project.AddLocation("Rivendell", "Elven sanctuary");
        project.AddLocation("Mordor", "Land of darkness");

        var query = new ListLocationsQuery
        {
            ProjectId = projectId
        };

        _projectRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().Contain(l => l.Name == "The Shire");
        result.Should().Contain(l => l.Name == "Rivendell");
        result.Should().Contain(l => l.Name == "Mordor");
    }

    [Fact]
    public async Task Handle_Should_Return_Empty_List_When_No_Locations()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = Project.Create("Test Project", "Test Author", "Test Synopsis");

        var query = new ListLocationsQuery
        {
            ProjectId = projectId
        };

        _projectRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
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
        var query = new ListLocationsQuery
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
    public async Task Handle_Should_Return_Locations_With_All_Properties()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = Project.Create("Test Project", "Test Author", "Test Synopsis");
        var location = project.AddLocation("Gondor", "Kingdom of Men");
        location.UpdateGeography("Mountainous region");
        location.UpdateCulture("Noble warriors");
        location.UpdateSignificance("Last bastion of hope");

        var query = new ListLocationsQuery
        {
            ProjectId = projectId
        };

        _projectRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var locationDto = result.First();
        locationDto.Name.Should().Be("Gondor");
        locationDto.Description.Should().Be("Kingdom of Men");
        locationDto.Geography.Should().Be("Mountainous region");
        locationDto.Culture.Should().Be("Noble warriors");
        locationDto.Significance.Should().Be("Last bastion of hope");
    }
}
