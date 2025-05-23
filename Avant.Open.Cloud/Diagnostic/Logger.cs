﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nexus.Logging.Attribute;
using Nexus.Logging.Output;

namespace Avant.Open.Cloud.Diagnostic;

public class Logger
{
    /// <summary>
    /// Static instance of the logger.
    /// </summary>
    public static readonly Nexus.Logging.Logger NexusLogger = new Nexus.Logging.Logger();

    /// <summary>
    /// Static instance of the console output.
    /// </summary>
    private static readonly ConsoleOutput ConsoleOutput = new ConsoleOutput()
    {
        NamespaceWhitelist = new List<string>() { "Avant.Open.Cloud" },
        MinimumLevel = LogLevel.Information,
        DefaultLineWidth = 200,
    };
    
    /// <summary>
    /// Sets up the logging.
    /// </summary>
    static Logger()
    {
        NexusLogger.Outputs.Add(ConsoleOutput);
    }
    
    /// <summary>
    /// Sets the minimum level for logging.
    /// </summary>
    public static void SetMinimumLogLevel(LogLevel logLevel)
    {
        ConsoleOutput.MinimumLevel = logLevel;
    }
    
    /// <summary>
    /// Logs a message as a Debug level.
    /// </summary>
    /// <param name="content">Content to log. Can be an object, like an exception.</param>
    [LogTraceIgnore]
    public static void Debug(object content)
    {
        NexusLogger.Debug(content);
    }

    /// <summary>
    /// Logs a message as an Information level.
    /// </summary>
    /// <param name="content">Content to log. Can be an object, like an exception.</param>
    [LogTraceIgnore]
    public static void Info(object content)
    {
        NexusLogger.Info(content);
    }

    /// <summary>
    /// Logs a message as a Warning level.
    /// </summary>
    /// <param name="content">Content to log. Can be an object, like an exception.</param>
    [LogTraceIgnore]
    public static void Warn(object content)
    {
        NexusLogger.Warn(content);
    }

    /// <summary>
    /// Logs a message as an Error level.
    /// </summary>
    /// <param name="content">Content to log. Can be an object, like an exception.</param>
    [LogTraceIgnore]
    public static void Error(object content)
    {
        NexusLogger.Error(content);
    }
    
    /// <summary>
    /// Waits for all loggers to finish processing logs.
    /// </summary>
    public static async Task WaitForCompletionAsync()
    {
        await NexusLogger.WaitForCompletionAsync();
    }
}