using System.Threading.Tasks;

namespace PromptSync.Desktop.Services;

/// <summary>
/// Service that listens for local activation requests from the HotkeyAgent.
/// </summary>
public interface IActivationService
{
    /// <summary>
    /// Starts the activation listener.
    /// </summary>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops the activation listener.
    /// </summary>
    Task StopAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the token file path that agents can read to authenticate.
    /// </summary>
    string TokenFilePath { get; }
}
