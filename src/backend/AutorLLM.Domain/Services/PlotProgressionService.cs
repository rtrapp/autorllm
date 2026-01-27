using AutorLLM.Domain.Entities;

namespace AutorLLM.Domain.Services;

/// <summary>
/// Domain service implementation for plot progression validation and analysis.
/// Stateless service - no internal state, pure business logic.
/// </summary>
public class PlotProgressionService : IPlotProgressionService
{
    public bool ValidatePlotProgression(Plot plot, IEnumerable<Chapter> chapters)
    {
        if (plot == null)
            throw new ArgumentNullException(nameof(plot));

        if (chapters == null)
            throw new ArgumentNullException(nameof(chapters));

        var plotPoints = plot.PlotPoints.ToList();

        // Plot must have at least 3 points (beginning, middle, end)
        if (plotPoints.Count < 3)
            return false;

        var orderedPoints = plotPoints.OrderBy(pp => pp.Order).ToList();
        var chapterList = chapters.ToList();
        var totalChapters = chapterList.Count;

        if (totalChapters == 0)
            return false;

        // Validate that plot points are distributed across the story
        var firstPointOrder = orderedPoints.First().Order;
        var lastPointOrder = orderedPoints.Last().Order;

        // First point should be early, last point should be late
        if (firstPointOrder > plotPoints.Count / 4)
            return false;

        if (lastPointOrder < (plotPoints.Count * 3 / 4))
            return false;

        // Validate climax position (highest intensity should be in second half)
        var climaxOrder = FindClimaxChapter(plot);
        if (climaxOrder.HasValue && climaxOrder.Value < plotPoints.Count / 2)
            return false;

        return true;
    }

    public Dictionary<int, double> CalculateIntensityProgression(Plot plot)
    {
        if (plot == null)
            throw new ArgumentNullException(nameof(plot));

        var result = new Dictionary<int, double>();
        var plotPoints = plot.PlotPoints.ToList();

        if (!plotPoints.Any())
            return result;

        // Normalize intensity values to 0-100 scale (IntensityLevel is 0-10)
        foreach (var point in plotPoints.OrderBy(pp => pp.Order))
        {
            var normalizedIntensity = (point.IntensityLevel / 10.0) * 100;
            result[point.Order] = Math.Round(normalizedIntensity, 2);
        }

        return result;
    }

    public int? FindClimaxChapter(Plot plot)
    {
        if (plot == null)
            throw new ArgumentNullException(nameof(plot));

        var plotPoints = plot.PlotPoints.ToList();

        if (!plotPoints.Any())
            return null;

        var climaxPoint = plotPoints.OrderByDescending(pp => pp.IntensityLevel).First();
        return climaxPoint.Order;
    }
}
