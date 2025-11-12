using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace PromptSync.HotkeyAgent;

/// <summary>
/// Platform-agnostic hotkey agent entry point.
/// Delegates to platform-specific implementations.
/// </summary>
internal class Program
{
    static async Task<int> Main(string[] args)
    {
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        var logger = loggerFactory.CreateLogger<Program>();

        logger.LogInformation("?? PromptSync Hotkey Agent starting...");
        logger.LogInformation("Platform: {Platform}", GetPlatformName());

        try
        {
            // Determine platform and start appropriate agent
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                logger.LogInformation("Starting Windows hotkey agent");
                // await WindowsHotkeyAgent.StartAsync(logger);
                logger.LogInformation("Windows agent not yet implemented");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                logger.LogInformation("Starting macOS hotkey agent");
                logger.LogInformation("macOS agent not yet implemented");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                logger.LogInformation("Starting Linux hotkey agent");
                logger.LogInformation("Linux agent not yet implemented");
            }
            else
            {
                logger.LogError("Unsupported platform");
                return 1;
            }

            logger.LogInformation("Hotkey agent running. Press Ctrl+C to exit.");
            
            // Keep running until interrupted
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) =>
            {
                logger.LogInformation("Shutdown requested");
                cts.Cancel();
                e.Cancel = true;
            };

            await Task.Delay(-1, cts.Token);
            return 0;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Fatal error in hotkey agent");
            return 1;
        }
    }

    private static string GetPlatformName()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return "Windows";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return "macOS";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return "Linux";
        return "Unknown";
    }
}
