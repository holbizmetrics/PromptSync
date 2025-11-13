# Testing PromptSync Desktop Application

## Current Issue

The PowerShell terminal has display issues, but this doesn't necessarily mean the app failed. Let's diagnose:

## Diagnostic Steps

### 1. Check if the app is actually running

**Look for these signs:**
- Did an Avalonia window appear on your screen?
- Is there a process in Task Manager called "PromptSync.Desktop"?

### 2. Expected Behavior When Running

When you run `dotnet run --project src/PromptSync.Desktop`, you should see:

? **Success indicators:**
- Console output showing: "Building..." then "Running..."
- A window appears with title "PromptSync - Select Prompt"
- The window has a search box and empty prompt list

? **Failure indicators:**
- Exception stack trace in console
- No window appears
- App exits immediately

### 3. Common Issues & Solutions

#### Issue: "No services registered"

**Symptom:** Exception about missing services in DI container

**Fix:** The services are commented out in `App.axaml.cs`. We need to register at least mock implementations:

```csharp
private void ConfigureServices(IServiceCollection services)
{
    // Register mock services for now (replace with real implementations later)
    services.AddSingleton<IGitService>(sp => 
        new MockGitService());
    
    services.AddSingleton<IAIService>(sp => 
        new MockAIService());

    // Register ViewModels
    services.AddTransient<PromptSelectorViewModel>();

    // Register logging
    services.AddLogging(builder => builder.AddConsole());
}
```

#### Issue: "Cannot find InitializeComponent"

**Symptom:** Compilation error in Window code-behind

**Fix:** Add Avalonia.Markup.Xaml.AvaloniaXamlLoader reference (already done)

#### Issue: Window appears but crashes immediately

**Symptom:** Window flashes then closes

**Fix:** Check for exceptions in ViewModels during initialization

### 4. Manual Test (Alternative Method)

If the terminal is problematic, try running from Visual Studio:

1. Open `PromptSync.sln` in Visual Studio 2022
2. Set `PromptSync.Desktop` as startup project (right-click ? Set as Startup Project)
3. Press **F5** (Start Debugging)
4. Watch the Output window for any errors

Or use Windows Terminal instead of VS Code's terminal:

```bash
# Open Windows Terminal
# Navigate to project
cd C:\FromGithubEtc\PromptSync

# Run
dotnet run --project src/PromptSync.Desktop
```

### 5. What Should Happen (Expected Result)

**On successful launch:**

```
Building...
Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:05.23

[Avalonia] Initializing...
[Avalonia] Platform: Windows
[Avalonia] Rendering: DirectX
```

**Then:** A window appears that looks like this:

```
???????????????????????????????????????????????????????
?  PromptSync                                    ? × ?
???????????????????????????????????????????????????????
?                                                      ?
?  [Search prompts...]                    [?] [?]    ?
?                                                      ?
?  ?????????????????????????????????????????????????? ?
?  ?                                                 ? ?
?  ?  (No prompts loaded yet)                       ? ?
?  ?                                                 ? ?
?  ?                                                 ? ?
?  ?????????????????????????????????????????????????? ?
?                                                      ?
?  Ready                         [Select]  [Cancel]   ?
?                                                      ?
???????????????????????????????????????????????????????
```

### 6. Known Limitation: Services Not Implemented

The app will start, but many features won't work yet because:

- ? `IGitService` not registered (commented out)
- ? `IAIService` not registered (commented out)
- ? Prompt loading not implemented

This is **expected** and **normal** for this stage!

### 7. Quick Test Code

To verify the app can start with mock services, create this file:

**File:** `src/PromptSync.Core/Services/MockGitService.cs`

```csharp
using Microsoft.Extensions.Logging;

namespace PromptSync.Core.Services;

/// <summary>
/// Mock Git service for testing UI without real Git operations.
/// </summary>
public class MockGitService : IGitService
{
    private readonly ILogger<MockGitService>? _logger;

    public MockGitService(ILogger<MockGitService>? logger = null)
    {
        _logger = logger;
    }

    public Task CloneAsync(string repositoryUrl, string localPath, CancellationToken ct = default)
    {
        _logger?.LogInformation("Mock: Clone {Url} to {Path}", repositoryUrl, localPath);
        return Task.CompletedTask;
    }

    public Task PullAsync(string localPath, CancellationToken ct = default)
    {
        _logger?.LogInformation("Mock: Pull from {Path}", localPath);
        return Task.CompletedTask;
    }

    public Task PushAsync(string localPath, string message, CancellationToken ct = default)
    {
        _logger?.LogInformation("Mock: Push to {Path} with message: {Message}", localPath, message);
        return Task.CompletedTask;
    }

    public Task CommitAsync(string localPath, string message, CancellationToken ct = default)
    {
        _logger?.LogInformation("Mock: Commit at {Path} with message: {Message}", localPath, message);
        return Task.CompletedTask;
    }

    public bool HasChanges(string localPath)
    {
        return false; // No changes in mock
    }

    public string GetCurrentBranch(string localPath)
    {
        return "main"; // Always return main branch
    }

    public bool IsValidRepository(string localPath)
    {
        return true; // Always valid in mock
    }
}
```

Then update `App.axaml.cs`:

```csharp
private void ConfigureServices(IServiceCollection services)
{
    // Register Mock services (temporary, for UI testing)
    services.AddSingleton<IGitService, MockGitService>();
    
    // Mock AI service (you'll need to create this too)
    // services.AddSingleton<IAIService, MockAIService>();

    // Register ViewModels
    services.AddTransient<PromptSelectorViewModel>();

    // Register logging
    services.AddLogging(builder => builder.AddConsole());
}
```

### 8. What to Report Back

Please let me know:

1. **Did a window appear?** (Yes/No)
2. **Any error messages in console?** (Copy the error if any)
3. **Using Visual Studio or VS Code?**
4. **Want me to create the mock services?** (So the app can start properly)

---

## Next Steps Based on Results

### If Window Appeared ?
? **Success!** The foundation works. Now we can:
- Create mock services
- Implement real services
- Add prompt loading

### If Error About Missing Services ??
? I'll create mock implementations so you can test the UI

### If Other Error ?
? Share the error and I'll fix it immediately

---

**Let me know what happened and I'll help you get it running!**
