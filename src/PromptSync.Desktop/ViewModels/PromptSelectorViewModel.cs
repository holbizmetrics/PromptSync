using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PromptSync.Core.Models;
using PromptSync.Core.Services;
using System.Collections.ObjectModel;
using System.Linq;
using TextCopy;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace PromptSync.Desktop.ViewModels;

/// <summary>
/// ViewModel for the prompt selector window.
/// Handles prompt search, selection, and context-aware suggestions.
/// Follows MVVM pattern with CommunityToolkit.Mvvm.
/// </summary>
public partial class PromptSelectorViewModel : ViewModelBase
{
    private readonly IGitService _gitService;
    private readonly IAIService _aiService;

    /// <summary>
    /// Event raised when the ViewModel requests the selector to close.
    /// Parameter: selected prompt or null when cancelled.
    /// </summary>
    public event Action<Prompt?>? RequestClose;

    /// <summary>
    /// Event raised when the ViewModel requests to edit a prompt.
    /// </summary>
    public event Action<Prompt>? RequestEdit;

    /// <summary>
    /// Gets or sets the search query.
    /// </summary>
    [ObservableProperty]
    private string _searchQuery = string.Empty;

    /// <summary>
    /// Gets or sets the selected prompt.
    /// </summary>
    [ObservableProperty]
    private Prompt? _selectedPrompt;

    /// <summary>
    /// Gets or sets a value indicating whether a prompt is selected.
    /// Bound to the Select button's IsEnabled.
    /// </summary>
    [ObservableProperty]
    private bool _hasSelection;

    /// <summary>
    /// Gets or sets a value indicating whether the selector is loading.
    /// </summary>
    [ObservableProperty]
    private bool _isLoading;

    /// <summary>
    /// Gets or sets the status message.
    /// </summary>
    [ObservableProperty]
    private string _statusMessage = "Ready";

    /// <summary>
    /// Gets the collection of available prompts.
    /// </summary>
    public ObservableCollection<Prompt> Prompts { get; } = new();

    /// <summary>
    /// Gets the collection of filtered prompts based on search.
    /// </summary>
    public ObservableCollection<Prompt> FilteredPrompts { get; } = new();

    private readonly string _promptsFolder;

    /// <summary>
    /// Initializes a new instance of the <see cref="PromptSelectorViewModel"/> class.
    /// </summary>
    /// <param name="gitService">The Git service.</param>
    /// <param name="aiService">The AI service.</param>
    public PromptSelectorViewModel(IGitService gitService, IAIService aiService)
    {
        _gitService = gitService ?? throw new ArgumentNullException(nameof(gitService));
        _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));

        _promptsFolder = Path.Combine(AppContext.BaseDirectory, "prompts");

        // Load prompts from disk if available, otherwise use samples
        if (Directory.Exists(_promptsFolder) && Directory.EnumerateFiles(_promptsFolder, "*.md").Any())
        {
            LoadPromptsFromFolder();
        }
        else
        {
            LoadSamplePrompts();
        }
    }

    /// <summary>
    /// Loads prompts from the repository.
    /// </summary>
    [RelayCommand]
    private async Task LoadPromptsAsync()
    {
        IsLoading = true;
        StatusMessage = "Loading prompts...";

        try
        {
            await Task.Delay(200); // small placeholder
            // If prompts folder exists, reload
            if (Directory.Exists(_promptsFolder))
            {
                LoadPromptsFromFolder();
            }

            StatusMessage = $"Loaded {Prompts.Count} prompts";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading prompts: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void LoadPromptsFromFolder()
    {
        Prompts.Clear();
        FilteredPrompts.Clear();

        foreach (var file in Directory.EnumerateFiles(_promptsFolder, "*.md"))
        {
            try
            {
                var text = File.ReadAllText(file, Encoding.UTF8);
                var (meta, body) = ParseFrontMatter(text);

                var prompt = new Prompt
                {
                    Id = meta.ContainsKey("id") ? meta["id"] : Path.GetFileNameWithoutExtension(file),
                    Title = meta.ContainsKey("title") ? meta["title"] : Path.GetFileNameWithoutExtension(file),
                    Content = body,
                    Tags = meta.ContainsKey("tags") ? meta["tags"].Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToArray() : Array.Empty<string>(),
                    Applications = meta.ContainsKey("apps") ? meta["apps"].Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToArray() : Array.Empty<string>(),
                    FilePath = file
                };

                Prompts.Add(prompt);
                FilteredPrompts.Add(prompt);
            }
            catch
            {
                // ignore parse errors
            }
        }

        StatusMessage = $"Loaded {Prompts.Count} prompts from folder";
    }

    private static (Dictionary<string, string> meta, string body) ParseFrontMatter(string text)
    {
        var meta = new Dictionary<string, string>();
        if (!text.TrimStart().StartsWith("---"))
        {
            return (meta, text);
        }

        var m = Regex.Match(text, "^---\r?\n(.*?)\r?\n---\r?\n(.*)$", RegexOptions.Singleline);
        if (!m.Success) return (meta, text);

        var fm = m.Groups[1].Value;
        var body = m.Groups[2].Value;

        foreach (var line in fm.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
        {
            var idx = line.IndexOf(':');
            if (idx <= 0) continue;
            var key = line.Substring(0, idx).Trim();
            var val = line.Substring(idx + 1).Trim().Trim('"').Trim();
            meta[key] = val;
        }

        return (meta, body);
    }

    /// <summary>
    /// Filters prompts based on the search query.
    /// </summary>
    [RelayCommand]
    private void FilterPrompts()
    {
        FilteredPrompts.Clear();

        if (string.IsNullOrWhiteSpace(SearchQuery))
        {
            foreach (var prompt in Prompts)
            {
                FilteredPrompts.Add(prompt);
            }
            return;
        }

        var query = SearchQuery.ToLowerInvariant();
        var matches = Prompts.Where(p =>
            p.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            p.Tags.Any(t => t.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
            p.Content.Contains(query, StringComparison.OrdinalIgnoreCase));

        foreach (var match in matches)
        {
            FilteredPrompts.Add(match);
        }
    }

    /// <summary>
    /// Selects a prompt, copies it to the clipboard (with simple variable substitution), and requests the selector to close.
    /// </summary>
    [RelayCommand]
    private async Task SelectPrompt(Prompt? prompt)
    {
        if (prompt == null)
        {
            return;
        }

        SelectedPrompt = prompt;

        try
        {
            // Try to read current clipboard (best-effort)
            string clipboardText = string.Empty;
            try
            {
                clipboardText = await ClipboardService.GetTextAsync() ?? string.Empty;
            }
            catch
            {
                // ignore failures reading clipboard
            }

            // Prepare final content with simple substitutions
            var final = prompt.Content ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(clipboardText))
            {
                // Common placeholder names used in sample prompts
                if (final.Contains("{{error}}")) final = final.Replace("{{error}}", clipboardText);
                if (final.Contains("{{code}}")) final = final.Replace("{{code}}", clipboardText);
                if (final.Contains("{{clipboard}}")) final = final.Replace("{{clipboard}}", clipboardText);
            }

            // Copy to clipboard (user-facing action)
            await ClipboardService.SetTextAsync(final);

            StatusMessage = "Prompt copied to clipboard";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to copy prompt: {ex.Message}";
        }

        // Request the view to close and provide the selected prompt
        RequestClose?.Invoke(prompt);
    }

    /// <summary>
    /// Command to request editing of a prompt.
    /// </summary>
    [RelayCommand]
    private void EditPrompt(Prompt? prompt)
    {
        if (prompt == null) return;
        RequestEdit?.Invoke(prompt);
    }

    /// <summary>
    /// Saves edited prompt content back to file and commits via IGitService (best-effort).
    /// </summary>
    public async Task SaveEditedPromptAsync(Prompt prompt, string newContent)
    {
        if (prompt == null || string.IsNullOrWhiteSpace(prompt.FilePath)) return;

        try
        {
            // Read existing frontmatter if present
            var original = File.ReadAllText(prompt.FilePath, Encoding.UTF8);
            var (meta, _) = ParseFrontMatter(original);

            var sb = new StringBuilder();
            sb.AppendLine("---");
            if (meta.TryGetValue("title", out var t)) sb.AppendLine($"title: {t}");
            if (meta.TryGetValue("tags", out var tags)) sb.AppendLine($"tags: {tags}");
            if (meta.TryGetValue("apps", out var apps)) sb.AppendLine($"apps: {apps}");
            sb.AppendLine("---");
            sb.AppendLine(newContent);

            File.WriteAllText(prompt.FilePath, sb.ToString(), Encoding.UTF8);

            // Update in-memory prompt
            var idx = Prompts.IndexOf(prompt);
            if (idx >= 0)
            {
                var updated = prompt with { Content = newContent, ModifiedAt = DateTime.UtcNow };
                Prompts[idx] = updated;

                // Refresh filtered list
                var fidx = FilteredPrompts.IndexOf(prompt);
                if (fidx >= 0) FilteredPrompts[fidx] = updated;
            }

            StatusMessage = "Saved prompt";

            // Try to commit via git service (best-effort)
            try
            {
                await _gitService.CommitAsync(Path.GetDirectoryName(prompt.FilePath) ?? string.Empty, $"Update prompt: {prompt.Title}");
                await _gitService.PushAsync(Path.GetDirectoryName(prompt.FilePath) ?? string.Empty, "Update prompt");
            }
            catch
            {
                // ignore git failures for now
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to save prompt: {ex.Message}";
        }
    }

    /// <summary>
    /// Cancels the selection and requests the selector to close.
    /// </summary>
    [RelayCommand]
    private void Cancel()
    {
        SelectedPrompt = null;
        RequestClose?.Invoke(null);
    }

    partial void OnSearchQueryChanged(string value)
    {
        FilterPrompts();
    }

    partial void OnSelectedPromptChanged(Prompt? value)
    {
        HasSelection = value != null;
    }

    /// <summary>
    /// Loads sample prompts for testing the UI.
    /// Remove this once real prompt loading is implemented.
    /// </summary>
    private void LoadSamplePrompts()
    {
        var samplePrompts = new[]
        {
            new Prompt
            {
                Id = "test-1",
                Title = "Debug Python Error",
                Content = "I'm getting this error:\n{{error}}\n\nIn this code:\n{{code}}\n\nHelp me debug it.",
                Tags = new[] { "python", "debug", "error" },
                Applications = new[] { "vscode", "pycharm" },
                FilePath = Path.Combine(_promptsFolder, "debug-python.md")
            },
            new Prompt
            {
                Id = "test-2",
                Title = "Write Unit Tests",
                Content = "Create comprehensive unit tests for:\n{{code}}\n\nInclude edge cases and error handling.",
                Tags = new[] { "testing", "unit-test", "qa" },
                Applications = new[] { "vscode" },
                FilePath = Path.Combine(_promptsFolder, "unit-tests.md")
            },
            new Prompt
            {
                Id = "test-3",
                Title = "Code Review",
                Content = "Review this code for:\n1. Performance issues\n2. Security vulnerabilities\n3. Best practices\n\n{{code}}",
                Tags = new[] { "review", "security", "performance" },
                Applications = new[] { "github", "gitlab" },
                FilePath = Path.Combine(_promptsFolder, "code-review.md")
            },
            new Prompt
            {
                Id = "test-4",
                Title = "Explain Complex Code",
                Content = "Explain this code in simple terms:\n{{code}}\n\nInclude:\n- What it does\n- How it works\n- Why it's written this way",
                Tags = new[] { "explain", "documentation", "learning" },
                Applications = new[] { "vscode" },
                FilePath = Path.Combine(_promptsFolder, "explain-code.md")
            },
            new Prompt
            {
                Id = "test-5",
                Title = "Refactor for Clean Code",
                Content = "Refactor this code following SOLID principles:\n{{code}}\n\nFocus on:\n- Single Responsibility\n- DRY principle\n- Clear naming",
                Tags = new[] { "refactor", "clean-code", "solid" },
                Applications = new[] { "vscode", "rider" },
                FilePath = Path.Combine(_promptsFolder, "refactor.md")
            }
        };

        foreach (var prompt in samplePrompts)
        {
            Prompts.Add(prompt);
            FilteredPrompts.Add(prompt);
        }

        StatusMessage = $"Loaded {Prompts.Count} test prompts";
    }
}
