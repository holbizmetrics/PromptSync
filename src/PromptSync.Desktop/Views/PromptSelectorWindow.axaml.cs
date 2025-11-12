using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using PromptSync.Desktop.ViewModels;

namespace PromptSync.Desktop.Views;

/// <summary>
/// Interaction logic for PromptSelectorWindow.
/// Minimal code-behind following MVVM pattern.
/// </summary>
public partial class PromptSelectorWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PromptSelectorWindow"/> class.
    /// </summary>
    public PromptSelectorWindow()
    {
        InitializeComponent();

        // Wire up RequestClose if DataContext is a PromptSelectorViewModel
        this.Opened += (_, _) =>
        {
            if (DataContext is PromptSelectorViewModel vm)
            {
                vm.RequestClose += OnRequestClose;
            }
        };

        this.Closed += (_, _) =>
        {
            if (DataContext is PromptSelectorViewModel vm)
            {
                vm.RequestClose -= OnRequestClose;
            }
        };
    }

    private async void OnRequestClose(Core.Models.Prompt? selected)
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            // Clipboard handling removed due to cross-platform API differences.
            // The ViewModel already exposes SelectedPrompt; higher-level code can handle copying.
            Close();
        });
    }
}
