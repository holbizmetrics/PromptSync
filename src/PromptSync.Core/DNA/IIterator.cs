using PromptSync.Core.Models;

namespace PromptSync.Core.DNA;

/// <summary>
/// Interface for iteratively improving prompts through AI-powered refinement.
/// Single Responsibility: Auto-iterate prompts to improve quality.
/// </summary>
public interface IIterator
{
    /// <summary>
    /// Iteratively improves a prompt through multiple refinement cycles.
    /// </summary>
    /// <param name="topic">The topic or domain for the prompt.</param>
    /// <param name="question">The specific question or task.</param>
    /// <param name="maxIterations">Maximum number of iterations (default: 5).</param>
    /// <param name="targetQuality">Target quality score to achieve (default: 8.5).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A DNA result with the improved prompt and metrics.</returns>
    Task<DnaResult> IterateAsync(
        string topic,
        string question,
        int maxIterations = 5,
        double targetQuality = 8.5,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Improves an existing prompt template.
    /// </summary>
    /// <param name="existingPrompt">The existing prompt to improve.</param>
    /// <param name="maxIterations">Maximum number of iterations (default: 5).</param>
    /// <param name="targetQuality">Target quality score to achieve (default: 8.5).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A DNA result with the improved prompt and metrics.</returns>
    Task<DnaResult> ImproveAsync(
        Prompt existingPrompt,
        int maxIterations = 5,
        double targetQuality = 8.5,
        CancellationToken cancellationToken = default);
}
