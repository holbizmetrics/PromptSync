# ?? Quick Launch Instructions

## The PowerShell Terminal Issue

The VS Code integrated terminal is having display issues (German PowerShell PSReadLine bug). This doesn't mean the app won't work - we just need a different way to run it.

---

## ? Solution: 3 Easy Ways to Run

### Method 1: Double-Click the Batch File (EASIEST!)

1. Look in your file explorer at: `C:\FromGithubEtc\PromptSync\`
2. Find the file: **`run-desktop.bat`**
3. **Double-click it**
4. A command window will open, build the app, and launch it

**Expected:** A window titled "PromptSync - Select Prompt" should appear!

---

### Method 2: Windows PowerShell (Not VS Code's)

1. Press **Windows Key**
2. Type: **PowerShell**
3. Open **Windows PowerShell** (the regular one, not VS Code's)
4. Run these commands:

```powershell
cd C:\FromGithubEtc\PromptSync
dotnet run --project src\PromptSync.Desktop
```

---

### Method 3: Visual Studio 2022

1. Double-click `PromptSync.sln` in File Explorer
2. Visual Studio 2022 will open
3. In Solution Explorer, right-click **PromptSync.Desktop**
4. Click **"Set as Startup Project"**
5. Press **F5** (or click the green ? Play button)

---

## ?? What Should Happen

When successful, you'll see:

### 1. Command Window Output (if using Method 1 or 2):
```
Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:03.45
```

### 2. A Window Appears:

```
??????????????????????????????????????????????
? PromptSync - Select Prompt          _ ? ×  ?
??????????????????????????????????????????????
?                                             ?
?  PromptSync                                 ?
?                                             ?
?  ??????????????????????????   ?   ?      ?
?  ? Search prompts...      ?               ?
?  ??????????????????????????               ?
?                                             ?
?  ???????????????????????????????????????? ?
?  ?                                       ? ?
?  ?  (Empty - no prompts loaded)         ? ?
?  ?                                       ? ?
?  ???????????????????????????????????????? ?
?                                             ?
?  Ready             [Select]    [Cancel]    ?
?                                             ?
??????????????????????????????????????????????
```

---

## ? If You Get Errors

### Error: "Could not execute because 'dotnet' was not found"

**Fix:** Install .NET 8.0 SDK from https://dotnet.microsoft.com/download

### Error: Build failed with code errors

**Run this to see the errors:**
```powershell
cd C:\FromGithubEtc\PromptSync
dotnet build src\PromptSync.Desktop\PromptSync.Desktop.csproj
```

Copy the error and share it with me.

### Error: Window appears then immediately closes

This means there's a runtime exception. To see it:

1. Open a PowerShell window
2. Run:
   ```powershell
   cd C:\FromGithubEtc\PromptSync
   dotnet run --project src\PromptSync.Desktop 2>&1 | Tee-Object output.log
   ```
3. Share the `output.log` file contents

---

## ?? Alternative: Build First, Then Run

Sometimes it's clearer to build and run separately:

```powershell
# Navigate to project
cd C:\FromGithubEtc\PromptSync

# Build
dotnet build

# If build succeeds, run
dotnet run --project src\PromptSync.Desktop
```

---

## ?? What I Need to Know

After you try one of the methods above, please tell me:

1. **Which method did you use?** (1, 2, or 3)
2. **Did a window appear?** (Yes/No)
3. **If no window, what error message did you see?** (Copy/paste it)
4. **If yes, can you:**
   - Type in the search box?
   - Click the buttons?
   - See "Ready" in the bottom left?

---

## ?? Once It's Running

If you see the window, **congratulations!** The foundation is working. It will be empty because:

- ? UI framework works
- ? MVVM bindings work
- ? Dependency injection works
- ? Prompt loading not implemented yet (that's next!)

**Then we can add sample prompts to test the full UI!**

---

## ?? Pro Tip

If Visual Studio is available, use Method 3 - it's the best developer experience with:
- Built-in debugger
- Breakpoints
- Variable inspection
- IntelliSense
- Hot reload

---

**Try Method 1 (double-click run-desktop.bat) first - it's the easiest! Let me know what happens! ??**
