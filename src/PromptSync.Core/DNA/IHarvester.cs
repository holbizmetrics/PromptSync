using PromptSync.Core.Models;

namespace PromptSync.Core.DNA;

/// <summary>
/// Interface for harvesting prompts from web pages and other sources.
/// Single Responsibility: Extract and convert web content to prompts.
/// </summary>
public interface IHarvester
{
    /// <summary>
    /// Harvests content from a web URL and creates a prompt.
    /// </summary>
    /// <param name="url">The URL to harvest from.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A DNA result with the harvested prompt.</returns>
    Task<DnaResult> HarvestFromWebAsync(string url, CancellationToken cancellationToken = default);

    /// <summary>
    /// Harvests content from an HTML string and creates a prompt.
    /// </summary>
    /// <param name="html">The HTML content to harvest.</param>
    /// <param name="sourceUrl">Optional source URL for context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A DNA result with the harvested prompt.</returns>
    Task<DnaResult> HarvestFromHtmlAsync(
        string html,
        string? sourceUrl = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a prompt from harvested data.
    /// </summary>
    /// <param name="content">The harvested content.</param>
    /// <param name="metadata">Metadata about the source.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A DNA result with the created prompt.</returns>
    Task<DnaResult> CreatePromptFromHarvestAsync(
        string content,
        Dictionary<string, string> metadata,
        CancellationToken cancellationToken = default);
}
