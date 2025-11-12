using PromptSync.Core.Models;

namespace PromptSync.Core.Services;

/// <summary>
/// Interface for AI service operations (Claude, GPT, etc.).
/// Single Responsibility: Handle AI API communication.
/// </summary>
public interface IAIService
{
    /// <summary>
    /// Sends a prompt to the AI service and gets a response.
    /// </summary>
    /// <param name="prompt">The prompt to send.</param>
    /// <param name="systemPrompt">Optional system prompt for context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The AI's response.</returns>
    Task<string> SendPromptAsync(
        string prompt,
        string? systemPrompt = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a prompt with structured output (JSON).
    /// </summary>
    /// <typeparam name="T">The type to deserialize the response to.</typeparam>
    /// <param name="prompt">The prompt to send.</param>
    /// <param name="systemPrompt">Optional system prompt for context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The structured response.</returns>
    Task<T?> SendStructuredPromptAsync<T>(
        string prompt,
        string? systemPrompt = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the service is available and responding.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if available, false otherwise.</returns>
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the name of the AI service provider.
    /// </summary>
    string ProviderName { get; }
}
