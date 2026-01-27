using AutorLLM.Domain.Common;
using AutorLLM.Domain.ValueObjects;
using AutorLLM.Domain.Events;
using AutorLLM.Domain.Exceptions;

namespace AutorLLM.Domain.Entities;

/// <summary>
/// Character entity - represents a character in the story.
/// Rich domain entity with encapsulated behavior.
/// </summary>
public class Character : EntityBase
{
    public Guid ProjectId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public CharacterRole Role { get; private set; } = CharacterRole.Supporting;
    public string? Backstory { get; private set; }
    public string? Appearance { get; private set; }
    public string? Personality { get; private set; }

    // Private constructor for EF Core
    private Character() { }

    // Internal factory method for hydration from database (used by repository)
    internal static Character Hydrate(
        Guid id,
        Guid projectId,
        string name,
        string description,
        CharacterRole role,
        string? backstory,
        string? appearance,
        string? personality,
        DateTime createdAt,
        DateTime updatedAt)
    {
        var character = new Character
        {
            Id = id,
            ProjectId = projectId,
            Name = name,
            Description = description,
            Role = role,
            Backstory = backstory,
            Appearance = appearance,
            Personality = personality,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        return character;
    }

    // Factory method - encapsulates creation logic
    public static Character Create(
        Guid projectId,
        string name,
        string description,
        CharacterRole role)
    {
        if (projectId == Guid.Empty)
            throw new ArgumentException("ProjectId cannot be empty.", nameof(projectId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Character name cannot be empty.", nameof(name));

        if (name.Length > 100)
            throw new ArgumentException("Character name cannot exceed 100 characters.", nameof(name));

        if (description.Length > 1000)
            throw new ArgumentException("Character description cannot exceed 1000 characters.", nameof(description));

        var character = new Character
        {
            ProjectId = projectId,
            Name = name.Trim(),
            Description = description.Trim(),
            Role = role
        };

        character.AddDomainEvent(new CharacterCreatedEvent(character.Id, projectId, name));

        return character;
    }

    // Behavior methods (not public setters!)
    public void UpdateDetails(string name, string description, CharacterRole role)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Character name cannot be empty.", nameof(name));

        if (name.Length > 100)
            throw new ArgumentException("Character name cannot exceed 100 characters.", nameof(name));

        if (description.Length > 1000)
            throw new ArgumentException("Character description cannot exceed 1000 characters.", nameof(description));

        Name = name.Trim();
        Description = description.Trim();
        Role = role;
        Touch();
    }

    public void UpdateBackstory(string? backstory)
    {
        if (backstory != null && backstory.Length > 5000)
            throw new ArgumentException("Backstory cannot exceed 5000 characters.", nameof(backstory));

        Backstory = backstory?.Trim();
        Touch();
    }

    public void UpdateAppearance(string? appearance)
    {
        if (appearance != null && appearance.Length > 2000)
            throw new ArgumentException("Appearance cannot exceed 2000 characters.", nameof(appearance));

        Appearance = appearance?.Trim();
        Touch();
    }

    public void UpdatePersonality(string? personality)
    {
        if (personality != null && personality.Length > 2000)
            throw new ArgumentException("Personality cannot exceed 2000 characters.", nameof(personality));

        Personality = personality?.Trim();
        Touch();
    }
}
