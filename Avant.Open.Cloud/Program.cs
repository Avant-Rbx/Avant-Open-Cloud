using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Avant.Open.Cloud.Configuration;
using Avant.Open.Cloud.Diagnostic;
using Microsoft.Extensions.Logging;

namespace Avant.Open.Cloud;

public class Program
{
    /// <summary>
    /// Command option for enabling debug logging.
    /// </summary>
    public static readonly Option<bool> DebugOption = new Option<bool>("--debug", "Enables debug logging.");

    /// <summary>
    /// Argument for the configuration path.
    /// </summary>
    public static readonly Argument<string> ConfigurationPathArgument = new Argument<string>("ConfigurationPath", () => "avant.json", "Path to the Avant configuration file.");
    
    /// <summary>
    /// Runs the application.
    /// </summary>
    /// <param name="args">Arguments from the command line.</param>
    /// <returns>Status code of the application.</returns>
    public static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand(description: "Runs unit tests using Roblox's Open Cloud Luau Execution.");
        rootCommand.AddOption(DebugOption);
        rootCommand.AddArgument(ConfigurationPathArgument);
        rootCommand.SetHandler(RunApplicationAsync);
        try
        {
            return await rootCommand.InvokeAsync(args);
        }
        finally
        {
            await Logger.WaitForCompletionAsync();
        }
    }
    
    /// <summary>
    /// Runs the application with parsed command line arguments.
    /// </summary>
    /// <param name="invocationContext">Context for the command line options.</param>
    private static async Task<int> RunApplicationAsync(InvocationContext invocationContext)
    {
        // Set the minimum log level.
        if (invocationContext.ParseResult.GetValueForOption(DebugOption))
        {
            Logger.SetMinimumLogLevel(LogLevel.Debug);
            Logger.Debug("Enabling debug logging.");
        }
        
        // Return if the configuration file doesn't exist.
        var configurationFilePath = Path.GetFullPath(invocationContext.ParseResult.GetValueForArgument(ConfigurationPathArgument));
        if (!File.Exists(configurationFilePath))
        {
            Logger.Error($"Configuration file not found at: {configurationFilePath}");
            return -1;
        }
        Logger.Debug($"Using configuration file at: {configurationFilePath}");
        
        // Read and verify the configuration.
        AvantConfiguration configuration;
        try
        {
            configuration = JsonSerializer.Deserialize<AvantConfiguration>(await File.ReadAllTextAsync(configurationFilePath))!;
        }
        catch (JsonException e)
        {
            Logger.Error($"Error reading configuration file: {e.Message}");
            return -1;
        }
        Logger.Debug("Configuration file read.");
        if (configuration.VerifyConfiguration().Count > 0)
        {
            return -1;
        }
        Logger.Debug("Configuration file verified.");
        
        // Return the success exit code.
        return 0;
    }
}