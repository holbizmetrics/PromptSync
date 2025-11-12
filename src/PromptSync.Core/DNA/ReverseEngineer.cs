using Microsoft.Extensions.Logging;
using PromptSync.Core.Models;

namespace PromptSync.Core.DNA;

/// <summary>
/// Minimal reverse engineer implementation. For images this class currently does not perform OCR
/// but provides the method stubs and safe defaults for unit testing and incremental development.
/// </summary>
public class ReverseEngineer : IReverseEngineer
{
    private readonly ILogger<ReverseEngineer> _logger;

    public ReverseEngineer(ILogger<ReverseEngineer> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<DnaResult> FromImageAsync(string imagePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(imagePath)) throw new ArgumentException("imagePath is required", nameof(imagePath));

        _logger.LogInformation("Reverse engineering from image: {Path}", imagePath);

        // Placeholder: real OCR would be implemented later
        var content = $"[Extracted text from image at {imagePath}]. Replace with real OCR implementation.";
        var prompt = new Prompt { Id = Guid.NewGuid().ToString(), Title = "FromImage", Content = content };
        return Task.FromResult(new DnaResult { Success = true, Prompt = prompt, Content = content });
    }

    public Task<DnaResult> FromTextAsync(string text, CancellationToken cancellationToken = default)
    {
        if (text == null) throw new ArgumentNullException(nameof(text));

        _logger.LogInformation("Reverse engineering from text");
        var prompt = new Prompt { Id = Guid.NewGuid().ToString(), Title = "FromText", Content = text };
        return Task.FromResult(new DnaResult { Success = true, Prompt = prompt, Content = text });
    }

    public async Task<DnaResult> FromUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(url)) throw new ArgumentException("url is required", nameof(url));

        _logger.LogInformation("Reverse engineering from URL: {Url}", url);

        using var http = new HttpClient();
        var html = await http.GetStringAsync(url, cancellationToken);

        // Very simple text extraction
        var text = System.Text.RegularExpressions.Regex.Replace(html, "<[^>]+>", " ");
        text = System.Text.RegularExpressions.Regex.Replace(text, "\\s+", " ").Trim();

        var snippet = text.Length > 1000 ? text.Substring(0, 1000) + "..." : text;
        var content = $"[Extracted from {url}]: {snippet}";
        var prompt = new Prompt { Id = Guid.NewGuid().ToString(), Title = url, Content = content };

        return new DnaResult { Success = true, Prompt = prompt, Content = content };
    }
}
