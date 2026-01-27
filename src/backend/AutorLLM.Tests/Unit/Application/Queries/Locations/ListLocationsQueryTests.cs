using FluentAssertions;
using AutorLLM.Application.Queries.Locations.ListLocations;
using AutorLLM.Application.DTOs;
using MediatR;

namespace AutorLLM.Tests.Unit.Application.Queries.Locations;

public class ListLocationsQueryTests
{
    [Fact]
    public void Should_Implement_IRequest_Interface()
    {
        // Arrange & Act
        var query = new ListLocationsQuery { ProjectId = Guid.NewGuid() };

        // Assert
        query.Should().BeAssignableTo<IRequest<IEnumerable<LocationDto>>>();
    }

    [Fact]
    public void Should_Have_ProjectId_Property()
    {
        // Arrange
        var projectId = Guid.NewGuid();

        // Act
        var query = new ListLocationsQuery { ProjectId = projectId };

        // Assert
        query.ProjectId.Should().Be(projectId);
    }

    [Fact]
    public void Should_Be_Immutable_Record()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var query1 = new ListLocationsQuery { ProjectId = projectId };
        var query2 = new ListLocationsQuery { ProjectId = projectId };

        // Assert
        query1.Should().Be(query2);
        query1.GetHashCode().Should().Be(query2.GetHashCode());
    }
}
