using Microsoft.Extensions.Logging;
using PromptSync.Core.Models;
using System.Text.RegularExpressions;

namespace PromptSync.Core.DNA;

/// <summary>
/// Minimal harvester that extracts textual content from HTML and creates simple prompts.
/// Designed to be a safe default for unit tests and initial functionality.
/// </summary>
public class Harvester : IHarvester
{
    private readonly ILogger<Harvester> _logger;

    public Harvester(ILogger<Harvester> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<DnaResult> HarvestFromWebAsync(string url, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(url)) throw new ArgumentException("url is required", nameof(url));

        _logger.LogInformation("Harvesting from web: {Url}", url);

        using var http = new HttpClient();
        var html = await http.GetStringAsync(url, cancellationToken);
        return await HarvestFromHtmlAsync(html, url, cancellationToken);
    }

    public Task<DnaResult> HarvestFromHtmlAsync(string html, string? sourceUrl = null, CancellationToken cancellationToken = default)
    {
        if (html == null) throw new ArgumentNullException(nameof(html));

        _logger.LogInformation("Harvesting from HTML (source: {Source})", sourceUrl ?? "<inline>");

        // Very simple HTML tag stripper
        var text = Regex.Replace(html, "<script[^>]*>.*?<\\/script>", string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);
        text = Regex.Replace(text, "<style[^>]*>.*?<\\/style>", string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);
        text = Regex.Replace(text, "<[^>]+>", " ", RegexOptions.Singleline);
        text = Regex.Replace(text, "\\s+", " ").Trim();

        // Create a basic prompt from the text
        var snippet = text.Length > 1000 ? text.Substring(0, 1000) + "..." : text;
        var content = $"Extracted from {sourceUrl ?? "html"}: {snippet}\n\nPlease summarize the important points and identify potential prompts that could be used from this content.";

        var prompt = new Prompt
        {
            Id = Guid.NewGuid().ToString(),
            Title = sourceUrl ?? "Harvested Prompt",
            Content = content
        };

        var result = new DnaResult
        {
            Success = true,
            Content = content,
            Prompt = prompt,
            QualityScore = 0.0,
            Metadata = new Dictionary<string, object>
            {
                ["source"] = sourceUrl ?? string.Empty
            }
        };

        return Task.FromResult(result);
    }

    public Task<DnaResult> CreatePromptFromHarvestAsync(string content, Dictionary<string, string> metadata, CancellationToken cancellationToken = default)
    {
        if (content == null) throw new ArgumentNullException(nameof(content));

        var prompt = new Prompt
        {
            Id = Guid.NewGuid().ToString(),
            Title = metadata != null && metadata.TryGetValue("title", out var t) ? t : "Harvested Prompt",
            Content = content
        };

        var result = new DnaResult
        {
            Success = true,
            Prompt = prompt,
            Content = content,
            Metadata = metadata?.ToDictionary(k => k.Key, v => (object)v) ?? new Dictionary<string, object>()
        };

        return Task.FromResult(result);
    }
}
