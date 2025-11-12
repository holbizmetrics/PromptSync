# PromptSync - C# Rewrite

**Production-ready Git-based AI prompt management system with intelligent DNA Lab features.**

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![Avalonia](https://img.shields.io/badge/Avalonia-11.0-8B5CF6?logo=avalonia)
![License](https://img.shields.io/badge/License-MIT-green.svg)

---

## ?? Project Vision

PromptSync is being rebuilt from Python to C# for production use, targeting:

- **Sub-5ms hotkey response** (Python was 50-200ms)
- **Single .exe distribution** (vs 100+ MB Python bundle)
- **True cross-platform** desktop app (Windows, macOS, Linux)
- **Professional developer experience**
- **Enterprise-ready architecture**

---

## ??? Architecture

### Technology Stack

- **UI Framework**: Avalonia 11+ (cross-platform XAML)
- **MVVM Pattern**: CommunityToolkit.Mvvm (modern, attribute-based)
- **Dependency Injection**: Microsoft.Extensions.DependencyInjection
- **Git Operations**: LibGit2Sharp (native Git)
- **GitHub API**: Octokit
- **Testing**: xUnit + FluentAssertions + Moq

### Clean Architecture Principles

The codebase follows SOLID principles and clean code practices:

- **Single Responsibility**: Each class has one well-defined purpose
- **Separation of Concerns**: UI, business logic, and data access are separated
- **DRY (Don't Repeat Yourself)**: Shared logic extracted into reusable components
- **Dependency Inversion**: Interfaces used for all service dependencies
- **Testability**: All components designed for unit testing with mocking

---

## ?? Project Structure

```
PromptSync.sln
??? src/
?   ??? PromptSync.Core/              # Core business logic (platform-agnostic)
?   ?   ??? DNA/                      # DNA Lab intelligence features
?   ?   ?   ??? IReverseEngineer.cs   # Extract prompts from content
?   ?   ?   ??? IIterator.cs          # Auto-improve prompts
?   ?   ?   ??? ISecurityScanner.cs   # Scan for vulnerabilities
?   ?   ?   ??? IQualityScorer.cs     # Score prompt quality
?   ?   ?   ??? IEncryptor.cs         # Encrypt sensitive prompts
?   ?   ?   ??? IHarvester.cs         # Harvest from web
?   ?   ?   ??? QualityScorer.cs      # Example implementation ?
?   ?   ??? Services/                 # External service abstractions
?   ?   ?   ??? IAIService.cs         # AI provider (Claude, GPT)
?   ?   ?   ??? IGitService.cs        # Git operations
?   ?   ?   ??? IGitHubService.cs     # GitHub API
?   ?   ??? Models/                   # Domain models
?   ?   ?   ??? Prompt.cs             # Immutable prompt record
?   ?   ?   ??? DnaResult.cs          # DNA operation results
?   ?   ?   ??? WorkflowChain.cs      # Workflow composition
?   ?   ??? Exceptions/               # Custom exceptions
?   ?       ??? PromptSyncException.cs
?   ?       ??? GitSyncException.cs
?   ?       ??? AIServiceException.cs
?   ?
?   ??? PromptSync.Desktop/           # Avalonia cross-platform UI
?   ?   ??? ViewModels/               # MVVM ViewModels
?   ?   ?   ??? ViewModelBase.cs      # Base ViewModel
?   ?   ?   ??? PromptSelectorViewModel.cs ?
?   ?   ??? Views/                    # Avalonia XAML views
?   ?   ?   ??? PromptSelectorWindow.axaml ?
?   ?   ?   ??? PromptSelectorWindow.axaml.cs
?   ?   ??? Services/                 # Desktop-specific services
?   ?   ?   ??? IHotkeyService.cs
?   ?   ?   ??? ITrayIconService.cs
?   ?   ??? App.axaml                 # Application entry point ?
?   ?   ??? Program.cs                # Main method ?
?   ?
?   ??? PromptSync.HotkeyAgent/       # Platform-specific hotkey agent
?   ?   ??? Program.cs
?   ?   ??? Windows/                  # Win32 P/Invoke
?   ?   ??? MacOS/                    # CGEventTap
?   ?   ??? Linux/                    # X11/Wayland
?   ?
?   ??? PromptSync.CLI/               # Command-line interface
?       ??? Program.cs
?
??? tests/
    ??? PromptSync.Tests/             # xUnit test project
        ??? Core/
            ??? DNA/
                ??? QualityScorerTests.cs ?
```

---

## ?? Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- Git
- Visual Studio 2022 or JetBrains Rider (recommended)
- GitHub account with personal access token

### Building the Solution

```bash
# Clone the repository
git clone https://github.com/holbizmetrics/PromptSync.git
cd PromptSync

# Restore dependencies
dotnet restore

# Build all projects
dotnet build

# Run tests
dotnet test

# Run the desktop app
dotnet run --project src/PromptSync.Desktop
```

### Project Status

**Current Phase**: Foundation Setup ?

- [x] Solution structure created
- [x] Core library with models and interfaces
- [x] DNA Lab interfaces defined (6 features)
- [x] Quality Scorer implemented with tests
- [x] Avalonia Desktop project configured
- [x] MVVM ViewModels with CommunityToolkit
- [x] PromptSelectorWindow UI designed
- [ ] Service implementations (Git, GitHub, AI)
- [ ] Remaining DNA Lab implementations
- [ ] Hotkey agent for Windows
- [ ] End-to-end integration

---

## ?? DNA Lab Features

### 1. **Reverse Engineering** ??
Extract prompts from images, text, or web content.

```csharp
var result = await reverseEngineer.FromImageAsync("screenshot.png");
Console.WriteLine(result.Content); // Extracted prompt
```

### 2. **Automated Iteration** ??
Self-improving prompts through AI-powered refinement.

```csharp
var result = await iterator.IterateAsync(
    topic: "email writing",
    question: "Professional follow-up",
    maxIterations: 5,
    targetQuality: 8.5);
// Improved from 6.2/10 to 8.7/10
```

### 3. **Security Scanning** ???
Detect prompt injection, code execution, and PII exposure.

```csharp
var result = await scanner.ScanAsync(prompt);
if (result.RiskLevel == SecurityRiskLevel.High)
{
    Console.WriteLine($"Warning: {result.Vulnerabilities.Count} issues found");
}
```

### 4. **Quality Scoring** ??
5-dimension evaluation: Clarity, Specificity, Structure, Context, Examples.

```csharp
var result = await scorer.ScoreAsync(prompt);
Console.WriteLine($"Overall: {result.QualityScore:F1}/10");
Console.WriteLine($"Clarity: {result.QualityDetails.Clarity:F1}/10");
```

### 5. **Encryption** ??
Secure sensitive prompts with password encryption.

```csharp
var encrypted = await encryptor.EncryptAsync(prompt, "password");
var decrypted = await encryptor.DecryptAsync(encrypted, "password");
```

### 6. **Web Harvesting** ???
Capture prompts from web pages automatically.

```csharp
var result = await harvester.HarvestFromWebAsync("https://example.com");
// Auto-categorized prompt with confidence score
```

---

## ?? Testing

The project uses xUnit with FluentAssertions and Moq for comprehensive testing.

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Run specific test class
dotnet test --filter "FullyQualifiedName~QualityScorerTests"
```

### Test Coverage Goals

- **Core Library**: Minimum 80% coverage
- **DNA Lab Features**: 90%+ coverage (critical logic)
- **ViewModels**: Command and property change testing
- **Integration Tests**: Git, GitHub, AI service calls

---

## ?? Code Quality Standards

### Required Practices

? **OOP Principles**: Proper encapsulation, inheritance, polymorphism  
? **SOLID**: Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion  
? **Clean Code**: Descriptive names, small methods, clear intent  
? **Error Handling**: Try-catch blocks, meaningful exceptions, user-friendly messages  
? **Documentation**: XML comments on all public APIs  
? **Testing**: Unit tests for all business logic

### Enforced by Tools

- **StyleCop.Analyzers**: Code style consistency
- **.NET Analyzers**: Best practices and security
- **Code Reviews**: Manual inspection before merge

---

## ?? UI/UX Principles

- **Keyboard-First**: Arrow keys, Enter, Escape for navigation
- **Sub-3-Second Workflow**: Hotkey ? Search ? Select ? Done
- **High Contrast Support**: Accessibility built-in
- **Consistent Design**: Fluent Design System with Avalonia
- **Responsive**: Handle different screen sizes and DPI scaling

---

## ?? Contributing

We follow strict code quality guidelines. Before submitting a PR:

1. **Read**: `PROJECT_INSTRUCTIONS.md` and code quality docs
2. **Test**: All tests must pass (`dotnet test`)
3. **Format**: Follow StyleCop rules (enable analyzers)
4. **Document**: Add XML comments to public APIs
5. **Review**: Self-review your changes

### PR Checklist

- [ ] All tests passing
- [ ] Code coverage ? 80% for new code
- [ ] XML documentation on public members
- [ ] No StyleCop warnings
- [ ] Error handling implemented
- [ ] Logging added for debugging
- [ ] Manual testing completed

---

## ?? Documentation

- **README.md** (this file): Project overview
- **ARCHITECTURE.md**: Detailed system design (coming soon)
- **API.md**: Auto-generated from XML comments (coming soon)
- **CONTRIBUTING.md**: Contribution guidelines (coming soon)

---

## ??? Roadmap

### Phase 1: Foundation ? (Current)
- [x] Solution structure
- [x] Core models and interfaces
- [x] DNA Lab interfaces
- [x] Sample implementation (Quality Scorer)
- [x] Avalonia UI foundation
- [x] Test project setup

### Phase 2: Core Implementation (Weeks 1-4)
- [ ] Git service implementation (LibGit2Sharp)
- [ ] GitHub service (Octokit)
- [ ] AI service (Claude API)
- [ ] Complete DNA Lab implementations
- [ ] Prompt loading and parsing

### Phase 3: Desktop UI (Weeks 5-6)
- [ ] Prompt selector full implementation
- [ ] Settings window
- [ ] Tray icon service
- [ ] Hotkey coordination (IPC)

### Phase 4: Hotkey Agent (Week 7)
- [ ] Windows P/Invoke implementation
- [ ] macOS CGEventTap
- [ ] Linux X11/Wayland
- [ ] IPC security

### Phase 5: Polish & Release (Week 8+)
- [ ] End-to-end testing
- [ ] Performance optimization (<5ms response)
- [ ] Packaging (single-file exe)
- [ ] Documentation
- [ ] GitHub Actions CI/CD

---

## ?? License

MIT License - see [LICENSE](../LICENSE) file for details.

**Built with ?? by developers tired of copy-pasting prompts.**

---

## ?? Acknowledgments

- **Python Prototype**: Foundation for feature design
- **Avalonia UI**: Cross-platform XAML framework
- **LibGit2Sharp**: Native Git operations
- **CommunityToolkit.Mvvm**: Modern MVVM helpers
- **.NET Foundation**: Excellent ecosystem

---

**Questions? Open an issue or start a discussion on GitHub!**
