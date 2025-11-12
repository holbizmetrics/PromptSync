namespace PromptSync.Core.Services;

/// <summary>
/// Interface for Git repository operations.
/// Single Responsibility: Handle Git synchronization.
/// </summary>
public interface IGitService
{
    /// <summary>
    /// Clones a Git repository to a local path.
    /// </summary>
    /// <param name="repositoryUrl">The URL of the repository.</param>
    /// <param name="localPath">The local path to clone to.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task CloneAsync(string repositoryUrl, string localPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pulls latest changes from the remote repository.
    /// </summary>
    /// <param name="localPath">The local repository path.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task PullAsync(string localPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pushes local changes to the remote repository.
    /// </summary>
    /// <param name="localPath">The local repository path.</param>
    /// <param name="message">The commit message.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task PushAsync(string localPath, string message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits changes in the local repository.
    /// </summary>
    /// <param name="localPath">The local repository path.</param>
    /// <param name="message">The commit message.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task CommitAsync(string localPath, string message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if there are uncommitted changes.
    /// </summary>
    /// <param name="localPath">The local repository path.</param>
    /// <returns>True if there are changes, false otherwise.</returns>
    bool HasChanges(string localPath);

    /// <summary>
    /// Gets the current branch name.
    /// </summary>
    /// <param name="localPath">The local repository path.</param>
    /// <returns>The current branch name.</returns>
    string GetCurrentBranch(string localPath);

    /// <summary>
    /// Checks if a path is a valid Git repository.
    /// </summary>
    /// <param name="localPath">The path to check.</param>
    /// <returns>True if it's a valid repository, false otherwise.</returns>
    bool IsValidRepository(string localPath);
}
