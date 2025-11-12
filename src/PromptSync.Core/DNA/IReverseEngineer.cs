using PromptSync.Core.Models;

namespace PromptSync.Core.DNA;

/// <summary>
/// Interface for reverse engineering prompts from various sources.
/// Single Responsibility: Extract prompts from images, text, or web content.
/// </summary>
public interface IReverseEngineer
{
    /// <summary>
    /// Extracts a prompt from an image file.
    /// </summary>
    /// <param name="imagePath">The path to the image file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A DNA result containing the extracted prompt.</returns>
    Task<DnaResult> FromImageAsync(string imagePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Extracts a prompt from text content.
    /// </summary>
    /// <param name="text">The text content to analyze.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A DNA result containing the extracted prompt.</returns>
    Task<DnaResult> FromTextAsync(string text, CancellationToken cancellationToken = default);

    /// <summary>
    /// Extracts a prompt from a web URL.
    /// </summary>
    /// <param name="url">The URL to extract from.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A DNA result containing the extracted prompt.</returns>
    Task<DnaResult> FromUrlAsync(string url, CancellationToken cancellationToken = default);
}
