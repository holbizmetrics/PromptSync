# PromptSync - C# Production Rewrite

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com)
[![Avalonia](https://img.shields.io/badge/Avalonia-11.0-8B5CF6)](https://avaloniaui.net)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![Tests](https://img.shields.io/badge/tests-passing-brightgreen.svg)](tests/)

**Git-native AI prompt management with intelligent DNA Lab features.**

> ?? **Status**: Foundation complete, core implementation in progress  
> ?? **Target**: Production-ready Q1 2025  
> ?? **Python Prototype**: [View original](https://github.com/holbizmetrics/PromptSync/tree/main) (archive branch)

---

## ?? Why C# Rewrite?

The Python prototype validated the concept. Now we're building for production:

| Metric | Python Prototype | C# Production |
|--------|-----------------|---------------|
| **Hotkey Response** | 50-200ms | <5ms target |
| **Distribution Size** | 100+ MB | 15-20 MB single .exe |
| **Startup Time** | 2-3 seconds | <500ms |
| **Memory Usage** | 80-150 MB | 30-50 MB |
| **Cross-Platform** | Limited | Windows, macOS, Linux |
| **Enterprise Ready** | ? | ? |

---

## ? What is PromptSync?

**The problem:** AI prompts are scattered across chat histories, notes apps, and memories. Finding the right prompt wastes time.

**The solution:** PromptSync stores prompts in Git (version control you already know) with intelligent features that make them better automatically.

### Key Features

?? **Instant Access**: Press `Ctrl+Shift+P` anywhere ? AI suggests YOUR prompts ? One click ? Done (3 seconds total)

?? **DNA Lab Intelligence** (6 Features):
- **Reverse Engineering**: Extract prompts from images/text
- **Auto-Iteration**: Self-improving prompts (3-5 refinement cycles)
- **Security Scanning**: Detect vulnerabilities before execution
- **Quality Scoring**: 5-dimension evaluation (Clarity, Specificity, Structure, Context, Examples)
- **Encryption**: Secure sensitive prompts
- **Web Harvesting**: Capture prompts while browsing

?? **Composable Workflows**: Chain prompts into automated pipelines  
?? **Context-Aware**: Suggests relevant prompts automatically  
?? **Git-Native**: Version control, branching, collaboration

---

## ?? Quick Start

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download) or later
- [Git](https://git-scm.com/downloads)
- GitHub account with [personal access token](https://github.com/settings/tokens)

### Installation

```bash
# Clone the repository
git clone https://github.com/holbizmetrics/PromptSync.git
cd PromptSync

# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test

# Run the desktop app
dotnet run --project src/PromptSync.Desktop
```

**See [GETTING_STARTED.md](docs/GETTING_STARTED.md) for detailed setup instructions.**

---

## ?? Project Structure

```
PromptSync/
??? src/
?   ??? PromptSync.Core/              # ? Core business logic
?   ?   ??? DNA/                      # ? Intelligence features
?   ?   ??? Services/                 # ? External integrations
?   ?   ??? Models/                   # ? Domain models
?   ?   ??? Exceptions/               # ? Custom exceptions
?   ?
?   ??? PromptSync.Desktop/           # ? Avalonia cross-platform UI
?   ?   ??? ViewModels/               # ? MVVM ViewModels
?   ?   ??? Views/                    # ? XAML views
?   ?
?   ??? PromptSync.CLI/               # ? Command-line interface
?   ??? PromptSync.HotkeyAgent/       # ? Platform-specific hotkey
?   ??? PromptSync.Tests/             # ? Unit & integration tests
?
??? docs/
?   ??? README.md                     # ? Project overview
?   ??? ARCHITECTURE.md               # ? System design deep-dive
?   ??? GETTING_STARTED.md            # ? Developer setup guide
?
??? PromptSync.sln                    # ? Visual Studio solution
??? global.json                       # ? .NET SDK version lock
```

? = Implemented | ?? = In Progress | ?? = Planned

---

## ??? Architecture Highlights

### Clean Architecture

- **SOLID Principles**: Every class has a single responsibility
- **Dependency Injection**: Loose coupling, high testability
- **Interface-Based Design**: Swap implementations easily
- **MVVM Pattern**: Separation of UI and business logic

### Technology Stack

| Component | Technology | Why? |
|-----------|-----------|------|
| UI Framework | Avalonia 11+ | Cross-platform XAML, native performance |
| MVVM | CommunityToolkit.Mvvm | Modern, attribute-based, less boilerplate |
| Git Operations | LibGit2Sharp | Native Git, no external dependencies |
| GitHub API | Octokit | Official GitHub SDK |
| AI Service | Extensible (Claude, GPT) | Strategy pattern for provider swap |
| Testing | xUnit + Moq + FluentAssertions | Industry standard .NET testing |

**See [ARCHITECTURE.md](docs/ARCHITECTURE.md) for detailed design documentation.**

---

## ?? DNA Lab Example

### Quality Scoring

```csharp
using PromptSync.Core.DNA;
using PromptSync.Core.Models;

// Inject the scorer
var scorer = serviceProvider.GetRequiredService<IQualityScorer>();

// Score a prompt
var prompt = new Prompt
{
    Id = "test",
    Title = "Debug Python Error",
    Content = "Help me debug this error: {{error}}"
};

var result = await scorer.ScoreAsync(prompt);

Console.WriteLine($"Overall: {result.QualityScore:F1}/10");
Console.WriteLine($"Clarity: {result.QualityDetails.Clarity:F1}/10");
Console.WriteLine($"Specificity: {result.QualityDetails.Specificity:F1}/10");

// Output:
// Overall: 6.8/10
// Clarity: 7.5/10
// Specificity: 6.0/10
```

**All DNA features follow this pattern:**
1. Interface-based design (`IQualityScorer`, `ISecurityScanner`, etc.)
2. Async/await for I/O operations
3. `DnaResult` return type for consistency
4. Comprehensive error handling
5. Logging for debugging

---

## ?? Testing

We enforce high code quality through comprehensive testing:

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true

# Run specific category
dotnet test --filter "Category=Unit"
```

### Test Coverage Goals

- **Core Library**: ?80% coverage (ENFORCED)
- **DNA Lab Features**: ?90% coverage (critical logic)
- **ViewModels**: Command and property testing
- **Integration**: Git, GitHub, AI service calls

### Example Test

```csharp
[Fact]
public async Task ScoreContentAsync_WithValidContent_ReturnsSuccessResult()
{
    // Arrange
    var mockAI = new Mock<IAIService>();
    mockAI.Setup(x => x.SendStructuredPromptAsync<QualityBreakdown>(...))
          .ReturnsAsync(new QualityBreakdown { Clarity = 7.5, ... });
    var scorer = new QualityScorer(mockAI.Object, Mock.Of<ILogger>());

    // Act
    var result = await scorer.ScoreContentAsync("Create a report");

    // Assert
    result.Success.Should().BeTrue();
    result.QualityScore.Should().BeGreaterThan(0);
}
```

---

## ?? Code Quality Standards

We follow industry best practices:

? **OOP Principles**: Encapsulation, inheritance, polymorphism  
? **SOLID**: Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion  
? **Clean Code**: Descriptive names, small methods, clear intent  
? **DRY**: Don't Repeat Yourself - extract shared logic  
? **Error Handling**: Try-catch at boundaries, meaningful exceptions  
? **Documentation**: XML comments on all public APIs  
? **Async/Await**: All I/O operations are async

### Enforced by Tools

- **StyleCop.Analyzers**: Code style consistency
- **.NET Analyzers**: Best practices and security
- **xUnit**: Minimum 80% test coverage
- **Code Reviews**: Manual inspection before merge

---

## ??? Development Roadmap

### ? Phase 1: Foundation (Complete)

- [x] Solution structure with 5 projects
- [x] Core models: `Prompt`, `DnaResult`, `WorkflowChain`
- [x] DNA Lab interfaces (6 features)
- [x] `QualityScorer` implementation + tests
- [x] Avalonia Desktop app foundation
- [x] MVVM ViewModels with CommunityToolkit
- [x] `PromptSelectorWindow` UI design
- [x] CLI and HotkeyAgent scaffolding
- [x] Comprehensive documentation

### ?? Phase 2: Core Implementation (In Progress - Weeks 1-4)

- [ ] Git service (LibGit2Sharp wrapper)
- [ ] GitHub service (Octokit wrapper)
- [ ] AI service (Claude API integration)
- [ ] Security Scanner implementation
- [ ] Iterator (auto-improvement) implementation
- [ ] Reverse Engineer implementation
- [ ] Encryptor implementation
- [ ] Harvester implementation
- [ ] Prompt file parser (Markdown + YAML frontmatter)

### ?? Phase 3: Desktop UI (Weeks 5-6)

- [ ] Prompt selector full implementation
- [ ] Search and filtering
- [ ] Clipboard integration
- [ ] Settings window
- [ ] Tray icon service
- [ ] Hotkey coordination (IPC setup)

### ?? Phase 4: Hotkey Agent (Week 7)

- [ ] Windows: Win32 P/Invoke (RegisterHotKey)
- [ ] macOS: CGEventTap
- [ ] Linux: X11/Wayland
- [ ] IPC client (HTTP POST)
- [ ] Security: Token-based auth

### ?? Phase 5: Polish & Release (Week 8+)

- [ ] End-to-end integration testing
- [ ] Performance optimization (<5ms response)
- [ ] Single-file executable packaging
- [ ] Cross-platform installers
- [ ] CI/CD with GitHub Actions
- [ ] Documentation polish
- [ ] Beta release

---

## ?? Contributing

We welcome contributions! Here's how to get started:

1. **Read the docs**:
   - [GETTING_STARTED.md](docs/GETTING_STARTED.md) - Setup your environment
   - [ARCHITECTURE.md](docs/ARCHITECTURE.md) - Understand the design
   - Code quality guidelines (this README)

2. **Pick a task**:
   - Check [Issues](https://github.com/holbizmetrics/PromptSync/issues)
   - Look for `good first issue` or `help wanted` labels
   - Or propose your own feature

3. **Follow the workflow**:
   ```bash
   git checkout -b feature/my-feature
   # Make changes
   dotnet build && dotnet test
   git commit -m "feat: Add my feature"
   git push origin feature/my-feature
   # Open Pull Request
   ```

4. **PR Checklist**:
   - [ ] All tests passing (`dotnet test`)
   - [ ] Code coverage ?80% for new code
   - [ ] XML documentation on public APIs
   - [ ] No StyleCop warnings
   - [ ] Error handling implemented
   - [ ] Manual testing completed

---

## ?? Documentation

| Document | Purpose |
|----------|---------|
| [README.md](README.md) | You are here! Project overview |
| [ARCHITECTURE.md](docs/ARCHITECTURE.md) | System design, patterns, principles |
| [GETTING_STARTED.md](docs/GETTING_STARTED.md) | Developer setup guide |
| [Python README](README_PYTHON.md) | Original prototype documentation |

---

## ?? License

MIT License - see [LICENSE](LICENSE) file for details.

**TL;DR:** Free to use, modify, distribute. Attribution appreciated.

---

## ?? Acknowledgments

- **Python Prototype**: Validated the concept, inspired the architecture
- **Avalonia UI**: Excellent cross-platform XAML framework
- **LibGit2Sharp**: Native Git operations in .NET
- **CommunityToolkit**: Modern MVVM helpers
- **.NET Foundation**: Amazing ecosystem

---

## ?? Community

- **Issues**: [Report bugs or request features](https://github.com/holbizmetrics/PromptSync/issues)
- **Discussions**: [Ask questions, share ideas](https://github.com/holbizmetrics/PromptSync/discussions)
- **Twitter**: [@promptsync](https://twitter.com/promptsync) (coming soon)

---

## ?? Project Status

**Current Focus**: Implementing core services (Git, GitHub, AI)

**Next Milestone**: Complete DNA Lab feature implementations

**Weekly Progress**: Updated in GitHub Discussions

---

**Built with ?? by developers tired of copy-pasting prompts.**

**[? Back to Top](#promptsync---c-production-rewrite)**
