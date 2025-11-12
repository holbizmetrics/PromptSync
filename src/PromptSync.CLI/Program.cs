using System.CommandLine;

namespace PromptSync.CLI;

/// <summary>
/// Command-line interface for PromptSync.
/// Provides prompt management without GUI.
/// </summary>
internal class Program
{
    /// <summary>
    /// Main entry point for the CLI.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    /// <returns>Exit code.</returns>
    static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("PromptSync - Git-based AI prompt management");

        // List command
        var listCommand = new Command("list", "List all available prompts");
        listCommand.SetHandler(ListPrompts);
        rootCommand.AddCommand(listCommand);

        // Search command
        var searchCommand = new Command("search", "Search for prompts");
        var queryArgument = new Argument<string>("query", "Search query");
        searchCommand.AddArgument(queryArgument);
        searchCommand.SetHandler(SearchPrompts, queryArgument);
        rootCommand.AddCommand(searchCommand);

        // Sync command
        var syncCommand = new Command("sync", "Sync with GitHub repository");
        syncCommand.SetHandler(SyncRepository);
        rootCommand.AddCommand(syncCommand);

        // Score command
        var scoreCommand = new Command("score", "Score a prompt's quality");
        var promptFileArgument = new Argument<string>("file", "Path to prompt file");
        scoreCommand.AddArgument(promptFileArgument);
        scoreCommand.SetHandler(ScorePrompt, promptFileArgument);
        rootCommand.AddCommand(scoreCommand);

        return await rootCommand.InvokeAsync(args);
    }

    private static void ListPrompts()
    {
        Console.WriteLine("?? Available Prompts:");
        Console.WriteLine("  (Implementation coming soon)");
    }

    private static void SearchPrompts(string query)
    {
        Console.WriteLine($"?? Searching for: {query}");
        Console.WriteLine("  (Implementation coming soon)");
    }

    private static void SyncRepository()
    {
        Console.WriteLine("?? Syncing with GitHub...");
        Console.WriteLine("  (Implementation coming soon)");
    }

    private static void ScorePrompt(string filePath)
    {
        Console.WriteLine($"?? Scoring prompt: {filePath}");
        Console.WriteLine("  (Implementation coming soon)");
    }
}
