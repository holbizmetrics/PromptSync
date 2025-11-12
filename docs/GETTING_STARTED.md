# PromptSync - Getting Started Guide

Welcome to PromptSync development! This guide will help you set up your environment and start contributing.

---

## Prerequisites

### Required Software

1. **.NET 8.0 SDK** or later
   - Download: https://dotnet.microsoft.com/download
   - Verify: `dotnet --version` (should show 8.0.x or higher)

2. **Git**
   - Download: https://git-scm.com/downloads
   - Verify: `git --version`

3. **IDE** (choose one):
   - **Visual Studio 2022** (recommended for Windows)
     - Community Edition is free
     - Workload: ".NET Desktop Development"
   - **JetBrains Rider** (cross-platform, excellent for .NET)
   - **Visual Studio Code** with C# extension

4. **GitHub Account**
   - Sign up at https://github.com
   - Generate a Personal Access Token:
     - Settings ? Developer settings ? Personal access tokens ? Tokens (classic)
     - Scopes: `repo` (Full control of private repositories)

---

## Step 1: Clone the Repository

```bash
git clone https://github.com/holbizmetrics/PromptSync.git
cd PromptSync
```

---

## Step 2: Restore Dependencies

```bash
dotnet restore
```

This downloads all NuGet packages defined in project files.

---

## Step 3: Build the Solution

```bash
dotnet build
```

**Expected output:**
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

If you see errors, check:
- .NET SDK version (`dotnet --version`)
- All project files have correct package references
- No syntax errors in code files

---

## Step 4: Run Tests

```bash
dotnet test
```

**Expected output:**
```
Starting test execution, please wait...
Total tests: X
     Passed: X
 Total time: X.XXs
```

All tests should pass. If not:
- Check test output for specific failures
- Ensure mock setups are correct
- Verify test data is valid

---

## Step 5: Run the Desktop App

```bash
dotnet run --project src/PromptSync.Desktop
```

**Expected behavior:**
- Avalonia window opens
- Prompt selector UI displays (empty initially)
- No crashes or exceptions

**Note:** Many features are not yet implemented. You'll see placeholder messages.

---

## Step 6: Explore the Codebase

### Key Directories

```
PromptSync/
??? src/
?   ??? PromptSync.Core/           # Start here! Business logic
?   ?   ??? DNA/                   # Intelligence features
?   ?   ??? Services/              # External integrations
?   ?   ??? Models/                # Domain models
?   ?   ??? Exceptions/            # Custom exceptions
?   ?
?   ??? PromptSync.Desktop/        # Avalonia UI
?   ?   ??? ViewModels/            # MVVM ViewModels
?   ?   ??? Views/                 # XAML views
?   ?
?   ??? PromptSync.CLI/            # Command-line interface
?   ??? PromptSync.HotkeyAgent/    # Platform-specific hotkey
?
??? tests/
?   ??? PromptSync.Tests/          # Unit & integration tests
?
??? docs/
    ??? README.md                   # Project overview
    ??? ARCHITECTURE.md             # System design
    ??? GETTING_STARTED.md          # This file!
```

### What's Implemented ?

- Solution structure with 5 projects
- Core models: `Prompt`, `DnaResult`, `WorkflowChain`
- DNA Lab interfaces: 6 features defined
- `QualityScorer` implementation with tests
- Avalonia Desktop app foundation
- MVVM ViewModels with CommunityToolkit
- `PromptSelectorWindow` UI design
- CLI and HotkeyAgent project scaffolding

### What's Next ??

- Service implementations (Git, GitHub, AI)
- Remaining DNA Lab features
- Prompt loading and parsing
- Hotkey IPC coordination
- End-to-end integration

---

## Step 7: Make Your First Change

Let's add a simple method to the `Prompt` model.

### 7.1 Open the File

```bash
# Open in your IDE or editor
code src/PromptSync.Core/Models/Prompt.cs
```

### 7.2 Add a Method

Add this method to the `Prompt` record:

```csharp
/// <summary>
/// Checks if the prompt has any tags.
/// </summary>
/// <returns>True if tags exist, false otherwise.</returns>
public bool HasTags() => Tags.Any();
```

### 7.3 Build and Test

```bash
dotnet build
# Should succeed with no errors
```

### 7.4 Write a Test

Create a test in `tests/PromptSync.Tests/Core/Models/PromptTests.cs`:

```csharp
using FluentAssertions;
using PromptSync.Core.Models;
using Xunit;

namespace PromptSync.Tests.Core.Models;

public class PromptTests
{
    [Fact]
    public void HasTags_WithNoTags_ReturnsFalse()
    {
        // Arrange
        var prompt = new Prompt
        {
            Id = "test",
            Title = "Test",
            Content = "Content",
            Tags = Array.Empty<string>()
        };

        // Act
        var result = prompt.HasTags();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void HasTags_WithTags_ReturnsTrue()
    {
        // Arrange
        var prompt = new Prompt
        {
            Id = "test",
            Title = "Test",
            Content = "Content",
            Tags = new[] { "coding", "debug" }
        };

        // Act
        var result = prompt.HasTags();

        // Assert
        result.Should().BeTrue();
    }
}
```

### 7.5 Run Tests Again

```bash
dotnet test
# All tests should pass, including your new ones!
```

---

## Step 8: Understand the MVVM Pattern

Let's examine the `PromptSelectorViewModel`:

```csharp
public partial class PromptSelectorViewModel : ViewModelBase
{
    // Observable property (auto-generates PropertyChanged events)
    [ObservableProperty]
    private string _searchQuery = string.Empty;

    // Command (auto-generates ICommand implementation)
    [RelayCommand]
    private async Task LoadPromptsAsync()
    {
        IsLoading = true;
        // Load prompts logic
        IsLoading = false;
    }
}
```

**XAML Binding:**

```xml
<TextBox Text="{Binding SearchQuery}" />
<Button Command="{Binding LoadPromptsCommand}" />
```

**How it works:**
1. `[ObservableProperty]` generates `SearchQuery` property with `INotifyPropertyChanged`
2. `[RelayCommand]` generates `LoadPromptsCommand` of type `IAsyncRelayCommand`
3. XAML binds to these automatically
4. No boilerplate code!

---

## Step 9: Debugging Tips

### In Visual Studio

1. Set breakpoint (F9) in code
2. Press F5 (Start Debugging)
3. App launches with debugger attached
4. Step through code (F10/F11)

### In VS Code

1. Install C# extension
2. Open Command Palette (Ctrl+Shift+P)
3. "C#: Generate Assets for Build and Debug"
4. Press F5 to debug

### Common Issues

**"The type or namespace name 'Avalonia' could not be found"**
- Run `dotnet restore`
- Rebuild solution

**"System.ArgumentNullException" in tests**
- Check mock setups
- Ensure all dependencies are injected

**Avalonia window doesn't render**
- Check XAML syntax
- Verify DataContext is set
- Look for binding errors in output window

---

## Step 10: Contribution Workflow

### 10.1 Create a Branch

```bash
git checkout -b feature/my-awesome-feature
```

### 10.2 Make Changes

- Follow SOLID principles
- Write tests for new code
- Add XML documentation
- Keep commits small and focused

### 10.3 Test Locally

```bash
dotnet build
dotnet test
# Ensure StyleCop warnings are addressed
```

### 10.4 Commit and Push

```bash
git add .
git commit -m "feat: Add awesome feature"
git push origin feature/my-awesome-feature
```

### 10.5 Open Pull Request

1. Go to GitHub repository
2. Click "Pull Requests" ? "New Pull Request"
3. Select your branch
4. Fill out PR template:
   - Description of changes
   - Testing done
   - Related issues
5. Submit for review

---

## Useful Commands

### Build

```bash
dotnet build                           # Build all projects
dotnet build -c Release                # Release build
dotnet build --no-restore              # Skip package restore
```

### Test

```bash
dotnet test                            # Run all tests
dotnet test --filter "Category=Unit"   # Run unit tests only
dotnet test --logger "console;verbosity=detailed"  # Verbose output
```

### Run

```bash
dotnet run --project src/PromptSync.Desktop  # Run desktop app
dotnet run --project src/PromptSync.CLI      # Run CLI
```

### Clean

```bash
dotnet clean                           # Remove build artifacts
rm -rf */bin */obj                     # Deep clean (PowerShell: Remove-Item)
```

### Publish

```bash
dotnet publish -c Release -r win-x64 --self-contained /p:PublishSingleFile=true
# Creates single-file .exe in bin/Release/net8.0/win-x64/publish/
```

---

## Learning Resources

### .NET & C#

- [Microsoft Learn - C#](https://learn.microsoft.com/en-us/dotnet/csharp/)
- [.NET API Browser](https://learn.microsoft.com/en-us/dotnet/api/)

### Avalonia UI

- [Avalonia Docs](https://docs.avaloniaui.net/)
- [Avalonia Samples](https://github.com/AvaloniaUI/Avalonia.Samples)

### MVVM

- [CommunityToolkit.Mvvm Docs](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/)
- [MVVM Pattern Overview](https://learn.microsoft.com/en-us/dotnet/architecture/maui/mvvm)

### Testing

- [xUnit Docs](https://xunit.net/)
- [FluentAssertions Docs](https://fluentassertions.com/)
- [Moq Quickstart](https://github.com/moq/moq4/wiki/Quickstart)

---

## Next Steps

Now that you're set up, consider tackling one of these tasks:

1. **Implement Git Service**
   - Use LibGit2Sharp
   - Implement `IGitService` interface
   - Write unit tests with mocked repository

2. **Complete Security Scanner**
   - Pattern-based vulnerability detection
   - Risk scoring algorithm
   - Integration with AI service

3. **Build Settings UI**
   - Create `SettingsWindow.axaml`
   - ViewModel for configuration
   - Save/load settings to JSON

4. **Add Prompt Parser**
   - Parse Markdown files
   - Extract frontmatter (YAML)
   - Validate prompt structure

5. **Implement Windows Hotkey Agent**
   - P/Invoke to RegisterHotKey
   - IPC client to desktop app
   - Error handling and logging

---

## Getting Help

- **Issues**: Open a GitHub issue for bugs or feature requests
- **Discussions**: Start a discussion for questions or ideas
- **Code Review**: Tag maintainers in your PR for review

---

**Happy coding! Welcome to the PromptSync team! ??**
