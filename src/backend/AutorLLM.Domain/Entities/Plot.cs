using AutorLLM.Domain.Common;
using AutorLLM.Domain.ValueObjects;

namespace AutorLLM.Domain.Entities;

/// <summary>
/// Plot entity - represents a plot line/story arc in the book.
/// Rich domain entity with encapsulated behavior.
/// </summary>
public class Plot : EntityBase
{
    public Guid ProjectId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public PlotType Type { get; private set; } = PlotType.Subplot;
    public string? Resolution { get; private set; }
    public bool IsActive { get; private set; } = true;

    private readonly List<PlotPoint> _plotPoints = new();
    public IReadOnlyCollection<PlotPoint> PlotPoints => _plotPoints.AsReadOnly();

    // Private constructor for EF Core
    private Plot() { }

    // Factory method for new plots
    public static Plot Create(
        Guid projectId,
        string title,
        string description,
        PlotType type,
        string? resolution = null)
    {
        if (projectId == Guid.Empty)
            throw new ArgumentException("ProjectId cannot be empty.", nameof(projectId));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Plot title cannot be empty.", nameof(title));

        if (title.Length > 200)
            throw new ArgumentException("Plot title cannot exceed 200 characters.", nameof(title));

        if (description.Length > 2000)
            throw new ArgumentException("Plot description cannot exceed 2000 characters.", nameof(description));

        if (resolution != null && resolution.Length > 2000)
            throw new ArgumentException("Resolution cannot exceed 2000 characters.", nameof(resolution));

        var plot = new Plot
        {
            ProjectId = projectId,
            Title = title.Trim(),
            Description = description.Trim(),
            Type = type,
            Resolution = resolution?.Trim()
        };

        return plot;
    }

    // Hydrate method for repository (reconstruct from database)
    internal static Plot Hydrate(
        Guid id,
        Guid projectId,
        string title,
        string description,
        PlotType type,
        string? resolution,
        bool isActive,
        DateTime createdAt,
        DateTime updatedAt)
    {
        var plot = new Plot
        {
            Id = id,
            ProjectId = projectId,
            Title = title,
            Description = description,
            Type = type,
            Resolution = resolution,
            IsActive = isActive,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        return plot;
    }

    // Behavior methods
    public void UpdateDetails(string title, string description, PlotType type)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Plot title cannot be empty.", nameof(title));

        if (title.Length > 200)
            throw new ArgumentException("Plot title cannot exceed 200 characters.", nameof(title));

        if (description.Length > 2000)
            throw new ArgumentException("Plot description cannot exceed 2000 characters.", nameof(description));

        Title = title.Trim();
        Description = description.Trim();
        Type = type;
        Touch();
    }

    public void SetResolution(string? resolution)
    {
        if (resolution != null && resolution.Length > 2000)
            throw new ArgumentException("Resolution cannot exceed 2000 characters.", nameof(resolution));

        Resolution = resolution?.Trim();
        Touch();
    }

    public void Activate()
    {
        IsActive = true;
        Touch();
    }

    public void Deactivate()
    {
        IsActive = false;
        Touch();
    }

    public void AddPlotPoint(PlotPoint plotPoint)
    {
        if (plotPoint == null)
            throw new ArgumentNullException(nameof(plotPoint));

        if (plotPoint.PlotId != Id)
            throw new InvalidOperationException("PlotPoint must belong to this plot.");

        _plotPoints.Add(plotPoint);
        Touch();
    }

    public void RemovePlotPoint(Guid plotPointId)
    {
        var plotPoint = _plotPoints.FirstOrDefault(pp => pp.Id == plotPointId);
        if (plotPoint != null)
        {
            _plotPoints.Remove(plotPoint);
            Touch();
        }
    }
}
