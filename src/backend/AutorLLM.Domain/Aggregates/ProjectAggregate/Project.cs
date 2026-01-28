using AutorLLM.Domain.Common;
using AutorLLM.Domain.Events;
using AutorLLM.Domain.Exceptions;
using AutorLLM.Domain.ValueObjects;

namespace AutorLLM.Domain.Aggregates.ProjectAggregate;

/// <summary>
/// Project is the Aggregate Root - controls access to all child entities.
/// Rich domain entity with encapsulated behavior.
/// All operations on Characters, Chapters, Plots, and Locations must go through Project.
/// </summary>
public class Project : EntityBase
{
    public string Title { get; private set; } = string.Empty;
    public string Author { get; private set; } = string.Empty;
    public string Synopsis { get; private set; } = string.Empty;
    public string? Genre { get; private set; }
    public int TargetWordCount { get; private set; }
    public int CurrentWordCount { get; private set; }

    // Collections - encapsulated, accessed only through methods
    private readonly List<Entities.Character> _characters = new();
    public IReadOnlyCollection<Entities.Character> Characters => _characters.AsReadOnly();

    private readonly List<Entities.Chapter> _chapters = new();
    public IReadOnlyCollection<Entities.Chapter> Chapters => _chapters.AsReadOnly();

    private readonly List<Entities.Plot> _plots = new();
    public IReadOnlyCollection<Entities.Plot> Plots => _plots.AsReadOnly();

    private readonly List<Entities.Location> _locations = new();
    public IReadOnlyCollection<Entities.Location> Locations => _locations.AsReadOnly();

    // Private constructor for EF Core
    private Project() { }

    // Internal factory method for hydration from database (used by repository)
    public static Project Hydrate(
        Guid id,
        string title,
        string author,
        string synopsis,
        string? genre,
        int targetWordCount,
        int currentWordCount,
        DateTime createdAt,
        DateTime updatedAt)
    {
        var project = new Project
        {
            Id = id,
            Title = title,
            Author = author,
            Synopsis = synopsis,
            Genre = genre,
            TargetWordCount = targetWordCount,
            CurrentWordCount = currentWordCount,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        return project;
    }

    // Factory method - encapsulates creation logic
    public static Project Create(
        string title,
        string author,
        string synopsis,
        string? genre = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Project title cannot be empty.", nameof(title));

        if (title.Length > 200)
            throw new ArgumentException("Project title cannot exceed 200 characters.", nameof(title));

        if (string.IsNullOrWhiteSpace(author))
            throw new ArgumentException("Author name cannot be empty.", nameof(author));

        if (author.Length > 100)
            throw new ArgumentException("Author name cannot exceed 100 characters.", nameof(author));

        if (synopsis.Length > 5000)
            throw new ArgumentException("Synopsis cannot exceed 5000 characters.", nameof(synopsis));

        if (genre != null && genre.Length > 50)
            throw new ArgumentException("Genre cannot exceed 50 characters.", nameof(genre));

        var project = new Project
        {
            Title = title.Trim(),
            Author = author.Trim(),
            Synopsis = synopsis.Trim(),
            Genre = genre?.Trim(),
            TargetWordCount = 50000, // Default target
            CurrentWordCount = 0
        };

        project.AddDomainEvent(new ProjectCreatedEvent(project.Id, title));

        return project;
    }

    #region Project Operations

    public void UpdateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Project title cannot be empty.", nameof(title));

        if (title.Length > 200)
            throw new ArgumentException("Project title cannot exceed 200 characters.", nameof(title));

        Title = title.Trim();
        Touch();
        AddDomainEvent(new ProjectUpdatedEvent(Id));
    }

    public void UpdateAuthor(string author)
    {
        if (string.IsNullOrWhiteSpace(author))
            throw new ArgumentException("Author name cannot be empty.", nameof(author));

        if (author.Length > 100)
            throw new ArgumentException("Author name cannot exceed 100 characters.", nameof(author));

        Author = author.Trim();
        Touch();
    }

    public void UpdateSynopsis(string synopsis)
    {
        if (synopsis.Length > 5000)
            throw new ArgumentException("Synopsis cannot exceed 5000 characters.", nameof(synopsis));

        Synopsis = synopsis.Trim();
        Touch();
    }

    public void SetGenre(string? genre)
    {
        if (genre != null && genre.Length > 50)
            throw new ArgumentException("Genre cannot exceed 50 characters.", nameof(genre));

        Genre = genre?.Trim();
        Touch();
    }

    public void SetTargetWordCount(int targetWordCount)
    {
        if (targetWordCount < 0)
            throw new ArgumentException("Target word count must be greater than or equal to 0.", nameof(targetWordCount));

        TargetWordCount = targetWordCount;
        Touch();
    }

    #endregion

    #region Character Operations

    public Entities.Character AddCharacter(
        string name,
        string description,
        CharacterRole role,
        string? backstory = null,
        string? appearance = null,
        string? personality = null)
    {
        // Validate uniqueness
        if (_characters.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            throw new DuplicateCharacterNameException(name);

        var character = Entities.Character.Create(
            Id,
            name,
            description,
            role,
            backstory,
            appearance,
            personality);
        _characters.Add(character);
        Touch();

        return character;
    }

    // Internal method for repository hydration (bypasses business rules)
    internal void HydrateCharacter(Entities.Character character)
    {
        _characters.Add(character);
    }

    public void RemoveCharacter(Guid characterId)
    {
        var character = _characters.FirstOrDefault(c => c.Id == characterId);
        if (character == null)
            throw new CharacterNotFoundException(characterId);

        _characters.Remove(character);
        Touch();

        AddDomainEvent(new CharacterDeletedEvent(characterId, Id));
    }

    public Entities.Character GetCharacter(Guid characterId)
    {
        var character = _characters.FirstOrDefault(c => c.Id == characterId);
        if (character == null)
            throw new CharacterNotFoundException(characterId);

        return character;
    }

    #endregion

    #region Chapter Operations

    public Entities.Chapter AddChapter(string title)
    {
        var nextOrder = _chapters.Any() 
            ? _chapters.Max(c => c.Order.Value) + 1 
            : 1;

        var chapter = Entities.Chapter.Create(Id, title, nextOrder);
        _chapters.Add(chapter);
        Touch();

        return chapter;
    }

    public void RemoveChapter(Guid chapterId)
    {
        var chapter = _chapters.FirstOrDefault(c => c.Id == chapterId);
        if (chapter == null)
            return;

        _chapters.Remove(chapter);
        RecalculateWordCount();
        ReorderChaptersAfterRemoval(chapter.Order.Value);
        Touch();
    }

    public void ReorderChapters(List<Guid> chapterIds)
    {
        if (chapterIds.Count != _chapters.Count)
            throw new InvalidOperationException("All chapters must be included in reordering.");

        for (int i = 0; i < chapterIds.Count; i++)
        {
            var chapter = _chapters.FirstOrDefault(c => c.Id == chapterIds[i]);
            if (chapter == null)
                throw new InvalidOperationException($"Chapter {chapterIds[i]} not found.");

            chapter.UpdateOrder(i + 1);
        }

        Touch();
    }

    // Internal method for repository hydration (bypasses business rules)
    internal void HydrateChapter(Entities.Chapter chapter)
    {
        _chapters.Add(chapter);
    }

    private void ReorderChaptersAfterRemoval(int removedOrder)
    {
        var chaptersToReorder = _chapters
            .Where(c => c.Order.Value > removedOrder)
            .OrderBy(c => c.Order.Value)
            .ToList();

        foreach (var chapter in chaptersToReorder)
        {
            chapter.UpdateOrder(chapter.Order.Value - 1);
        }
    }

    private void RecalculateWordCount()
    {
        CurrentWordCount = _chapters.Sum(c => c.WordCount);
    }

    public Entities.Chapter GetChapter(Guid chapterId)
    {
        var chapter = _chapters.FirstOrDefault(c => c.Id == chapterId);
        if (chapter == null)
            throw new InvalidOperationException($"Chapter {chapterId} not found.");

        return chapter;
    }

    #endregion

    #region Plot Operations

    public Entities.Plot AddPlot(
        string title,
        string description,
        PlotType type)
    {
        var plot = Entities.Plot.Create(Id, title, description, type);
        _plots.Add(plot);
        Touch();

        return plot;
    }

    public void RemovePlot(Guid plotId)
    {
        var plot = _plots.FirstOrDefault(p => p.Id == plotId);
        if (plot == null)
            return;

        _plots.Remove(plot);
        Touch();
    }

    public Entities.Plot GetPlot(Guid plotId)
    {
        var plot = _plots.FirstOrDefault(p => p.Id == plotId);
        if (plot == null)
            throw new InvalidOperationException($"Plot {plotId} not found.");

        return plot;
    }

    public IEnumerable<Entities.Plot> GetActivePlots()
    {
        return _plots.Where(p => p.IsActive).ToList();
    }

    // Internal method for repository hydration (bypasses business rules)
    internal void HydratePlot(Entities.Plot plot)
    {
        _plots.Add(plot);
    }

    #endregion

    #region Location Operations

    public Entities.Location AddLocation(
        string name,
        string description,
        string? geography = null,
        string? culture = null,
        string? significance = null)
    {
        var location = Entities.Location.Create(Id, name, description, geography, culture, significance);
        _locations.Add(location);
        Touch();

        return location;
    }

    // Internal method for repository hydration (bypasses business rules)
    internal void HydrateLocation(Entities.Location location)
    {
        _locations.Add(location);
    }

    public void RemoveLocation(Guid locationId)
    {
        var location = _locations.FirstOrDefault(l => l.Id == locationId);
        if (location == null)
            return;

        _locations.Remove(location);
        Touch();
    }

    public Entities.Location GetLocation(Guid locationId)
    {
        var location = _locations.FirstOrDefault(l => l.Id == locationId);
        if (location == null)
            throw new InvalidOperationException($"Location {locationId} not found.");

        return location;
    }

    #endregion
}
