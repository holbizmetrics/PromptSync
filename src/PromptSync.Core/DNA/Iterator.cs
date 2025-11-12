using Microsoft.Extensions.Logging;
using PromptSync.Core.Models;
using PromptSync.Core.Services;

namespace PromptSync.Core.DNA;

/// <summary>
/// Basic implementation of IIterator that performs iterative prompt improvements using an AI service
/// and a quality scorer. This is a safe, minimal implementation suitable for unit testing and as a
/// basis for more advanced iteration strategies.
/// </summary>
public class Iterator : IIterator
{
    private readonly IAIService _aiService;
    private readonly IQualityScorer _qualityScorer;
    private readonly ILogger<Iterator> _logger;

    public Iterator(IAIService aiService, IQualityScorer qualityScorer, ILogger<Iterator> logger)
    {
        _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
        _qualityScorer = qualityScorer ?? throw new ArgumentNullException(nameof(qualityScorer));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<DnaResult> IterateAsync(
        string topic,
        string question,
        int maxIterations = 5,
        double targetQuality = 8.5,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(topic)) throw new ArgumentException("topic is required", nameof(topic));
        if (string.IsNullOrWhiteSpace(question)) throw new ArgumentException("question is required", nameof(question));

        _logger.LogInformation("Starting iteration for topic: {Topic}", topic);

        var basePrompt = $"Topic: {topic}\nTask: {question}\nImprove this prompt to maximize clarity and usefulness.";
        var lastContent = basePrompt;
        double initialScore = 0;
        double finalScore = 0;

        for (var i = 1; i <= maxIterations; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogDebug("Iteration {Iteration} — sending to AI", i);
            string aiResponse;
            try
            {
                aiResponse = await _aiService.SendPromptAsync(lastContent, null, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI service failed during iteration {Iteration}", i);
                return new DnaResult { Success = false, ErrorMessage = "AI service failure during iteration: " + ex.Message };
            }

            var scoreResult = await _qualityScorer.ScoreContentAsync(aiResponse, cancellationToken);
            finalScore = scoreResult.QualityScore ?? 0.0;
            if (i == 1) initialScore = finalScore;

            _logger.LogInformation("Iteration {Iteration} — score: {Score:F2}", i, finalScore);

            lastContent = aiResponse;

            if (finalScore >= targetQuality)
            {
                _logger.LogInformation("Target quality reached at iteration {Iteration}", i);
                return new DnaResult
                {
                    Success = true,
                    Content = lastContent,
                    QualityScore = finalScore,
                    IterationCount = i,
                    Improvement = finalScore - initialScore,
                    Prompt = new Prompt
                    {
                        Id = Guid.NewGuid().ToString(),
                        Title = $"Iterated: {topic}",
                        Content = lastContent
                    }
                };
            }
        }

        // Completed all iterations, return best-effort result
        return new DnaResult
        {
            Success = true,
            Content = lastContent,
            QualityScore = finalScore,
            IterationCount = maxIterations,
            Improvement = finalScore - initialScore,
            Prompt = new Prompt
            {
                Id = Guid.NewGuid().ToString(),
                Title = $"Iterated (final): {topic}",
                Content = lastContent
            }
        };
    }

    public async Task<DnaResult> ImproveAsync(Prompt existingPrompt, int maxIterations = 5, double targetQuality = 8.5, CancellationToken cancellationToken = default)
    {
        if (existingPrompt == null) throw new ArgumentNullException(nameof(existingPrompt));

        return await IterateAsync(existingPrompt.Title ?? "Improvement", existingPrompt.Content, maxIterations, targetQuality, cancellationToken);
    }
}
