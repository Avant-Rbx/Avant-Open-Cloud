using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avant.Open.Cloud.Client;
using Avant.Open.Cloud.Client.Model.Response;
using Avant.Open.Cloud.Client.Shim;
using Avant.Open.Cloud.Configuration;
using Avant.Open.Cloud.Diagnostic;
using Microsoft.Extensions.Logging;
using Nexus.Logging.Output;

namespace Avant.Open.Cloud.Action;

public class OpenCloudExecution
{
    /// <summary>
    /// List of execution states considered in-progress.
    /// </summary>
    public static readonly List<ExecutionState> InProgressStates = new List<ExecutionState>()
    {
        ExecutionState.Queued,
        ExecutionState.Processing,
    };

    /// <summary>
    /// Mapping of a message type to a log level.
    /// </summary>
    public static readonly Dictionary<MessageType, LogLevel> MessageTypeToLoveLevel = new Dictionary<MessageType, LogLevel>()
    {
        { MessageType.Unspecified, LogLevel.Information },
        { MessageType.Output, LogLevel.Information },
        { MessageType.Info, LogLevel.Trace },
        { MessageType.Warning, LogLevel.Warning },
        { MessageType.Error, LogLevel.Error },
    };
    
    /// <summary>
    /// Configuration for Open Cloud.
    /// </summary>
    public readonly OpenCloudConfiguration OpenCloudConfiguration;

    /// <summary>
    /// Open Cloud client.
    /// </summary>
    public readonly OpenCloudClient OpenCloudClient;
    
    /// <summary>
    /// Creates an Open Cloud execution job.
    /// </summary>
    /// <param name="openCloudApiKey">Open Cloud API key.</param>
    /// <param name="openCloudConfiguration">Configuration for Open Cloud.</param>
    /// <param name="httpClient">Optional HTTP client. Used for tests.</param>
    public OpenCloudExecution(string openCloudApiKey, OpenCloudConfiguration openCloudConfiguration, IHttpClientShim? httpClient = null)
    {
        this.OpenCloudConfiguration = openCloudConfiguration;
        this.OpenCloudClient = new OpenCloudClient(openCloudApiKey, httpClient);
    }
    
    /// <summary>
    /// Runs Open Cloud Luau execution.
    /// </summary>
    /// <param name="placeFilePath">Path of the place file to upload.</param>
    /// <returns>Whether the execution was successful.</returns>
    public async Task<bool> RunOpenCloudExecutionAsync(string placeFilePath)
    {
        try
        {
            // Read the test runner script.
            var assembly = this.GetType().Assembly;
            var runTestFilePath = assembly.GetManifestResourceNames().FirstOrDefault(name => Regex.Match(name, "RunTests\\.luau").Success);
            if (runTestFilePath == null)
            {
                Logger.Error("Embedded RunTests.luau script not found.");
                return false;
            }
            await using var runTestFileStream = assembly.GetManifestResourceStream(runTestFilePath)!;
            var runTestFile = await new StreamReader(runTestFileStream).ReadToEndAsync();
            
            // Publish the place file.
            var universeId = this.OpenCloudConfiguration.UniverseId!.Value;
            var placeId = this.OpenCloudConfiguration.PlaceId!.Value;
            var publishResponse = await this.OpenCloudClient.PublishPlaceAsync(universeId, placeId, placeFilePath);

            // Start the execution task.
            var executionTaskResponse = await this.OpenCloudClient.StartExecutionTaskAsync(universeId, placeId, publishResponse.VersionNumber, runTestFile);
            
            // Wait for the execution to complete.
            Console.Write("[Info ] Waiting for execution to complete..."); // TODO: Workaround to make single-line process work.
            while (true)
            {
                executionTaskResponse = await this.OpenCloudClient.GetExecutionTaskStateAsync(executionTaskResponse.Path);
                if (!InProgressStates.Contains(executionTaskResponse.State)) break;
                await Task.Delay(3000);
                Console.Write(".");
            }
            Console.WriteLine("");
            Logger.Info($"Test execution completed with state {executionTaskResponse.State}.");
            
            // Get the logs of the execution.
            var logsResponse = await this.OpenCloudClient.GetExecutionTaskLogsAsync(executionTaskResponse.Path);
            var consoleOutput = new ConsoleOutput();
            consoleOutput.MinimumLevel = LogLevel.Trace;
            foreach (var logSession in logsResponse.LuauExecutionSessionTaskLogs)
            {
                if (logSession.StructuredMessages == null) continue;
                foreach (var log in logSession.StructuredMessages)
                {
                    foreach (var line in log.Message.Split("\n"))
                    {
                        consoleOutput.LogMessage($"[{log.CreateTime:HH:mm:ss}] {line}", MessageTypeToLoveLevel[log.MessageType]);
                    }
                }
            }
            await consoleOutput.WaitForCompletionAsync();

            // Return if the test passed.
            return executionTaskResponse.State == ExecutionState.Complete;
        }
        catch (HttpRequestException e)
        {
            // Log the exception.
            Logger.Error(e.Message);
        }
        return false;
    }
}