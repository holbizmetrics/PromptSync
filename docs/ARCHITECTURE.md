# PromptSync C# Architecture

## Overview

PromptSync is architected as a **layered, cross-platform desktop application** following Clean Architecture and SOLID principles. The system is designed for **high performance** (<5ms hotkey response), **testability** (80%+ coverage), and **maintainability** (clear separation of concerns).

---

## System Architecture

```
???????????????????????????????????????????????????????????????
?                    User Interface Layer                      ?
?  ????????????????????????????????????????????????????????   ?
?  ?  Avalonia Desktop App (Cross-Platform XAML)          ?   ?
?  ?  - PromptSelectorWindow (MVVM)                       ?   ?
?  ?  - SettingsWindow                                    ?   ?
?  ?  - TrayIcon Integration                              ?   ?
?  ????????????????????????????????????????????????????????   ?
???????????????????????????????????????????????????????????????
                             ?
                             ?
???????????????????????????????????????????????????????????????
?                  Presentation Layer (MVVM)                   ?
?  ????????????????????????????????????????????????????????   ?
?  ?  ViewModels (CommunityToolkit.Mvvm)                  ?   ?
?  ?  - PromptSelectorViewModel                           ?   ?
?  ?  - SettingsViewModel                                 ?   ?
?  ?  - Commands & Observable Properties                  ?   ?
?  ????????????????????????????????????????????????????????   ?
???????????????????????????????????????????????????????????????
                             ?
                             ?
???????????????????????????????????????????????????????????????
?                    Business Logic Layer                      ?
?  ????????????????????????????????????????????????????????   ?
?  ?  PromptSync.Core (Platform-Agnostic)                 ?   ?
?  ?                                                       ?   ?
?  ?  DNA Lab:                                            ?   ?
?  ?  - IReverseEngineer / ReverseEngineer               ?   ?
?  ?  - IIterator / Iterator                             ?   ?
?  ?  - ISecurityScanner / SecurityScanner               ?   ?
?  ?  - IQualityScorer / QualityScorer ?                ?   ?
?  ?  - IEncryptor / Encryptor                           ?   ?
?  ?  - IHarvester / Harvester                           ?   ?
?  ?                                                       ?   ?
?  ?  Services:                                           ?   ?
?  ?  - IAIService (Claude, GPT abstraction)             ?   ?
?  ?  - IGitService (LibGit2Sharp wrapper)               ?   ?
?  ?  - IGitHubService (Octokit wrapper)                 ?   ?
?  ?                                                       ?   ?
?  ?  Models: Prompt, DnaResult, WorkflowChain           ?   ?
?  ????????????????????????????????????????????????????????   ?
???????????????????????????????????????????????????????????????
                             ?
                             ?
???????????????????????????????????????????????????????????????
?                Infrastructure / External Systems              ?
?  ?????????????????????????????????????????????????????????  ?
?  ?   Git Repo  ?  GitHub API ?   AI Service ?  Filesystem?  ?
?  ? (LibGit2)   ?  (Octokit)  ?   (Claude)   ?    I/O     ?  ?
?  ?????????????????????????????????????????????????????????  ?
???????????????????????????????????????????????????????????????

???????????????????????????????????????????????????????????????
?                    Platform-Specific Agent                    ?
?  ????????????????????????????????????????????????????????   ?
?  ?  PromptSync.HotkeyAgent (Separate Process)           ?   ?
?  ?  - Windows: Win32 RegisterHotKey (P/Invoke)          ?   ?
?  ?  - macOS: CGEventTap                                 ?   ?
?  ?  - Linux: X11/Wayland                                ?   ?
?  ?                                                       ?   ?
?  ?  IPC: HTTP POST ? http://127.0.0.1:PORT/activate    ?   ?
?  ????????????????????????????????????????????????????????   ?
???????????????????????????????????????????????????????????????
```

---

## Design Patterns

### 1. **MVVM (Model-View-ViewModel)**

**Why?** Separates UI from business logic, enables testability.

```csharp
// View (XAML)
<Window DataContext="{Binding PromptSelectorViewModel}">
  <TextBox Text="{Binding SearchQuery}" />
  <Button Command="{Binding LoadPromptsCommand}" />
</Window>

// ViewModel
public partial class PromptSelectorViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _searchQuery = string.Empty;

    [RelayCommand]
    private async Task LoadPromptsAsync()
    {
        // Business logic here
    }
}
```

**Benefits:**
- UI can be redesigned without touching logic
- ViewModels testable without UI framework
- Data binding eliminates boilerplate

### 2. **Dependency Injection**

**Why?** Loose coupling, testability, lifecycle management.

```csharp
// Registration (App.axaml.cs)
services.AddSingleton<IGitService, GitService>();
services.AddTransient<PromptSelectorViewModel>();

// Usage (ViewModel)
public PromptSelectorViewModel(IGitService gitService, IAIService aiService)
{
    _gitService = gitService;  // Injected automatically
}
```

**Benefits:**
- Easy to mock for unit tests
- Single source of truth for dependencies
- Supports singleton, transient, scoped lifetimes

### 3. **Repository Pattern** (for Git/GitHub)

**Why?** Abstracts data access, enables testing without real repositories.

```csharp
public interface IGitService
{
    Task PullAsync(string localPath, CancellationToken ct);
    Task PushAsync(string localPath, string message, CancellationToken ct);
}

// Mock in tests
var mockGit = new Mock<IGitService>();
mockGit.Setup(x => x.PullAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
       .ReturnsAsync(/* success */);
```

### 4. **Strategy Pattern** (for AI Services)

**Why?** Swap AI providers (Claude, GPT, Local models) without changing code.

```csharp
public interface IAIService
{
    Task<string> SendPromptAsync(string prompt, CancellationToken ct);
    string ProviderName { get; }
}

// Implementations
public class ClaudeService : IAIService { /* ... */ }
public class OpenAIService : IAIService { /* ... */ }
public class LocalModelService : IAIService { /* ... */ }

// Configure at runtime
services.AddSingleton<IAIService>(provider =>
    config.AIProvider == "Claude"
        ? new ClaudeService(config.ApiKey)
        : new OpenAIService(config.ApiKey));
```

---

## SOLID Principles in Action

### Single Responsibility Principle (SRP)

**Each class has ONE reason to change.**

? **Good:**
```csharp
public class GitService : IGitService
{
    // ONLY handles Git operations
    public Task PullAsync(string path, CancellationToken ct) { /* ... */ }
}

public class QualityScorer : IQualityScorer
{
    // ONLY scores prompt quality
    public Task<DnaResult> ScoreAsync(Prompt prompt, CancellationToken ct) { /* ... */ }
}
```

? **Bad:**
```csharp
public class PromptManager
{
    // Too many responsibilities: Git, scoring, AI, UI notifications
    public void SyncAndScoreAndNotify() { /* ... */ }
}
```

### Open/Closed Principle (OCP)

**Open for extension, closed for modification.**

```csharp
// Base abstraction
public interface IDnaFeature
{
    Task<DnaResult> ExecuteAsync(Prompt prompt, CancellationToken ct);
}

// New features added without modifying existing code
public class ReverseEngineer : IDnaFeature { /* ... */ }
public class SecurityScanner : IDnaFeature { /* ... */ }
public class CustomAnalyzer : IDnaFeature { /* ... */ }  // New feature!
```

### Liskov Substitution Principle (LSP)

**Subtypes must be substitutable for base types.**

```csharp
public abstract class ViewModelBase : ObservableObject { }

// All ViewModels can be used wherever ViewModelBase is expected
public class PromptSelectorViewModel : ViewModelBase { }
public class SettingsViewModel : ViewModelBase { }
```

### Interface Segregation Principle (ISP)

**Clients shouldn't depend on interfaces they don't use.**

? **Good:** Small, focused interfaces
```csharp
public interface IQualityScorer
{
    Task<DnaResult> ScoreAsync(Prompt prompt, CancellationToken ct);
    double GetQuickScore(string content);
}

public interface ISecurityScanner
{
    Task<DnaResult> ScanAsync(Prompt prompt, CancellationToken ct);
    Task<bool> IsSafeAsync(Prompt prompt, CancellationToken ct);
}
```

? **Bad:** Monolithic interface
```csharp
public interface IDnaLab
{
    // Too many unrelated methods
    Task<DnaResult> Score(Prompt p);
    Task<DnaResult> Scan(Prompt p);
    Task<DnaResult> Iterate(Prompt p);
    Task<DnaResult> ReverseEngineer(string image);
    // ...
}
```

### Dependency Inversion Principle (DIP)

**Depend on abstractions, not concretions.**

```csharp
// High-level module depends on abstraction
public class PromptSelectorViewModel
{
    private readonly IGitService _gitService;  // Abstraction, not GitService
    
    public PromptSelectorViewModel(IGitService gitService)
    {
        _gitService = gitService;
    }
}

// Low-level module implements abstraction
public class GitService : IGitService
{
    // LibGit2Sharp implementation
}
```

---

## Cross-Cutting Concerns

### 1. **Logging**

```csharp
public class QualityScorer : IQualityScorer
{
    private readonly ILogger<QualityScorer> _logger;

    public QualityScorer(ILogger<QualityScorer> logger)
    {
        _logger = logger;
    }

    public async Task<DnaResult> ScoreAsync(Prompt prompt, CancellationToken ct)
    {
        _logger.LogInformation("Scoring prompt: {Title}", prompt.Title);
        
        try
        {
            // ...
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to score prompt");
            throw;
        }
    }
}
```

### 2. **Error Handling**

**Strategy:**
- Custom exceptions with context
- Try-catch at service boundaries
- Fallback to safe defaults where possible
- User-friendly error messages

```csharp
try
{
    await _gitService.PullAsync(path, ct);
}
catch (GitSyncException ex)
{
    _logger.LogError(ex, "Git sync failed");
    StatusMessage = "Sync failed. Retrying in 30 seconds...";
    // Schedule retry
}
catch (Exception ex)
{
    _logger.LogError(ex, "Unexpected error");
    StatusMessage = "An unexpected error occurred.";
    // Don't retry, show error to user
}
```

### 3. **Async/Await Best Practices**

- All I/O operations are async
- CancellationToken passed through call stack
- ConfigureAwait(false) in library code
- Avoid async void (except event handlers)

```csharp
public async Task<DnaResult> ScoreContentAsync(
    string content,
    CancellationToken cancellationToken = default)
{
    // Check cancellation
    cancellationToken.ThrowIfCancellationRequested();

    // Use async I/O
    var result = await _aiService.SendPromptAsync(prompt, ct);

    return result;
}
```

---

## Performance Considerations

### 1. **Hotkey Response Time**

**Goal:** <5ms from hotkey press to UI display

**Strategy:**
- Separate hotkey agent process (no .NET startup delay)
- Pre-load prompts in memory (background sync)
- Lazy-load heavy resources (AI service connections)
- Use async I/O to avoid blocking UI thread

### 2. **Memory Management**

- Use `IReadOnlyList` and `IReadOnlyDictionary` to prevent mutations
- Dispose IDisposable resources (Git repos, HTTP clients)
- Avoid large object allocations in hot paths
- Profile with dotMemory to find leaks

### 3. **Caching**

```csharp
// Cache prompt metadata, not full content
private Dictionary<string, PromptMetadata> _promptCache = new();

// Invalidate on Git pull
public async Task PullAsync(string path, CancellationToken ct)
{
    await _git.PullAsync(path, ct);
    _promptCache.Clear();  // Force reload
}
```

---

## Security Architecture

### 1. **Hotkey Agent IPC**

- **Local-only**: Bind to `127.0.0.1`, reject external connections
- **Token-based auth**: Short-lived tokens, regenerated on app restart
- **TLS optional**: HTTPS for paranoid users, HTTP for simplicity

```csharp
// Desktop app starts HTTP server
app.StartIPCServer(token: GenerateSecureToken());

// Hotkey agent sends POST with token
POST http://127.0.0.1:39571/activate
Authorization: Bearer {token}
```

### 2. **Prompt Encryption**

- AES-256 encryption for sensitive prompts
- Password-based key derivation (PBKDF2)
- Encrypted prompts marked in frontmatter

```markdown
---
encrypted: true
---
U2FsdGVkX1+vupppZksvRf5pq5g5XjFRIipRkwB0K1Y=
```

### 3. **Security Scanning**

Detects:
- Prompt injection patterns
- Code execution risks (eval, exec, system calls)
- PII exposure (emails, phone numbers, SSNs)
- Unsafe file operations

---

## Testing Strategy

### 1. **Unit Tests** (Core library)

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
    var result = await scorer.ScoreContentAsync("content");

    // Assert
    result.Success.Should().BeTrue();
    result.QualityScore.Should().BeGreaterThan(0);
}
```

### 2. **Integration Tests** (External services)

```csharp
[Fact]
[Trait("Category", "Integration")]
public async Task GitService_PullAsync_WithRealRepo_Succeeds()
{
    // Uses real Git repository (test-only)
    var gitService = new GitService(logger);
    
    await gitService.CloneAsync(TestRepoUrl, tempPath, ct);
    await gitService.PullAsync(tempPath, ct);
    
    Directory.Exists(tempPath).Should().BeTrue();
}
```

### 3. **ViewModel Tests**

```csharp
[Fact]
public void SearchQuery_WhenChanged_FiltersPrompts()
{
    // Arrange
    var vm = new PromptSelectorViewModel(mockGit, mockAI);
    vm.Prompts.Add(new Prompt { Title = "Test", ... });

    // Act
    vm.SearchQuery = "Test";

    // Assert
    vm.FilteredPrompts.Should().HaveCount(1);
}
```

---

## Deployment Architecture

### Single-File Executable

```bash
dotnet publish -r win-x64 --self-contained /p:PublishSingleFile=true
# Result: PromptSync.exe (15-20 MB, includes .NET runtime)
```

### Platform Matrix

| Platform | RID | Output |
|----------|-----|--------|
| Windows x64 | win-x64 | PromptSync.exe |
| macOS Intel | osx-x64 | PromptSync.app |
| macOS Apple Silicon | osx-arm64 | PromptSync.app |
| Linux x64 | linux-x64 | promptsync |

---

## Future Extensions

### 1. **Plugin System**

```csharp
public interface IPromptSyncPlugin
{
    string Name { get; }
    void Initialize(IServiceProvider services);
    Task<DnaResult> ExecuteAsync(Prompt prompt, CancellationToken ct);
}

// Load plugins from ~/PromptSync/Plugins/*.dll
```

### 2. **Cloud Sync** (beyond GitHub)

- Azure Blob Storage
- AWS S3
- Self-hosted WebDAV

### 3. **Team Collaboration**

- Real-time co-editing (SignalR)
- Access control (RBAC)
- Audit logs (who changed what, when)

---

## Summary

PromptSync's architecture prioritizes:

? **Testability**: 80%+ code coverage through DI and mocking  
? **Maintainability**: SOLID principles, small focused classes  
? **Performance**: <5ms hotkey response, async I/O  
? **Extensibility**: Plugin system, swappable AI providers  
? **Security**: Encrypted prompts, sandboxed execution  
? **Cross-platform**: Single codebase for Windows/macOS/Linux  

**Questions? Open an issue or discussion on GitHub!**
