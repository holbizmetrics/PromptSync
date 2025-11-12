# ?? PromptSync

**The Git of AI Prompts - with intelligence built in**

> Stop copy-pasting prompts. Start managing them like code.

This repository contains PromptSync — a Git-backed prompt management tool rebuilt in C#/.NET 8 with a cross-platform desktop UI and a DNA Lab of AI-powered features.

Quick links
- Project root: PromptSync.sln
- Desktop app: `src/PromptSync.Desktop`
- Hotkey agent: `src/PromptSync.HotkeyAgent`
- Core logic (DNA Lab): `src/PromptSync.Core`

Quick start

```sh
# Restore and build
dotnet restore
dotnet build

# Run desktop app
dotnet run --project src/PromptSync.Desktop

# Test HotkeyAgent activation (after desktop started)
dotnet run --project src/PromptSync.HotkeyAgent -- --test-activate
```

Summary
- Hotkey ? popup ? select ? prompt copied to clipboard
- Prompts stored as markdown files in a Git repo (Git-first)
- DNA Lab: reverse-engineer, iterate, security scan, quality scoring, encryption, harvesting
- HotkeyAgent: lightweight platform-specific process that signals the desktop app via local IPC

See `ROADMAP.md` for development phases and priorities.
