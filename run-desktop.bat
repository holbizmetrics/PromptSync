@echo off
echo ===================================
echo  PromptSync Desktop App Launcher
echo ===================================
echo.
echo Building...
dotnet build src\PromptSync.Desktop\PromptSync.Desktop.csproj

if %ERRORLEVEL% EQU 0 (
    echo.
    echo Build successful! Starting app...
    echo.
    dotnet run --project src\PromptSync.Desktop
) else (
    echo.
    echo Build FAILED! Check errors above.
    echo.
    pause
)
