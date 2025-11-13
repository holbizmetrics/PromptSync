using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PromptSync.Core.Models;
using PromptSync.Core.Services;

namespace PromptSync.Desktop.ViewModels;

public partial class PromptEditorViewModel : ViewModelBase
{
    private readonly PromptSelectorViewModel _parent;
    private readonly IGitService _gitService;

    public event Action? RequestClose;

    [ObservableProperty]
    private string editedContent = string.Empty;

    private readonly Prompt _prompt;

    public PromptEditorViewModel(PromptSelectorViewModel parent, Prompt prompt, IGitService gitService)
    {
        _parent = parent;
        _prompt = prompt;
        _gitService = gitService;
        EditedContent = prompt.Content ?? string.Empty;
    }

    [RelayCommand]
    private async Task Save()
    {
        await _parent.SaveEditedPromptAsync(_prompt, EditedContent);
        RequestClose?.Invoke();
    }

    [RelayCommand]
    private void Cancel()
    {
        RequestClose?.Invoke();
    }
}
