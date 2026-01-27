using AutorLLM.Domain.Entities;

namespace AutorLLM.Domain.Services;

/// <summary>
/// Domain service for validating character consistency across the narrative.
/// Contains business rules for character usage and consistency validation.
/// </summary>
public interface ICharacterConsistencyService
{
    /// <summary>
    /// Validates if a character appears consistently throughout their designated chapters.
    /// Checks for proper character introduction, development, and presence.
    /// </summary>
    /// <param name="character">The character to validate</param>
    /// <param name="chapters">All chapters in the project</param>
    /// <returns>True if the character usage is consistent, false otherwise</returns>
    bool ValidateCharacterConsistency(Character character, IEnumerable<Chapter> chapters);

    /// <summary>
    /// Checks if a character has sufficient presence in the story based on their role.
    /// Protagonists should appear in most chapters, supporting characters less frequently.
    /// </summary>
    /// <param name="character">The character to check</param>
    /// <param name="totalChapters">Total number of chapters in the project</param>
    /// <returns>True if the character has adequate presence for their role</returns>
    bool HasAdequatePresence(Character character, int totalChapters);

    /// <summary>
    /// Identifies chapters where a character should appear but might be missing.
    /// Useful for suggesting where characters need more development.
    /// </summary>
    /// <param name="character">The character to analyze</param>
    /// <param name="chapters">All chapters in the project</param>
    /// <returns>List of chapter orders where the character should potentially appear</returns>
    List<int> SuggestChaptersForCharacter(Character character, IEnumerable<Chapter> chapters);
}
