using FluentAssertions;
using AutorLLM.Application.Queries.Characters.ListCharacters;
using AutorLLM.Application.DTOs;
using MediatR;

namespace AutorLLM.Tests.Unit.Application.Queries.Characters;

public class ListCharactersQueryTests
{
    [Fact]
    public void Should_Implement_IRequest_Interface()
    {
        // Arrange & Act
        var query = new ListCharactersQuery { ProjectId = Guid.NewGuid() };

        // Assert
        query.Should().BeAssignableTo<IRequest<IEnumerable<CharacterDto>>>();
    }

    [Fact]
    public void Should_Have_ProjectId_Property()
    {
        // Arrange
        var projectId = Guid.NewGuid();

        // Act
        var query = new ListCharactersQuery { ProjectId = projectId };

        // Assert
        query.ProjectId.Should().Be(projectId);
    }

    [Fact]
    public void Should_Be_Immutable_Record()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var query1 = new ListCharactersQuery { ProjectId = projectId };
        var query2 = new ListCharactersQuery { ProjectId = projectId };

        // Assert
        query1.Should().Be(query2);
        query1.GetHashCode().Should().Be(query2.GetHashCode());
    }
}
