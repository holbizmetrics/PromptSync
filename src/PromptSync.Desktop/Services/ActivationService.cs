using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Security.Cryptography;
using Avalonia;
using Avalonia.Threading;

namespace PromptSync.Desktop.Services;

/// <summary>
/// Minimal loopback HTTP activation service. Binds to 127.0.0.1 and a random available port.
/// Generates a short-lived token written to a token file. HotkeyAgent must POST /activate
/// with Authorization: Bearer {token} and a small JSON payload to trigger activation.
/// </summary>
public class ActivationService : IActivationService, IDisposable
{
    private readonly ILogger<ActivationService> _logger;
    private HttpListener? _listener;
    private CancellationTokenSource? _cts;
    private Task? _listenTask;
    private readonly string _token;
    private readonly string _tokenFilePath;
    private readonly string _activationJsonPath;

    public ActivationService(ILogger<ActivationService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        _tokenFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PromptSync", "activation.token");
        Directory.CreateDirectory(Path.GetDirectoryName(_tokenFilePath) ?? Path.GetTempPath());
        File.WriteAllText(_tokenFilePath, _token, Encoding.UTF8);
        _activationJsonPath = Path.Combine(Path.GetDirectoryName(_tokenFilePath)!, "activation.json");
    }

    public string TokenFilePath => _tokenFilePath;

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (_listener != null) return Task.CompletedTask;

        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _listener = new HttpListener();

        // Bind to a random loopback port
        var listenerPort = GetFreePort();
        var prefix = $"http://127.0.0.1:{listenerPort}/";

        _logger.LogInformation("Activation service listening at {Prefix}", prefix);

        _listener.Prefixes.Add(prefix);
        _listener.Start();

        // Write activation discovery file (port + token)
        try
        {
            var payload = new { port = listenerPort, token = _token, created = DateTimeOffset.UtcNow };
            var json = JsonSerializer.Serialize(payload);
            File.WriteAllText(_activationJsonPath, json, Encoding.UTF8);
            _logger.LogInformation("Wrote activation discovery file at {Path}", _activationJsonPath);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to write activation discovery file");
        }

        _listenTask = Task.Run(() => ListenLoopAsync(_cts.Token));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        _cts?.Cancel();
        try
        {
            _listener?.Stop();
        }
        catch { }
        return Task.CompletedTask;
    }

    private async Task ListenLoopAsync(CancellationToken ct)
    {
        if (_listener == null) return;

        while (!ct.IsCancellationRequested)
        {
            try
            {
                var context = await _listener.GetContextAsync().ConfigureAwait(false);
                _ = Task.Run(() => HandleRequestAsync(context), ct);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Activation listener error");
                await Task.Delay(1000, ct).ConfigureAwait(false);
            }
        }
    }

    private async Task HandleRequestAsync(HttpListenerContext context)
    {
        try
        {
            if (context.Request.HttpMethod != "POST" || context.Request.Url == null)
            {
                context.Response.StatusCode = 405;
                context.Response.Close();
                return;
            }

            if (!context.Request.Url.AbsolutePath.Equals("/activate", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = 404;
                context.Response.Close();
                return;
            }

            var auth = context.Request.Headers["Authorization"];
            if (string.IsNullOrWhiteSpace(auth) || !auth.StartsWith("Bearer "))
            {
                context.Response.StatusCode = 401;
                context.Response.Close();
                return;
            }

            var token = auth.Substring("Bearer ".Length).Trim();
            if (!CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(token), Encoding.UTF8.GetBytes(_token)))
            {
                context.Response.StatusCode = 403;
                context.Response.Close();
                return;
            }

            using var sr = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
            var body = await sr.ReadToEndAsync().ConfigureAwait(false);
            var payload = JsonSerializer.Deserialize<Dictionary<string, object>>(body);

            // Simple acknowledgement
            context.Response.StatusCode = 200;
            var resp = Encoding.UTF8.GetBytes("{\"ok\":true}");
            context.Response.ContentType = "application/json";
            await context.Response.OutputStream.WriteAsync(resp, 0, resp.Length).ConfigureAwait(false);
            context.Response.Close();

            _logger.LogInformation("Received activation request: {Payload}", payload);

            // Raise a desktop activation event: for now, bring main window to front via App.Current
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (Avalonia.Application.Current.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
                {
                    var window = desktop.MainWindow as Views.PromptSelectorWindow;
                    if (window != null)
                    {
                        window.Activate();
                    }
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to handle activation request");
            try { context.Response.StatusCode = 500; context.Response.Close(); } catch { }
        }
    }

    private static int GetFreePort()
    {
        var listener = new System.Net.Sockets.TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    public void Dispose()
    {
        try
        {
            _listener?.Close();
            _cts?.Cancel();
        }
        catch { }
    }
}
