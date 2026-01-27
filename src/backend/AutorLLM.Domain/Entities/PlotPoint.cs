using AutorLLM.Domain.Common;

namespace AutorLLM.Domain.Entities;

/// <summary>
/// PlotPoint entity - represents a specific point/moment in a plot arc.
/// Used for tracking plot progression through chapters.
/// </summary>
public class PlotPoint : EntityBase
{
    public Guid PlotId { get; private set; }
    public Guid ChapterId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public int IntensityLevel { get; private set; } // 0-10 scale
    public int Order { get; private set; }

    // Private constructor for EF Core
    private PlotPoint() { }

    // Factory method
    public static PlotPoint Create(
        Guid plotId,
        Guid chapterId,
        string description,
        int intensityLevel,
        int order)
    {
        if (plotId == Guid.Empty)
            throw new ArgumentException("PlotId cannot be empty.", nameof(plotId));

        if (chapterId == Guid.Empty)
            throw new ArgumentException("ChapterId cannot be empty.", nameof(chapterId));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("PlotPoint description cannot be empty.", nameof(description));

        if (description.Length > 500)
            throw new ArgumentException("PlotPoint description cannot exceed 500 characters.", nameof(description));

        if (intensityLevel < 0 || intensityLevel > 10)
            throw new ArgumentException("Intensity level must be between 0 and 10.", nameof(intensityLevel));

        if (order < 0)
            throw new ArgumentException("Order must be greater than or equal to 0.", nameof(order));

        var plotPoint = new PlotPoint
        {
            PlotId = plotId,
            ChapterId = chapterId,
            Description = description.Trim(),
            IntensityLevel = intensityLevel,
            Order = order
        };

        return plotPoint;
    }

    // Behavior methods
    public void UpdateDetails(string description, int intensityLevel)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("PlotPoint description cannot be empty.", nameof(description));

        if (description.Length > 500)
            throw new ArgumentException("PlotPoint description cannot exceed 500 characters.", nameof(description));

        if (intensityLevel < 0 || intensityLevel > 10)
            throw new ArgumentException("Intensity level must be between 0 and 10.", nameof(intensityLevel));

        Description = description.Trim();
        IntensityLevel = intensityLevel;
        Touch();
    }

    public void UpdateOrder(int order)
    {
        if (order < 0)
            throw new ArgumentException("Order must be greater than or equal to 0.", nameof(order));

        Order = order;
        Touch();
    }
}
