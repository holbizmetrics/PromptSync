using Microsoft.Extensions.Logging;

namespace PromptSync.Core.Services;

/// <summary>
/// Mock Git service for testing UI without real Git operations.
/// Replace with real GitService implementation later.
/// </summary>
public class MockGitService : IGitService
{
    private readonly ILogger<MockGitService>? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MockGitService"/> class.
    /// </summary>
    /// <param name="logger">Optional logger instance.</param>
    public MockGitService(ILogger<MockGitService>? logger = null)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public Task CloneAsync(string repositoryUrl, string localPath, CancellationToken cancellationToken = default)
    {
        _logger?.LogInformation("Mock: Clone {Url} to {Path}", repositoryUrl, localPath);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task PullAsync(string localPath, CancellationToken cancellationToken = default)
    {
        _logger?.LogInformation("Mock: Pull from {Path}", localPath);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task PushAsync(string localPath, string message, CancellationToken cancellationToken = default)
    {
        _logger?.LogInformation("Mock: Push to {Path} with message: {Message}", localPath, message);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task CommitAsync(string localPath, string message, CancellationToken cancellationToken = default)
    {
        _logger?.LogInformation("Mock: Commit at {Path} with message: {Message}", localPath, message);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public bool HasChanges(string localPath)
    {
        _logger?.LogDebug("Mock: Checking for changes at {Path}", localPath);
        return false; // No changes in mock
    }

    /// <inheritdoc/>
    public string GetCurrentBranch(string localPath)
    {
        _logger?.LogDebug("Mock: Getting current branch at {Path}", localPath);
        return "main";
    }

    /// <inheritdoc/>
    public bool IsValidRepository(string localPath)
    {
        _logger?.LogDebug("Mock: Validating repository at {Path}", localPath);
        return true;
    }
}
