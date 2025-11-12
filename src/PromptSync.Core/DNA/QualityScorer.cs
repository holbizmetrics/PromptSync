using Microsoft.Extensions.Logging;
using PromptSync.Core.Models;
using PromptSync.Core.Services;
using System.Text.RegularExpressions;

namespace PromptSync.Core.DNA;

/// <summary>
/// Implementation of quality scoring for prompts.
/// Evaluates prompts across 5 dimensions: Clarity, Specificity, Structure, Context, Examples.
/// </summary>
public class QualityScorer : IQualityScorer
{
    private readonly IAIService _aiService;
    private readonly ILogger<QualityScorer> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="QualityScorer"/> class.
    /// </summary>
    /// <param name="aiService">The AI service for detailed scoring.</param>
    /// <param name="logger">The logger instance.</param>
    public QualityScorer(IAIService aiService, ILogger<QualityScorer> logger)
    {
        _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<DnaResult> ScoreAsync(Prompt prompt, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(prompt);

        _logger.LogInformation("Scoring prompt: {PromptTitle}", prompt.Title);

        try
        {
            return await ScoreContentAsync(prompt.Content, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to score prompt: {PromptTitle}", prompt.Title);
            return new DnaResult
            {
                Success = false,
                ErrorMessage = $"Failed to score prompt: {ex.Message}"
            };
        }
    }

    /// <inheritdoc/>
    public async Task<DnaResult> ScoreContentAsync(string content, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Content cannot be null or whitespace.", nameof(content));
        }

        _logger.LogInformation("Scoring prompt content");

        try
        {
            // Get quick heuristic score
            var quickScore = GetQuickScore(content);

            // Get detailed AI-powered scoring
            var aiScoring = await GetAIScoringAsync(content, cancellationToken);

            var breakdown = new QualityBreakdown
            {
                Clarity = aiScoring.Clarity,
                Specificity = aiScoring.Specificity,
                Structure = aiScoring.Structure,
                Context = aiScoring.Context,
                Examples = aiScoring.Examples
            };

            _logger.LogInformation(
                "Quality scored: Overall {Overall:F1}/10 (Quick: {Quick:F1}/10)",
                breakdown.Overall,
                quickScore);

            return new DnaResult
            {
                Success = true,
                QualityScore = breakdown.Overall,
                QualityDetails = breakdown,
                Content = content,
                Metadata = new Dictionary<string, object>
                {
                    ["quick_score"] = quickScore,
                    ["ai_powered"] = true
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to score content");
            
            // Fallback to quick score only
            var quickScore = GetQuickScore(content);
            return new DnaResult
            {
                Success = true,
                QualityScore = quickScore,
                Content = content,
                Metadata = new Dictionary<string, object>
                {
                    ["quick_score"] = quickScore,
                    ["ai_powered"] = false,
                    ["fallback_reason"] = ex.Message
                }
            };
        }
    }

    /// <inheritdoc/>
    public double GetQuickScore(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return 0.0;
        }

        var score = 0.0;

        // Length scoring (optimal: 100-500 chars)
        var length = content.Length;
        if (length >= 100 && length <= 500)
        {
            score += 2.0;
        }
        else if (length >= 50 && length <= 1000)
        {
            score += 1.0;
        }

        // Has clear instruction words
        var instructionWords = new[] { "analyze", "create", "write", "generate", "explain", "summarize", "list" };
        if (instructionWords.Any(word => content.Contains(word, StringComparison.OrdinalIgnoreCase)))
        {
            score += 2.0;
        }

        // Has structure (bullets, numbers, sections)
        if (Regex.IsMatch(content, @"^\s*[-*•]\s", RegexOptions.Multiline) ||
            Regex.IsMatch(content, @"^\s*\d+\.\s", RegexOptions.Multiline))
        {
            score += 1.5;
        }

        // Has variables/placeholders
        if (Regex.IsMatch(content, @"\{\{[^}]+\}\}") || content.Contains("{{") || content.Contains("["))
        {
            score += 1.5;
        }

        // Has context or constraints
        if (Regex.IsMatch(content, @"\b(given|assuming|context|note|important|must|should|constraints?)\b", 
            RegexOptions.IgnoreCase))
        {
            score += 1.5;
        }

        // Has examples
        if (Regex.IsMatch(content, @"\b(example|for instance|such as|e\.g\.|like)\b", 
            RegexOptions.IgnoreCase))
        {
            score += 1.5;
        }

        return Math.Min(score, 10.0);
    }

    private async Task<QualityBreakdown> GetAIScoringAsync(string content, CancellationToken cancellationToken)
    {
        var systemPrompt = @"You are a prompt quality analyzer. Score prompts across 5 dimensions (0-10 scale):
1. Clarity: How clear and unambiguous is the prompt?
2. Specificity: How specific and detailed are the requirements?
3. Structure: How well-organized and formatted is the prompt?
4. Context: How much relevant context is provided?
5. Examples: Are examples or demonstrations included?

Return ONLY a JSON object with numeric scores.";

        var prompt = $@"Score this prompt:

{content}

Return JSON format:
{{
  ""clarity"": 0.0,
  ""specificity"": 0.0,
  ""structure"": 0.0,
  ""context"": 0.0,
  ""examples"": 0.0
}}";

        var result = await _aiService.SendStructuredPromptAsync<QualityBreakdown>(
            prompt,
            systemPrompt,
            cancellationToken);

        if (result == null)
        {
            // Fallback to heuristic scoring
            var quickScore = GetQuickScore(content);
            return new QualityBreakdown
            {
                Clarity = quickScore * 0.2,
                Specificity = quickScore * 0.2,
                Structure = quickScore * 0.2,
                Context = quickScore * 0.2,
                Examples = quickScore * 0.2
            };
        }

        return result;
    }
}
