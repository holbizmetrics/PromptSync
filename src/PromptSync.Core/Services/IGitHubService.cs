using PromptSync.Core.Models;

namespace PromptSync.Core.Services;

/// <summary>
/// Interface for GitHub API operations.
/// Single Responsibility: Handle GitHub-specific features.
/// </summary>
public interface IGitHubService
{
    /// <summary>
    /// Creates a new repository on GitHub.
    /// </summary>
    /// <param name="name">The repository name.</param>
    /// <param name="description">Optional description.</param>
    /// <param name="isPrivate">Whether the repository should be private.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The URL of the created repository.</returns>
    Task<string> CreateRepositoryAsync(
        string name,
        string? description = null,
        bool isPrivate = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all repositories for the authenticated user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of repository names.</returns>
    Task<IReadOnlyList<string>> ListRepositoriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a repository exists.
    /// </summary>
    /// <param name="owner">The repository owner.</param>
    /// <param name="name">The repository name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the repository exists, false otherwise.</returns>
    Task<bool> RepositoryExistsAsync(
        string owner,
        string name,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the authenticated user's information.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The username of the authenticated user.</returns>
    Task<string> GetAuthenticatedUserAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the GitHub token is valid.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if valid, false otherwise.</returns>
    Task<bool> IsTokenValidAsync(CancellationToken cancellationToken = default);
}
