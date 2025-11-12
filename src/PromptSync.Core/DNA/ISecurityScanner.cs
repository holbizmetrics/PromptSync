using PromptSync.Core.Models;

namespace PromptSync.Core.DNA;

/// <summary>
/// Interface for scanning prompts for security vulnerabilities.
/// Single Responsibility: Detect security risks in prompts.
/// </summary>
public interface ISecurityScanner
{
    /// <summary>
    /// Scans a prompt for security vulnerabilities.
    /// </summary>
    /// <param name="prompt">The prompt to scan.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A DNA result with security analysis.</returns>
    Task<DnaResult> ScanAsync(Prompt prompt, CancellationToken cancellationToken = default);

    /// <summary>
    /// Scans prompt content for security vulnerabilities.
    /// </summary>
    /// <param name="content">The prompt content to scan.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A DNA result with security analysis.</returns>
    Task<DnaResult> ScanContentAsync(string content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a prompt is safe to execute.
    /// </summary>
    /// <param name="prompt">The prompt to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the prompt is safe, false otherwise.</returns>
    Task<bool> IsSafeAsync(Prompt prompt, CancellationToken cancellationToken = default);
}
