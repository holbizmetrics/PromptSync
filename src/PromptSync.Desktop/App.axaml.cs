using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PromptSync.Desktop.ViewModels;
using PromptSync.Desktop.Views;
using PromptSync.Core.Services;
using PromptSync.Core.DNA;

namespace PromptSync.Desktop;

/// <summary>
/// Main application class with dependency injection setup.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Gets the service provider for dependency injection.
    /// </summary>
    public IServiceProvider? Services { get; private set; }

    /// <inheritdoc/>
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    /// <inheritdoc/>
    public override void OnFrameworkInitializationCompleted()
    {
        // Configure dependency injection
        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = new PromptSelectorWindow
            {
                DataContext = Services.GetRequiredService<PromptSelectorViewModel>()
            };

            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Register Mock services (temporary - replace with real implementations)
        services.AddSingleton<IGitService, MockGitService>();
        services.AddSingleton<IAIService, MockAIService>();

        // TODO: Register real services when implemented
        // services.AddSingleton<IGitHubService, GitHubService>();

        // Register DNA Lab features (when implemented)
        // services.AddSingleton<IQualityScorer, QualityScorer>();
        // services.AddSingleton<ISecurityScanner, SecurityScanner>();
        // services.AddSingleton<IIterator, Iterator>();
        // services.AddSingleton<IReverseEngineer, ReverseEngineer>();
        // services.AddSingleton<IEncryptor, Encryptor>();
        // services.AddSingleton<IHarvester, Harvester>();

        // Register ViewModels
        services.AddTransient<PromptSelectorViewModel>();

        // Register logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });
    }
}
