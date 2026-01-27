using FluentAssertions;
using AutorLLM.Application.Queries.Plots.GetPlot;
using AutorLLM.Application.DTOs;
using MediatR;

namespace AutorLLM.Tests.Unit.Application.Queries.Plots;

public class GetPlotQueryTests
{
    [Fact]
    public void Should_Implement_IRequest_Interface()
    {
        // Arrange & Act
        var query = new GetPlotQuery { PlotId = Guid.NewGuid() };

        // Assert
        query.Should().BeAssignableTo<IRequest<PlotDto>>();
    }

    [Fact]
    public void Should_Have_PlotId_Property()
    {
        // Arrange
        var plotId = Guid.NewGuid();

        // Act
        var query = new GetPlotQuery { PlotId = plotId };

        // Assert
        query.PlotId.Should().Be(plotId);
    }

    [Fact]
    public void Should_Be_Immutable_Record()
    {
        // Arrange
        var plotId = Guid.NewGuid();
        var query1 = new GetPlotQuery { PlotId = plotId };
        var query2 = new GetPlotQuery { PlotId = plotId };

        // Assert
        query1.Should().Be(query2);
        query1.GetHashCode().Should().Be(query2.GetHashCode());
    }
}
