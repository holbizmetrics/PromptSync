using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using PromptSync.Desktop.ViewModels;

namespace PromptSync.Desktop.Views;

/// <summary>
/// Lightweight code-only editor window used when editing prompts.
/// Created to avoid XAML generation issues in the build pipeline.
/// </summary>
public class PromptEditorWindow : Window
{
    public PromptEditorWindow(PromptEditorViewModel vm)
    {
        if (vm == null) throw new ArgumentNullException(nameof(vm));

        Width = 800;
        Height = 600;
        Title = "Edit Prompt";
        WindowStartupLocation = WindowStartupLocation.CenterOwner;

        DataContext = vm;
        vm.RequestClose += () => Dispatcher.UIThread.Post(() => Close());

        var grid = new Grid
        {
            RowDefinitions = new RowDefinitions("Auto,*,Auto"),
            Margin = new Thickness(12)
        };

        var header = new TextBlock
        {
            Text = "Edit Prompt",
            FontSize = 18,
            FontWeight = FontWeight.Bold,
            Margin = new Thickness(0,0,0,8)
        };
        Grid.SetRow(header, 0);

        var editor = new TextBox
        {
            AcceptsReturn = true,
            TextWrapping = TextWrapping.Wrap,
            FontFamily = new FontFamily("Consolas, 'Courier New'"),
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch
        };
        editor.Bind(TextBox.TextProperty, new Binding("EditedContent", BindingMode.TwoWay));
        Grid.SetRow(editor, 1);

        var buttons = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Spacing = 8,
            Margin = new Thickness(0,8,0,0)
        };
        Grid.SetRow(buttons, 2);

        var saveBtn = new Button { Content = "Save", Width = 100 };
        saveBtn.Bind(Button.CommandProperty, new Binding("SaveCommand"));

        var cancelBtn = new Button { Content = "Cancel", Width = 100 };
        cancelBtn.Bind(Button.CommandProperty, new Binding("CancelCommand"));

        buttons.Children.Add(saveBtn);
        buttons.Children.Add(cancelBtn);

        grid.Children.Add(header);
        grid.Children.Add(editor);
        grid.Children.Add(buttons);

        Content = grid;
    }
}
