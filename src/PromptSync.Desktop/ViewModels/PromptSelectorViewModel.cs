using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PromptSync.Core.Models;
using PromptSync.Core.Services;
using System.Collections.ObjectModel;
using System.Linq;

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

    /// <summary>
    /// Initializes a new instance of the <see cref="PromptSelectorViewModel"/> class.
    /// </summary>
    /// <param name="gitService">The Git service.</param>
    /// <param name="aiService">The AI service.</param>
    public PromptSelectorViewModel(IGitService gitService, IAIService aiService)
    {
        _gitService = gitService ?? throw new ArgumentNullException(nameof(gitService));
        _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));

        // Load sample test data for UI testing
        LoadSamplePrompts();
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
            // TODO: Implement prompt loading from Git repository
            await Task.Delay(500); // Placeholder

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
    /// Selects a prompt and closes the selector.
    /// </summary>
    [RelayCommand]
    private void SelectPrompt(Prompt? prompt)
    {
        if (prompt == null)
        {
            return;
        }

        SelectedPrompt = prompt;
        // TODO: Copy to clipboard and close window
    }

    /// <summary>
    /// Cancels the selection and closes the selector.
    /// </summary>
    [RelayCommand]
    private void Cancel()
    {
        SelectedPrompt = null;
        // TODO: Close window
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
                FilePath = "prompts/debug-python.md"
            },
            new Prompt
            {
                Id = "test-2",
                Title = "Write Unit Tests",
                Content = "Create comprehensive unit tests for:\n{{code}}\n\nInclude edge cases and error handling.",
                Tags = new[] { "testing", "unit-test", "qa" },
                Applications = new[] { "vscode" },
                FilePath = "prompts/unit-tests.md"
            },
            new Prompt
            {
                Id = "test-3",
                Title = "Code Review",
                Content = "Review this code for:\n1. Performance issues\n2. Security vulnerabilities\n3. Best practices\n\n{{code}}",
                Tags = new[] { "review", "security", "performance" },
                Applications = new[] { "github", "gitlab" },
                FilePath = "prompts/code-review.md"
            },
            new Prompt
            {
                Id = "test-4",
                Title = "Explain Complex Code",
                Content = "Explain this code in simple terms:\n{{code}}\n\nInclude:\n- What it does\n- How it works\n- Why it's written this way",
                Tags = new[] { "explain", "documentation", "learning" },
                Applications = new[] { "vscode" },
                FilePath = "prompts/explain-code.md"
            },
            new Prompt
            {
                Id = "test-5",
                Title = "Refactor for Clean Code",
                Content = "Refactor this code following SOLID principles:\n{{code}}\n\nFocus on:\n- Single Responsibility\n- DRY principle\n- Clear naming",
                Tags = new[] { "refactor", "clean-code", "solid" },
                Applications = new[] { "vscode", "rider" },
                FilePath = "prompts/refactor.md"
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
