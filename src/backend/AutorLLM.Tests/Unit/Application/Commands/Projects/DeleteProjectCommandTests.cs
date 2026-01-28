using FluentAssertions;
using AutorLLM.Application.Commands.Projects.DeleteProject;
using MediatR;

namespace AutorLLM.Tests.Unit.Application.Commands.Projects;

public class DeleteProjectCommandTests
{
    [Fact]
    public void Should_Implement_IRequest_Interface()
    {
        // Arrange & Act
        var command = new DeleteProjectCommand { ProjectId = Guid.NewGuid() };

        // Assert
        command.Should().BeAssignableTo<IRequest<MediatR.Unit>>();
    }

    [Fact]
    public void Should_Have_ProjectId_Property()
    {
        // Arrange
        var projectId = Guid.NewGuid();

        // Act
        var command = new DeleteProjectCommand { ProjectId = projectId };

        // Assert
        command.ProjectId.Should().Be(projectId);
    }

    [Fact]
    public void Should_Be_Immutable_Record()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var command1 = new DeleteProjectCommand { ProjectId = projectId };
        var command2 = new DeleteProjectCommand { ProjectId = projectId };

        // Assert
        command1.Should().Be(command2);
        command1.GetHashCode().Should().Be(command2.GetHashCode());
    }
}
