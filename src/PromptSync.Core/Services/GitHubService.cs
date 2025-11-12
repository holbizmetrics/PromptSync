using Microsoft.Extensions.Logging;
using PromptSync.Core.Models;

namespace PromptSync.Core.Services;

/// <summary>
/// Minimal GitHub service wrapper. For initial development this is a stub that mimics responses.
/// Replace with an Octokit-backed implementation for full GitHub integration.
/// </summary>
public class GitHubService : IGitHubService
{
    private readonly ILogger<GitHubService> _logger;

    public GitHubService(ILogger<GitHubService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<string> CreateRepositoryAsync(string name, string? description = null, bool isPrivate = true, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock create repository: {Name}", name);
        return Task.FromResult($"https://github.com/youruser/{name}");
    }

    public Task<IReadOnlyList<string>> ListRepositoriesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock list repositories");
        return Task.FromResult((IReadOnlyList<string>)new List<string> { "promptsync-prompts", "sample-repo" });
    }

    public Task<bool> RepositoryExistsAsync(string owner, string name, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock check repository exists: {Owner}/{Name}", owner, name);
        return Task.FromResult(true);
    }

    public Task<string> GetAuthenticatedUserAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock get authenticated user");
        return Task.FromResult("mock-user");
    }

    public Task<bool> IsTokenValidAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock token validation");
        return Task.FromResult(true);
    }
}
