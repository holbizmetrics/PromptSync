using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using PromptSync.Desktop.ViewModels;
using System.Windows.Input;

namespace PromptSync.Desktop.Views;

/// <summary>
/// Interaction logic for PromptSelectorWindow.
/// Minimal code-behind following MVVM pattern.
/// ✅ UPDATED: Now uses RichPromptEditorWindow with AvaloniaEdit
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
                vm.RequestEdit += OnRequestEdit;
            }
        };

        this.Closed += (_, _) =>
        {
            if (DataContext is PromptSelectorViewModel vm)
            {
                vm.RequestClose -= OnRequestClose;
                vm.RequestEdit -= OnRequestEdit;
            }
        };
    }

    /// <summary>
    /// Exposes the ViewModel's EditPromptCommand so XAML templates can bind directly to the window.
    /// </summary>
    public ICommand? EditPromptCommand => (DataContext as PromptSelectorViewModel)?.EditPromptCommand;

    private async void OnRequestClose(Core.Models.Prompt? selected)
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            Close();
        });
    }

    private void OnRequestEdit(Core.Models.Prompt prompt)
    {
        // ✅ UPDATED: Open the RICH editor window with AvaloniaEdit
        if (DataContext is PromptSelectorViewModel vm)
        {
            var editorVm = new PromptEditorViewModel(vm, prompt, vm.GitService);
            var editor = new RichPromptEditorWindow(editorVm);  // ← Using RichPromptEditorWindow now!
            editor.ShowDialog(this);
        }
    }
}
