using AutorLLM.Domain.Entities;

namespace AutorLLM.Domain.Services;

/// <summary>
/// Domain service for validating plot progression logic.
/// Contains business rules that span multiple entities and don't belong to a single entity.
/// </summary>
public interface IPlotProgressionService
{
    /// <summary>
    /// Validates if a plot has a proper narrative progression structure.
    /// Checks for proper story arc with beginning, middle, climax, and resolution.
    /// </summary>
    /// <param name="plot">The plot to validate</param>
    /// <param name="chapters">All chapters in the project</param>
    /// <returns>True if the plot progression is valid, false otherwise</returns>
    bool ValidatePlotProgression(Plot plot, IEnumerable<Chapter> chapters);

    /// <summary>
    /// Calculates the overall intensity progression of a plot across chapters.
    /// Returns a normalized intensity value (0-100) for each chapter.
    /// </summary>
    /// <param name="plot">The plot to analyze</param>
    /// <returns>Dictionary mapping chapter order to intensity percentage</returns>
    Dictionary<int, double> CalculateIntensityProgression(Plot plot);

    /// <summary>
    /// Identifies the climax point of the plot (chapter with highest intensity).
    /// </summary>
    /// <param name="plot">The plot to analyze</param>
    /// <returns>The chapter order where the climax occurs, or null if no plot points exist</returns>
    int? FindClimaxChapter(Plot plot);
}
