using FluentAssertions;
using AutorLLM.Application.Queries.Plots.ListPlots;
using AutorLLM.Application.DTOs;
using MediatR;

namespace AutorLLM.Tests.Unit.Application.Queries.Plots;

public class ListPlotsQueryTests
{
    [Fact]
    public void Should_Implement_IRequest_Interface()
    {
        // Arrange & Act
        var query = new ListPlotsQuery { ProjectId = Guid.NewGuid() };

        // Assert
        query.Should().BeAssignableTo<IRequest<IEnumerable<PlotDto>>>();
    }

    [Fact]
    public void Should_Have_ProjectId_Property()
    {
        // Arrange
        var projectId = Guid.NewGuid();

        // Act
        var query = new ListPlotsQuery { ProjectId = projectId };

        // Assert
        query.ProjectId.Should().Be(projectId);
    }

    [Fact]
    public void Should_Be_Immutable_Record()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var query1 = new ListPlotsQuery { ProjectId = projectId };
        var query2 = new ListPlotsQuery { ProjectId = projectId };

        // Assert
        query1.Should().Be(query2);
        query1.GetHashCode().Should().Be(query2.GetHashCode());
    }
}
