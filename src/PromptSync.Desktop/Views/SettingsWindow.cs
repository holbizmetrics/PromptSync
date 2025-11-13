using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using System;
using System.Diagnostics;

namespace PromptSync.Desktop.Views;

/// <summary>
/// Simple settings window for PromptSync.
/// Shows app info and basic configuration options.
/// </summary>
public class SettingsWindow : Window
{
	private TextBox? _externalEditorPathBox;

	public SettingsWindow()
	{
		Width = 600;
		Height = 400;
		Title = "PromptSync Settings";
		WindowStartupLocation = WindowStartupLocation.CenterOwner;
		CanResize = false;

		Content = BuildLayout();
	}

	private Grid BuildLayout()
	{
		var grid = new Grid
		{
			RowDefinitions = new RowDefinitions("Auto,*,Auto"),
			Margin = new Thickness(20)
		};

		// Header
		var header = new TextBlock
		{
			Text = "Settings",
			FontSize = 24,
			FontWeight = Avalonia.Media.FontWeight.Bold,
			Margin = new Thickness(0, 0, 0, 20)
		};
		Grid.SetRow(header, 0);

		// Content area
		var contentPanel = new StackPanel
		{
			Spacing = 20
		};
		Grid.SetRow(contentPanel, 1);

		// App Info Section
		var appInfoSection = CreateAppInfoSection();
		contentPanel.Children.Add(appInfoSection);

		// External Editor Section
		var editorSection = CreateExternalEditorSection();
		contentPanel.Children.Add(editorSection);

		// Footer with buttons
		var footer = new StackPanel
		{
			Orientation = Orientation.Horizontal,
			HorizontalAlignment = HorizontalAlignment.Right,
			Spacing = 12
		};
		Grid.SetRow(footer, 2);

		var saveButton = new Button
		{
			Content = "Save",
			Width = 100,
			Height = 36
		};
		saveButton.Click += (s, e) => SaveAndClose();

		var closeButton = new Button
		{
			Content = "Close",
			Width = 100,
			Height = 36
		};
		closeButton.Click += (s, e) => Close();

		footer.Children.Add(saveButton);
		footer.Children.Add(closeButton);

		grid.Children.Add(header);
		grid.Children.Add(contentPanel);
		grid.Children.Add(footer);

		return grid;
	}

	private Border CreateAppInfoSection()
	{
		var border = new Border
		{
			BorderThickness = new Thickness(1),
			Padding = new Thickness(16),
			Margin = new Thickness(0, 0, 0, 12)
		};

		var stack = new StackPanel { Spacing = 8 };

		stack.Children.Add(new TextBlock
		{
			Text = "PromptSync",
			FontSize = 18,
			FontWeight = Avalonia.Media.FontWeight.SemiBold
		});

		stack.Children.Add(new TextBlock
		{
			Text = "Version: 0.1.0-alpha",
			FontSize = 14,
			Opacity = 0.7
		});

		stack.Children.Add(new TextBlock
		{
			Text = "AI-powered prompt management with Git sync",
			FontSize = 12,
			Opacity = 0.6,
			TextWrapping = Avalonia.Media.TextWrapping.Wrap
		});

		border.Child = stack;
		return border;
	}

	private Border CreateExternalEditorSection()
	{
		var border = new Border
		{
			BorderThickness = new Thickness(1),
			Padding = new Thickness(16)
		};

		var stack = new StackPanel { Spacing = 12 };

		stack.Children.Add(new TextBlock
		{
			Text = "External Editor",
			FontSize = 16,
			FontWeight = Avalonia.Media.FontWeight.SemiBold
		});

		stack.Children.Add(new TextBlock
		{
			Text = "Path to your preferred editor (e.g., code, notepad++)",
			FontSize = 12,
			Opacity = 0.7
		});

		var pathGrid = new Grid
		{
			ColumnDefinitions = new ColumnDefinitions("*,Auto")
		};

		_externalEditorPathBox = new TextBox
		{
			Text = "code",
			Watermark = "e.g., code, notepad++, vim",
			FontFamily = "Consolas, Courier New"
		};
		Grid.SetColumn(_externalEditorPathBox, 0);

		var testButton = new Button
		{
			Content = "Test",
			Width = 80,
			Margin = new Thickness(8, 0, 0, 0)
		};
		testButton.Click += TestExternalEditor;
		Grid.SetColumn(testButton, 1);

		pathGrid.Children.Add(_externalEditorPathBox);
		pathGrid.Children.Add(testButton);

		stack.Children.Add(pathGrid);

		var helpText = new TextBlock
		{
			Text = "💡 Tip: Use 'code' for VS Code, 'notepad++' for Notepad++",
			FontSize = 11,
			Opacity = 0.6,
			Margin = new Thickness(0, 4, 0, 0)
		};
		stack.Children.Add(helpText);

		border.Child = stack;
		return border;
	}

	private async void TestExternalEditor(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
	{
		var editorPath = _externalEditorPathBox?.Text?.Trim();

		if (string.IsNullOrWhiteSpace(editorPath))
		{
			await ShowMessage("Please enter an editor command first.");
			return;
		}

		try
		{
			// Try to launch editor with --version or --help
			var psi = new ProcessStartInfo
			{
				FileName = editorPath,
				Arguments = "--version",
				UseShellExecute = false,
				CreateNoWindow = true
			};

			using var process = Process.Start(psi);
			if (process != null)
			{
				await ShowMessage($"✅ Found '{editorPath}'!\n\nIt should work for opening prompts.");
			}
		}
		catch (Exception ex)
		{
			await ShowMessage($"❌ Could not find '{editorPath}'.\n\nError: {ex.Message}\n\nMake sure it's in your PATH or use the full path.");
		}
	}

	private async System.Threading.Tasks.Task ShowMessage(string message)
	{
		var dialog = new Window
		{
			Title = "Editor Test",
			Width = 400,
			Height = 200,
			WindowStartupLocation = WindowStartupLocation.CenterOwner,
			CanResize = false
		};

		var stack = new StackPanel
		{
			Margin = new Thickness(20),
			Spacing = 16,
			VerticalAlignment = VerticalAlignment.Center
		};

		stack.Children.Add(new TextBlock
		{
			Text = message,
			TextWrapping = Avalonia.Media.TextWrapping.Wrap,
			TextAlignment = Avalonia.Media.TextAlignment.Center
		});

		var okButton = new Button
		{
			Content = "OK",
			Width = 100,
			HorizontalAlignment = HorizontalAlignment.Center
		};
		okButton.Click += (s, e) => dialog.Close();

		stack.Children.Add(okButton);
		dialog.Content = stack;

		await dialog.ShowDialog(this);
	}

	private void SaveAndClose()
	{
		// TODO: Actually save settings to a config file
		// For now, just close
		Close();
	}
}