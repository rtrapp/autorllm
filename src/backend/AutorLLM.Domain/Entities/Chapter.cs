using AutorLLM.Domain.Common;
using AutorLLM.Domain.ValueObjects;
using AutorLLM.Domain.Events;

namespace AutorLLM.Domain.Entities;

/// <summary>
/// Chapter entity - represents a chapter in the book.
/// Rich domain entity with encapsulated behavior.
/// </summary>
public class Chapter : EntityBase
{
    public Guid ProjectId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Summary { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public ChapterOrder Order { get; private set; } = ChapterOrder.Create(1);
    public int WordCount { get; private set; }

    // Private constructor for EF Core
    private Chapter() { }

    // Factory method
    public static Chapter Create(
        Guid projectId,
        string title,
        int order)
    {
        if (projectId == Guid.Empty)
            throw new ArgumentException("ProjectId cannot be empty.", nameof(projectId));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Chapter title cannot be empty.", nameof(title));

        if (title.Length > 200)
            throw new ArgumentException("Chapter title cannot exceed 200 characters.", nameof(title));

        var chapter = new Chapter
        {
            ProjectId = projectId,
            Title = title.Trim(),
            Order = ChapterOrder.Create(order),
            Summary = string.Empty,
            Content = string.Empty,
            WordCount = 0
        };

        chapter.AddDomainEvent(new ChapterCreatedEvent(chapter.Id, projectId, title));

        return chapter;
    }

    // Internal factory method for hydration from database (used by repository)
    internal static Chapter Hydrate(
        Guid id,
        Guid projectId,
        string title,
        string summary,
        string content,
        int order,
        int wordCount,
        DateTime createdAt,
        DateTime updatedAt)
    {
        return new Chapter
        {
            Id = id,
            ProjectId = projectId,
            Title = title,
            Summary = summary,
            Content = content,
            Order = ChapterOrder.Create(order),
            WordCount = wordCount,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
    }

    // Behavior methods
    public void UpdateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Chapter title cannot be empty.", nameof(title));

        if (title.Length > 200)
            throw new ArgumentException("Chapter title cannot exceed 200 characters.", nameof(title));

        Title = title.Trim();
        Touch();
    }

    public void UpdateSummary(string summary)
    {
        if (summary.Length > 2000)
            throw new ArgumentException("Chapter summary cannot exceed 2000 characters.", nameof(summary));

        Summary = summary.Trim();
        Touch();
    }

    public void UpdateContent(string content)
    {
        Content = content;
        WordCount = CountWords(content);
        Touch();

        AddDomainEvent(new ChapterContentUpdatedEvent(Id, ProjectId, WordCount));
    }

    public void UpdateOrder(int order)
    {
        Order = ChapterOrder.Create(order);
        Touch();
    }

    private static int CountWords(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;

        return text.Split(new[] { ' ', '\t', '\n', '\r' }, 
            StringSplitOptions.RemoveEmptyEntries).Length;
    }
}
