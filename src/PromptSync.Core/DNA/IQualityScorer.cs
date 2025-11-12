using PromptSync.Core.Models;

namespace PromptSync.Core.DNA;

/// <summary>
/// Interface for scoring prompt quality across multiple dimensions.
/// Single Responsibility: Evaluate prompt quality.
/// </summary>
public interface IQualityScorer
{
    /// <summary>
    /// Scores a prompt across all quality dimensions.
    /// </summary>
    /// <param name="prompt">The prompt to score.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A DNA result with quality scores.</returns>
    Task<DnaResult> ScoreAsync(Prompt prompt, CancellationToken cancellationToken = default);

    /// <summary>
    /// Scores prompt content across all quality dimensions.
    /// </summary>
    /// <param name="content">The prompt content to score.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A DNA result with quality scores.</returns>
    Task<DnaResult> ScoreContentAsync(string content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a quick quality estimate without detailed analysis.
    /// </summary>
    /// <param name="content">The prompt content.</param>
    /// <returns>A quality score from 0-10.</returns>
    double GetQuickScore(string content);
}
