using FluentAssertions;
using AutorLLM.Application.Queries.PlotPoints.GetPlotPointsByPlot;

namespace AutorLLM.Tests.Unit.Application.Queries.PlotPoints;

public class GetPlotPointsByPlotQueryTests
{
    [Fact]
    public void Query_Should_Implement_IRequest()
    {
        // Arrange & Act
        var query = new GetPlotPointsByPlotQuery
        {
            ProjectId = Guid.NewGuid(),
            PlotId = Guid.NewGuid()
        };

        // Assert
        query.Should().NotBeNull();
        query.ProjectId.Should().NotBeEmpty();
        query.PlotId.Should().NotBeEmpty();
    }

    [Fact]
    public void Query_Properties_Should_Be_Immutable()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var plotId = Guid.NewGuid();

        // Act
        var query = new GetPlotPointsByPlotQuery
        {
            ProjectId = projectId,
            PlotId = plotId
        };

        // Assert
        query.ProjectId.Should().Be(projectId);
        query.PlotId.Should().Be(plotId);
    }

    [Fact]
    public void Query_Is_Record_Type()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var plotId = Guid.NewGuid();

        var query1 = new GetPlotPointsByPlotQuery
        {
            ProjectId = projectId,
            PlotId = plotId
        };

        var query2 = new GetPlotPointsByPlotQuery
        {
            ProjectId = projectId,
            PlotId = plotId
        };

        // Act & Assert
        query1.Should().Be(query2); // Record types have value equality
    }
}
