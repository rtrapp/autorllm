using FluentAssertions;
using AutorLLM.Application.Queries.Projects.ListProjects;
using AutorLLM.Application.DTOs;
using MediatR;

namespace AutorLLM.Tests.Unit.Application.Queries.Projects;

public class ListProjectsQueryTests
{
    [Fact]
    public void Should_Implement_IRequest_Interface()
    {
        // Arrange & Act
        var query = new ListProjectsQuery();

        // Assert
        query.Should().BeAssignableTo<IRequest<IEnumerable<ProjectDto>>>();
    }

    [Fact]
    public void Should_Be_Immutable_Record()
    {
        // Arrange
        var query1 = new ListProjectsQuery();
        var query2 = new ListProjectsQuery();

        // Assert
        query1.Should().Be(query2);
        query1.GetHashCode().Should().Be(query2.GetHashCode());
    }
}
