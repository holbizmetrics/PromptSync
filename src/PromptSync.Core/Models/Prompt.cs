namespace PromptSync.Core.Models;

/// <summary>
/// Represents a prompt with metadata and content.
/// Immutable record following Clean Code principles.
/// </summary>
public record Prompt
{
    /// <summary>
    /// Gets the unique identifier for the prompt.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Gets the title of the prompt.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Gets the prompt content/template.
    /// </summary>
    public required string Content { get; init; }

    /// <summary>
    /// Gets the tags for categorization and search.
    /// </summary>
    public IReadOnlyList<string> Tags { get; init; } = Array.Empty<string>();

    /// <summary>
    /// Gets the applications where this prompt is relevant.
    /// </summary>
    public IReadOnlyList<string> Applications { get; init; } = Array.Empty<string>();

    /// <summary>
    /// Gets the patterns that should trigger this prompt.
    /// </summary>
    public IReadOnlyList<string> Patterns { get; init; } = Array.Empty<string>();

    /// <summary>
    /// Gets the file path relative to the prompts repository.
    /// </summary>
    public string? FilePath { get; init; }

    /// <summary>
    /// Gets a value indicating whether the prompt is encrypted.
    /// </summary>
    public bool IsEncrypted { get; init; }

    /// <summary>
    /// Gets the quality score (0-10).
    /// </summary>
    public double? QualityScore { get; init; }

    /// <summary>
    /// Gets the security risk score (0-100, higher = more risk).
    /// </summary>
    public int? SecurityRiskScore { get; init; }

    /// <summary>
    /// Gets the creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Gets the last modified timestamp.
    /// </summary>
    public DateTime ModifiedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Gets custom metadata as key-value pairs.
    /// </summary>
    public IReadOnlyDictionary<string, string> Metadata { get; init; } = 
        new Dictionary<string, string>();
}
