using AutorLLM.Domain.Common;
using AutorLLM.Domain.ValueObjects;
using AutorLLM.Domain.Events;

namespace AutorLLM.Domain.Entities;

/// <summary>
/// Location entity - represents a location in the story.
/// Rich domain entity with encapsulated behavior.
/// </summary>
public class Location : EntityBase
{
    public Guid ProjectId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string? Geography { get; private set; }
    public string? Culture { get; private set; }
    public string? Significance { get; private set; }

    // Private constructor for EF Core
    private Location() { }

    // Internal factory method for hydration from database (used by repository)
    internal static Location Hydrate(
        Guid id,
        Guid projectId,
        string name,
        string description,
        string? geography,
        string? culture,
        string? significance,
        DateTime createdAt,
        DateTime updatedAt)
    {
        var location = new Location
        {
            Id = id,
            ProjectId = projectId,
            Name = name,
            Description = description,
            Geography = geography,
            Culture = culture,
            Significance = significance,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        return location;
    }

    // Factory method
    public static Location Create(
        Guid projectId,
        string name,
        string description,
        string? geography = null,
        string? culture = null,
        string? significance = null)
    {
        if (projectId == Guid.Empty)
            throw new ArgumentException("ProjectId cannot be empty.", nameof(projectId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Location name cannot be empty.", nameof(name));

        if (name.Length > 100)
            throw new ArgumentException("Location name cannot exceed 100 characters.", nameof(name));

        if (description.Length > 1000)
            throw new ArgumentException("Location description cannot exceed 1000 characters.", nameof(description));

        if (geography != null && geography.Length > 2000)
            throw new ArgumentException("Geography cannot exceed 2000 characters.", nameof(geography));

        if (culture != null && culture.Length > 2000)
            throw new ArgumentException("Culture cannot exceed 2000 characters.", nameof(culture));

        if (significance != null && significance.Length > 1000)
            throw new ArgumentException("Significance cannot exceed 1000 characters.", nameof(significance));

        var location = new Location
        {
            ProjectId = projectId,
            Name = name.Trim(),
            Description = description.Trim(),
            Geography = geography?.Trim(),
            Culture = culture?.Trim(),
            Significance = significance?.Trim()
        };

        return location;
    }

    // Behavior methods
    public void UpdateDetails(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Location name cannot be empty.", nameof(name));

        if (name.Length > 100)
            throw new ArgumentException("Location name cannot exceed 100 characters.", nameof(name));

        if (description.Length > 1000)
            throw new ArgumentException("Location description cannot exceed 1000 characters.", nameof(description));

        Name = name.Trim();
        Description = description.Trim();
        Touch();
    }

    public void UpdateGeography(string? geography)
    {
        if (geography != null && geography.Length > 2000)
            throw new ArgumentException("Geography cannot exceed 2000 characters.", nameof(geography));

        Geography = geography?.Trim();
        Touch();
    }

    public void UpdateCulture(string? culture)
    {
        if (culture != null && culture.Length > 2000)
            throw new ArgumentException("Culture cannot exceed 2000 characters.", nameof(culture));

        Culture = culture?.Trim();
        Touch();
    }

    public void UpdateSignificance(string? significance)
    {
        if (significance != null && significance.Length > 1000)
            throw new ArgumentException("Significance cannot exceed 1000 characters.", nameof(significance));

        Significance = significance?.Trim();
        Touch();
    }
}
