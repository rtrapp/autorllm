using FluentAssertions;
using AutorLLM.Application.Queries.PlotPoints.GetPlotPointsByChapter;

namespace AutorLLM.Tests.Unit.Application.Queries.PlotPoints;

public class GetPlotPointsByChapterQueryTests
{
    [Fact]
    public void Query_Should_Implement_IRequest()
    {
        // Arrange & Act
        var query = new GetPlotPointsByChapterQuery
        {
            ProjectId = Guid.NewGuid(),
            ChapterId = Guid.NewGuid()
        };

        // Assert
        query.Should().NotBeNull();
        query.ProjectId.Should().NotBeEmpty();
        query.ChapterId.Should().NotBeEmpty();
    }

    [Fact]
    public void Query_Properties_Should_Be_Immutable()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var chapterId = Guid.NewGuid();

        // Act
        var query = new GetPlotPointsByChapterQuery
        {
            ProjectId = projectId,
            ChapterId = chapterId
        };

        // Assert
        query.ProjectId.Should().Be(projectId);
        query.ChapterId.Should().Be(chapterId);
    }

    [Fact]
    public void Query_Is_Record_Type()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var chapterId = Guid.NewGuid();

        var query1 = new GetPlotPointsByChapterQuery
        {
            ProjectId = projectId,
            ChapterId = chapterId
        };

        var query2 = new GetPlotPointsByChapterQuery
        {
            ProjectId = projectId,
            ChapterId = chapterId
        };

        // Act & Assert
        query1.Should().Be(query2); // Record types have value equality
    }
}
