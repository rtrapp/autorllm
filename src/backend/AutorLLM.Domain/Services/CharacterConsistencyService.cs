using AutorLLM.Domain.Entities;
using AutorLLM.Domain.ValueObjects;

namespace AutorLLM.Domain.Services;

/// <summary>
/// Domain service implementation for character consistency validation.
/// Stateless service - no internal state, pure business logic.
/// </summary>
public class CharacterConsistencyService : ICharacterConsistencyService
{
    public bool ValidateCharacterConsistency(Character character, IEnumerable<Chapter> chapters)
    {
        if (character == null)
            throw new ArgumentNullException(nameof(character));

        if (chapters == null)
            throw new ArgumentNullException(nameof(chapters));

        var chapterList = chapters.ToList();
        var totalChapters = chapterList.Count;

        if (totalChapters == 0)
            return true; // No chapters to validate against

        // For now, we return true as we don't have chapter appearances tracking yet
        // This will be implemented when we add character tracking to chapters
        return true;
    }

    public bool HasAdequatePresence(Character character, int totalChapters)
    {
        if (character == null)
            throw new ArgumentNullException(nameof(character));

        if (totalChapters <= 0)
            return true;

        // For now, we return true as we don't have chapter appearances tracking yet
        // This method will be fully implemented when we add character tracking to chapters
        // The business logic for minimum presence based on role is defined below:
        // - Protagonist: 70% of chapters
        // - Antagonist: 40% of chapters
        // - Supporting: 25% of chapters
        // - Minor: 10% of chapters
        return true;
    }

    public List<int> SuggestChaptersForCharacter(Character character, IEnumerable<Chapter> chapters)
    {
        if (character == null)
            throw new ArgumentNullException(nameof(character));

        if (chapters == null)
            throw new ArgumentNullException(nameof(chapters));

        var suggestions = new List<int>();
        var chapterList = chapters.OrderBy(c => c.Order.Value).ToList();

        if (!chapterList.Any())
            return suggestions;

        var totalChapters = chapterList.Count;

        // Suggest chapters based on role
        if (character.Role == CharacterRole.Protagonist)
        {
            // Protagonists should appear in most chapters - suggest all
            for (int i = 1; i <= totalChapters; i++)
            {
                suggestions.Add(i);
            }
        }
        else if (character.Role == CharacterRole.Antagonist)
        {
            // Antagonists should appear periodically, especially in climax
            var climaxChapter = (int)Math.Ceiling(totalChapters * 0.75);

            // Suggest early appearance (introduction)
            suggestions.Add(2);

            // Suggest middle chapter
            var middleChapter = totalChapters / 2;
            if (middleChapter > 2)
                suggestions.Add(middleChapter);

            // Suggest climax chapter
            if (climaxChapter > middleChapter)
                suggestions.Add(climaxChapter);
        }
        else if (character.Role == CharacterRole.Supporting)
        {
            // Supporting characters should appear in key moments
            var keyMoments = new[]
            {
                (int)Math.Ceiling(totalChapters * 0.25),  // Early
                (int)Math.Ceiling(totalChapters * 0.5),   // Middle
                (int)Math.Ceiling(totalChapters * 0.75)   // Climax
            };

            suggestions.AddRange(keyMoments);
        }

        return suggestions.Distinct().OrderBy(x => x).ToList();
    }

    private double GetMinimumPresencePercentage(CharacterRole role)
    {
        // Define minimum presence based on character role
        if (role == CharacterRole.Protagonist)
            return 0.7; // 70% of chapters

        if (role == CharacterRole.Antagonist)
            return 0.4; // 40% of chapters

        if (role == CharacterRole.Supporting)
            return 0.25; // 25% of chapters

        // Minor characters
        return 0.1; // 10% of chapters
    }
}
