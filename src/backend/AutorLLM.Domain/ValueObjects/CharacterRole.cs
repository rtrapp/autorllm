using AutorLLM.Domain.Common;

namespace AutorLLM.Domain.ValueObjects;

/// <summary>
/// Represents the role of a character in the story.
/// Value object - immutable and defined by its value.
/// </summary>
public sealed class CharacterRole : ValueObject
{
    public string Value { get; }

    // Predefined common roles
    public static readonly CharacterRole Protagonist = new("Protagonist");
    public static readonly CharacterRole Antagonist = new("Antagonist");
    public static readonly CharacterRole Supporting = new("Supporting");
    public static readonly CharacterRole Minor = new("Minor");

    private CharacterRole(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Character role cannot be empty.", nameof(value));

        if (value.Length > 50)
            throw new ArgumentException("Character role cannot exceed 50 characters.", nameof(value));

        Value = value.Trim();
    }

    public static CharacterRole Create(string value)
    {
        return new CharacterRole(value);
    }

    public static CharacterRole FromString(string value)
    {
        // Check if it matches predefined roles
        if (value.Equals("Protagonist", StringComparison.OrdinalIgnoreCase))
            return Protagonist;
        if (value.Equals("Antagonist", StringComparison.OrdinalIgnoreCase))
            return Antagonist;
        if (value.Equals("Supporting", StringComparison.OrdinalIgnoreCase))
            return Supporting;
        if (value.Equals("Minor", StringComparison.OrdinalIgnoreCase))
            return Minor;

        // Create custom role
        return Create(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
