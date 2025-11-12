using Microsoft.Extensions.Logging;

namespace PromptSync.Core.Services;

/// <summary>
/// Mock AI service for testing UI without real AI API calls.
/// Replace with real ClaudeService implementation later.
/// </summary>
public class MockAIService : IAIService
{
    private readonly ILogger<MockAIService>? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MockAIService"/> class.
    /// </summary>
    /// <param name="logger">Optional logger instance.</param>
    public MockAIService(ILogger<MockAIService>? logger = null)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public string ProviderName => "Mock AI Service";

    /// <inheritdoc/>
    public Task<string> SendPromptAsync(
        string prompt,
        string? systemPrompt = null,
        CancellationToken cancellationToken = default)
    {
        _logger?.LogInformation("Mock: Sending prompt (length: {Length} chars)", prompt.Length);
        
        // Return a mock response
        var response = "This is a mock AI response. Replace MockAIService with ClaudeService for real AI functionality.";
        return Task.FromResult(response);
    }

    /// <inheritdoc/>
    public Task<T?> SendStructuredPromptAsync<T>(
        string prompt,
        string? systemPrompt = null,
        CancellationToken cancellationToken = default)
    {
        _logger?.LogInformation("Mock: Sending structured prompt for type {Type}", typeof(T).Name);
        
        // Return default value for the type
        return Task.FromResult<T?>(default);
    }

    /// <inheritdoc/>
    public Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Mock: Checking availability");
        return Task.FromResult(true);
    }
}
