using FluentAssertions;
using AutorLLM.Application.Queries.Locations.GetLocation;
using AutorLLM.Application.DTOs;
using MediatR;

namespace AutorLLM.Tests.Unit.Application.Queries.Locations;

public class GetLocationQueryTests
{
    [Fact]
    public void Should_Implement_IRequest_Interface()
    {
        // Arrange & Act
        var query = new GetLocationQuery { LocationId = Guid.NewGuid() };

        // Assert
        query.Should().BeAssignableTo<IRequest<LocationDto>>();
    }

    [Fact]
    public void Should_Have_LocationId_Property()
    {
        // Arrange
        var locationId = Guid.NewGuid();

        // Act
        var query = new GetLocationQuery { LocationId = locationId };

        // Assert
        query.LocationId.Should().Be(locationId);
    }

    [Fact]
    public void Should_Be_Immutable_Record()
    {
        // Arrange
        var locationId = Guid.NewGuid();
        var query1 = new GetLocationQuery { LocationId = locationId };
        var query2 = new GetLocationQuery { LocationId = locationId };

        // Assert
        query1.Should().Be(query2);
        query1.GetHashCode().Should().Be(query2.GetHashCode());
    }
}
