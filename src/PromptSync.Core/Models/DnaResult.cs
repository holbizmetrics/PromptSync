namespace PromptSync.Core.Models;

/// <summary>
/// Represents the result of a DNA Lab analysis operation.
/// </summary>
public record DnaResult
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public required bool Success { get; init; }

    /// <summary>
    /// Gets the resulting prompt (if applicable).
    /// </summary>
    public Prompt? Prompt { get; init; }

    /// <summary>
    /// Gets the extracted or generated content.
    /// </summary>
    public string? Content { get; init; }

    /// <summary>
    /// Gets the quality score (0-10).
    /// </summary>
    public double? QualityScore { get; init; }

    /// <summary>
    /// Gets detailed quality breakdown by dimension.
    /// </summary>
    public QualityBreakdown? QualityDetails { get; init; }

    /// <summary>
    /// Gets the security risk score (0-100).
    /// </summary>
    public int? SecurityRiskScore { get; init; }

    /// <summary>
    /// Gets the security risk level.
    /// </summary>
    public SecurityRiskLevel? RiskLevel { get; init; }

    /// <summary>
    /// Gets the list of detected security vulnerabilities.
    /// </summary>
    public IReadOnlyList<SecurityVulnerability> Vulnerabilities { get; init; } = 
        Array.Empty<SecurityVulnerability>();

    /// <summary>
    /// Gets the number of iterations performed (for iterator).
    /// </summary>
    public int? IterationCount { get; init; }

    /// <summary>
    /// Gets the improvement achieved (for iterator).
    /// </summary>
    public double? Improvement { get; init; }

    /// <summary>
    /// Gets the confidence level (0-100).
    /// </summary>
    public int? Confidence { get; init; }

    /// <summary>
    /// Gets the error message if the operation failed.
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Gets additional metadata about the operation.
    /// </summary>
    public IReadOnlyDictionary<string, object> Metadata { get; init; } = 
        new Dictionary<string, object>();
}

/// <summary>
/// Represents a breakdown of quality scores by dimension.
/// </summary>
public record QualityBreakdown
{
    /// <summary>
    /// Gets the clarity score (0-10).
    /// </summary>
    public required double Clarity { get; init; }

    /// <summary>
    /// Gets the specificity score (0-10).
    /// </summary>
    public required double Specificity { get; init; }

    /// <summary>
    /// Gets the structure score (0-10).
    /// </summary>
    public required double Structure { get; init; }

    /// <summary>
    /// Gets the context score (0-10).
    /// </summary>
    public required double Context { get; init; }

    /// <summary>
    /// Gets the examples score (0-10).
    /// </summary>
    public required double Examples { get; init; }

    /// <summary>
    /// Gets the overall average score.
    /// </summary>
    public double Overall => (Clarity + Specificity + Structure + Context + Examples) / 5.0;
}

/// <summary>
/// Represents a detected security vulnerability.
/// </summary>
public record SecurityVulnerability
{
    /// <summary>
    /// Gets the type of vulnerability.
    /// </summary>
    public required string Type { get; init; }

    /// <summary>
    /// Gets the severity level.
    /// </summary>
    public required SecuritySeverity Severity { get; init; }

    /// <summary>
    /// Gets the description of the vulnerability.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Gets the location in the prompt where the issue was found.
    /// </summary>
    public string? Location { get; init; }

    /// <summary>
    /// Gets the recommendation for fixing the vulnerability.
    /// </summary>
    public string? Recommendation { get; init; }
}

/// <summary>
/// Security risk levels for prompts.
/// </summary>
public enum SecurityRiskLevel
{
    /// <summary>
    /// No security risks detected.
    /// </summary>
    Safe,

    /// <summary>
    /// Low security risk.
    /// </summary>
    Low,

    /// <summary>
    /// Moderate security risk.
    /// </summary>
    Moderate,

    /// <summary>
    /// High security risk.
    /// </summary>
    High,

    /// <summary>
    /// Critical security risk.
    /// </summary>
    Critical
}

/// <summary>
/// Security severity levels for vulnerabilities.
/// </summary>
public enum SecuritySeverity
{
    /// <summary>
    /// Informational only.
    /// </summary>
    Info,

    /// <summary>
    /// Low severity.
    /// </summary>
    Low,

    /// <summary>
    /// Medium severity.
    /// </summary>
    Medium,

    /// <summary>
    /// High severity.
    /// </summary>
    High,

    /// <summary>
    /// Critical severity.
    /// </summary>
    Critical
}
