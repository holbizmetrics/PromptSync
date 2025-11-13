using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using AvaloniaEdit;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Highlighting.Xshd;
using System;
using System.IO;
using System.Xml;
using PromptSync.Desktop.ViewModels;

namespace PromptSync.Desktop.Views;

/// <summary>
/// FULL-FEATURED rich text editor using AvaloniaEdit.
/// FIXED version that properly loads prompt content.
/// </summary>
public class RichPromptEditorWindow : Window
{
	private readonly TextEditor _editor;
	private readonly PromptEditorViewModel _viewModel;
	private readonly string _initialText;

	public RichPromptEditorWindow(PromptEditorViewModel viewModel)
	{
		_viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

		// Store the initial text immediately
		_initialText = _viewModel.EditedContent ?? string.Empty;

		Width = 900;
		Height = 700;
		Title = "Edit Prompt - PromptSync";
		WindowStartupLocation = WindowStartupLocation.CenterOwner;
		CanResize = true;

		DataContext = viewModel;

		// Subscribe to close request
		_viewModel.RequestClose += () => Dispatcher.UIThread.Post(() => Close());

		// Create the AvaloniaEdit TextEditor with ALL features
		_editor = new TextEditor
		{
			ShowLineNumbers = true,
			FontFamily = new FontFamily("Consolas, Courier New, monospace"),
			FontSize = 14,
			Padding = new Thickness(8),
			WordWrap = true,
			Background = Brushes.White,
			Foreground = Brushes.Black,
			HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
			VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto
		};

		// Set text IMMEDIATELY (don't wait for Opened)
		if (!string.IsNullOrEmpty(_initialText))
		{
			_editor.Text = _initialText;
			_editor.InvalidateVisual();
		}

		// Apply custom syntax highlighting for prompt templates
		ApplyPromptTemplateHighlighting();

		// Build the layout
		Content = BuildLayout();

		// ALSO set text when window opens (belt and suspenders approach)
		this.Opened += (s, e) =>
		{
			// Re-set text to be absolutely sure
			if (string.IsNullOrEmpty(_editor.Text) && !string.IsNullOrEmpty(_initialText))
			{
				_editor.Text = _initialText;
			}

			// Focus the editor and move cursor to end
			_editor.Focus();
			if (_editor.Document != null)
			{
				_editor.CaretOffset = _editor.Document.TextLength;
			}
		};

		// Sync text changes back to ViewModel
		_editor.TextChanged += (s, e) =>
		{
			if (_viewModel != null)
			{
				_viewModel.EditedContent = _editor.Text ?? string.Empty;
			}
		};
	}

	private Grid BuildLayout()
	{
		var grid = new Grid
		{
			RowDefinitions = new RowDefinitions("Auto,*,Auto"),
			Margin = new Thickness(16)
		};

		// Header
		var header = new StackPanel
		{
			Orientation = Orientation.Horizontal,
			Spacing = 8,
			Margin = new Thickness(0, 0, 0, 12)
		};

		header.Children.Add(new TextBlock
		{
			Text = "Edit Prompt",
			FontSize = 20,
			FontWeight = FontWeight.Bold,
			VerticalAlignment = VerticalAlignment.Center
		});

		header.Children.Add(new TextBlock
		{
			Text = "✨ Syntax highlighting enabled",
			FontSize = 11,
			Opacity = 0.6,
			VerticalAlignment = VerticalAlignment.Bottom,
			Margin = new Thickness(0, 0, 0, 2)
		});

		Grid.SetRow(header, 0);

		// Editor wrapped in a styled border
		var editorBorder = new Border
		{
			BorderThickness = new Thickness(1),
			BorderBrush = new SolidColorBrush(Color.Parse("#CCCCCC")),
			Background = Brushes.White,
			Child = _editor,
			Margin = new Thickness(0, 0, 0, 12),
			CornerRadius = new CornerRadius(4)
		};
		Grid.SetRow(editorBorder, 1);

		// Footer with buttons and info
		var footerGrid = new Grid
		{
			ColumnDefinitions = new ColumnDefinitions("*,Auto")
		};
		Grid.SetRow(footerGrid, 2);

		// Info text
		var infoText = new TextBlock
		{
			Text = "💡 Tip: {{variables}} are highlighted in blue",
			FontSize = 11,
			Opacity = 0.6,
			VerticalAlignment = VerticalAlignment.Center
		};
		Grid.SetColumn(infoText, 0);

		// Buttons
		var buttonStack = new StackPanel
		{
			Orientation = Orientation.Horizontal,
			HorizontalAlignment = HorizontalAlignment.Right,
			Spacing = 12
		};
		Grid.SetColumn(buttonStack, 1);

		var saveButton = new Button
		{
			Content = "💾 Save",
			Width = 100,
			Height = 36,
			FontSize = 14
		};
		saveButton.Click += async (s, e) => await _viewModel.SaveCommand.ExecuteAsync(null);

		var cancelButton = new Button
		{
			Content = "✖ Cancel",
			Width = 100,
			Height = 36,
			FontSize = 14
		};
		cancelButton.Click += (s, e) => _viewModel.CancelCommand.Execute(null);

		buttonStack.Children.Add(saveButton);
		buttonStack.Children.Add(cancelButton);

		footerGrid.Children.Add(infoText);
		footerGrid.Children.Add(buttonStack);

		// Add all to main grid
		grid.Children.Add(header);
		grid.Children.Add(editorBorder);
		grid.Children.Add(footerGrid);

		return grid;
	}

	private void ApplyPromptTemplateHighlighting()
	{
		// Custom XSHD (eXtensible Syntax Highlighting Definition) for prompt templates
		// This highlights:
		// - {{variables}} in blue bold
		// - # Markdown headings in green bold
		// - **bold text** in bold
		// - *italic text* in italic
		// - `inline code` in red monospace
		// - ```code blocks``` in red monospace

		var xshd = @"<?xml version=""1.0""?>
<SyntaxDefinition name=""PromptTemplate"" xmlns=""http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008"">
    
    <!-- Color definitions -->
    <Color name=""Variable"" foreground=""#0066CC"" fontWeight=""bold"" />
    <Color name=""Heading"" foreground=""#008000"" fontWeight=""bold"" />
    <Color name=""Bold"" fontWeight=""bold"" />
    <Color name=""Italic"" fontStyle=""italic"" />
    <Color name=""Code"" foreground=""#A31515"" fontFamily=""Consolas"" />
    <Color name=""Comment"" foreground=""#008000"" fontStyle=""italic"" opacity=""0.7"" />
    
    <RuleSet>
        <!-- Variables like {{variable}} or {{error}} -->
        <Rule color=""Variable"">
            \{\{[^}]+\}\}
        </Rule>
        
        <!-- Markdown headings: # Heading - FIXED -->
        <Rule color=""Heading"">
            ^\#{1,6}\s[^\r\n]+
        </Rule>
        
        <!-- Bold text: **bold** -->
        <Rule color=""Bold"">
            \*\*[^*]+\*\*
        </Rule>
        
        <!-- Italic text: *italic* - FIXED -->
        <Rule color=""Italic"">
            \*[^*\s][^*]*\*
        </Rule>
        
        <!-- Inline code: `code` -->
        <Rule color=""Code"">
            `[^`]+`
        </Rule>
        
        <!-- Code blocks: ```code``` - FIXED -->
        <Rule color=""Code"">
            ```[^`][^`]*```
        </Rule>
        
        <!-- Comments/Notes: // comment - FIXED -->
        <Rule color=""Comment"">
            //[^\r\n]*
        </Rule>
    </RuleSet>
</SyntaxDefinition>";

		try
		{
			using var reader = new XmlTextReader(new StringReader(xshd));
			var definition = HighlightingLoader.Load(reader, HighlightingManager.Instance);
			_editor.SyntaxHighlighting = definition;
		}
		catch (Exception ex)
		{
			// If syntax highlighting fails, log but continue (editor still works)
			System.Diagnostics.Debug.WriteLine($"Failed to load syntax highlighting: {ex.Message}");
			// Editor still works, just without pretty colors
		}
	}
}