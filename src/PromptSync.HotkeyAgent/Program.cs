using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Net.Http;
using System.IO;

namespace PromptSync.HotkeyAgent;

/// <summary>
/// Platform-agnostic hotkey agent entry point.
/// Delegates to platform-specific implementations.
/// Also supports a test activation mode: --test-activate
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

        // Test activation mode
        if (args.Length > 0 && args[0] == "--test-activate")
        {
            return await RunTestActivate(logger);
        }

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

    private static async Task<int> RunTestActivate(ILogger logger)
    {
        try
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var path = Path.Combine(appData, "PromptSync", "activation.json");
            if (!File.Exists(path))
            {
                logger.LogError("activation.json not found: {Path}", path);
                return 2;
            }

            var json = await File.ReadAllTextAsync(path);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            var port = root.GetProperty("port").GetInt32();
            var token = root.GetProperty("token").GetString();

            var url = $"http://127.0.0.1:{port}/activate";
            using var http = new HttpClient();
            http.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var payload = new { hotkey = "Ctrl+Shift+P", activeApp = "code", timestamp = DateTimeOffset.UtcNow };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var resp = await http.PostAsync(url, content);
            var respBody = await resp.Content.ReadAsStringAsync();
            logger.LogInformation("POST {Url} => {Status}: {Body}", url, resp.StatusCode, respBody);
            return resp.IsSuccessStatusCode ? 0 : 3;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Test activate failed");
            return 4;
        }
    }
}
