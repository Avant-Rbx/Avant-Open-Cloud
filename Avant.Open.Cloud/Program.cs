using System.CommandLine;
using System.CommandLine.Invocation;

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
    public static int Main(string[] args)
    {
        var rootCommand = new RootCommand(description: "Runs unit tests using Roblox's Open Cloud Luau Execution.");
        rootCommand.AddOption(DebugOption);
        rootCommand.AddArgument(ConfigurationPathArgument);
        rootCommand.SetHandler(RunApplicationAsync);
        return rootCommand.InvokeAsync(args).Result;
    }
    
    /// <summary>
    /// Runs the application with parsed command line arguments.
    /// </summary>
    /// <param name="invocationContext">Context for the command line options.</param>
    private static async Task RunApplicationAsync(InvocationContext invocationContext)
    {
        // TODO
    }
}