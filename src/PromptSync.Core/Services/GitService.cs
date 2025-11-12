using Microsoft.Extensions.Logging;
using PromptSync.Core.Exceptions;

namespace PromptSync.Core.Services;

/// <summary>
/// Minimal Git service that wraps basic file-based operations. This implementation does not
/// depend on LibGit2Sharp to keep the core lightweight for initial development and testing.
/// Replace with a LibGit2Sharp-backed implementation for production features.
/// </summary>
public class GitService : IGitService
{
    private readonly ILogger<GitService> _logger;

    public GitService(ILogger<GitService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task CloneAsync(string repositoryUrl, string localPath, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock clone from {Url} to {Path}", repositoryUrl, localPath);
        // For now, just create the directory
        try
        {
            Directory.CreateDirectory(localPath);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            throw new GitSyncException("Failed to clone repository", ex);
        }
    }

    public Task PullAsync(string localPath, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock pull at {Path}", localPath);
        if (!Directory.Exists(localPath)) throw new GitSyncException("Local repository path does not exist");
        return Task.CompletedTask;
    }

    public Task PushAsync(string localPath, string message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock push at {Path} with message {Message}", localPath, message);
        if (!Directory.Exists(localPath)) throw new GitSyncException("Local repository path does not exist");
        return Task.CompletedTask;
    }

    public Task CommitAsync(string localPath, string message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock commit at {Path} with message {Message}", localPath, message);
        if (!Directory.Exists(localPath)) throw new GitSyncException("Local repository path does not exist");
        return Task.CompletedTask;
    }

    public bool HasChanges(string localPath)
    {
        _logger.LogDebug("Mock has changes check at {Path}", localPath);
        return false;
    }

    public string GetCurrentBranch(string localPath)
    {
        return "main";
    }

    public bool IsValidRepository(string localPath)
    {
        return Directory.Exists(localPath);
    }
}
