using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avant.Open.Cloud.Configuration;
using Avant.Open.Cloud.Diagnostic;

namespace Avant.Open.Cloud.Action;

public class RojoBuild
{
    /// <summary>
    /// Tag to download for the Avant Runtime.
    /// </summary>
    public const string AvantRuntimeTag = "V.1.3.0";
    
    /// <summary>
    /// URL to download the Avant Runtime if a bundled version isn't included.
    /// </summary>
    public const string AvantRuntimeDownloadUrl = $"https://github.com/Avant-Rbx/Avant-Runtime/releases/download/{AvantRuntimeTag}/AvantRuntime.rbxmx";
    
    /// <summary>
    /// Working directory of the Rojo project.
    /// </summary>
    public readonly string WorkingDirectory;
    
    /// <summary>
    /// Configuration for building with Rojo.
    /// </summary>
    public readonly RojoBuildStrategyConfiguration RojoBuildStrategy;
    
    /// <summary>
    /// Creates a Rojo build action.
    /// </summary>
    /// <param name="workingDirectory">Working directory of the Rojo project.</param>
    /// <param name="rojoBuildStrategy">Configuration for building with Rojo.</param>
    public RojoBuild(string workingDirectory, RojoBuildStrategyConfiguration rojoBuildStrategy)
    {
        this.WorkingDirectory = workingDirectory;
        this.RojoBuildStrategy = rojoBuildStrategy;    
    }

    /// <summary>
    /// Builds the project.
    /// </summary>
    /// <returns>The Roblox place file if the build completed.</returns>
    public async Task<string?> BuildProjectAsync()
    {
        // Get the paths.
        var projectFilePath = Path.Combine(this.WorkingDirectory, this.RojoBuildStrategy.ProjectFile!);
        Logger.Debug($"Rojo project file path: {projectFilePath}");
        var buildPath = Path.GetTempFileName() + ".rbxl";
        Logger.Debug($"Rojo output path: {buildPath}");
        
        // Find Rojo in the system path.
        var systemPath = Environment.GetEnvironmentVariable("PATH")!;
        string? rojoPath = null;
        foreach (var systemPathEntry in systemPath.Split(Path.PathSeparator))
        {
            var newRojoPath = Path.Combine(systemPathEntry, "rojo");
            var newRojoExePath = Path.Combine(systemPathEntry, "rojo.exe");
            Logger.Debug($"Checking for Rojo in: {systemPathEntry}");
            if (File.Exists(newRojoPath))
            {
                rojoPath = newRojoPath;
                break;
            }
            if (File.Exists(newRojoExePath))
            {
                rojoPath = newRojoExePath;
                break;
            }
        }
        if (rojoPath == null)
        {
            Logger.Error("Rojo was not found in the system PATH.");
            return null;
        }
        Logger.Debug($"Using Rojo at: {rojoPath}");
        
        // Add the Avant runtime.
        string? avantRuntimeInjectPath = null;
        if (this.RojoBuildStrategy.AvantInjectionDirectory != null)
        {
            // Find the Avant runtime.
            var assembly = this.GetType().Assembly;
            var avantRuntimeFilePath = assembly.GetManifestResourceNames().FirstOrDefault(name => Regex.Match(name, "AvantRuntime\\.").Success);
            Stream? avantRuntimeFileStream = null;
            if (avantRuntimeFilePath != null)
            {
                // Read the bundled runtime.
                Logger.Debug("Using bundled Avant Runtime.");
                avantRuntimeFileStream = assembly.GetManifestResourceStream(avantRuntimeFilePath)!;
            }
            else
            {
                // Download the runtime.
                avantRuntimeFilePath = $"AvantRuntime-{AvantRuntimeTag}{Path.GetExtension(AvantRuntimeDownloadUrl)}";
                var downloadPath = Path.Combine(Path.GetTempPath(), avantRuntimeFilePath);
                if (!File.Exists(downloadPath))
                {
                    Logger.Debug($"Downloading Avant Runtime from {AvantRuntimeDownloadUrl} to: {downloadPath}");
                    var client = new HttpClient();
                    await File.WriteAllBytesAsync(downloadPath, await (await client.GetAsync(AvantRuntimeDownloadUrl)).Content.ReadAsByteArrayAsync());
                }
                
                // Read the runtime.
                Logger.Debug($"Using downloaded Avant Runtime from: {downloadPath}");
                avantRuntimeFileStream = File.Open(downloadPath, FileMode.Open)!;
            }
            
            // Copy the runtime.
            avantRuntimeInjectPath = Path.Combine(this.WorkingDirectory, this.RojoBuildStrategy.AvantInjectionDirectory,
                $"AvantRuntime{Path.GetExtension(avantRuntimeFilePath)}");
            if (!File.Exists(avantRuntimeInjectPath))
            {
                Logger.Debug($"Injecting AvantRuntime to: {avantRuntimeInjectPath}");
                var avantRuntimeInjectFile = File.Create(avantRuntimeInjectPath);
                await avantRuntimeFileStream.CopyToAsync(avantRuntimeInjectFile);
                avantRuntimeInjectFile.Close();
            }
            else
            {
                Logger.Debug($"AvantRuntime already found at: {avantRuntimeInjectPath}");
            }
        }
        
        // Build the project.
        try
        {
            // Run the Rojo process.
            var rojoProcess = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = rojoPath,
                    WorkingDirectory = this.WorkingDirectory,
                    ArgumentList = { "build", projectFilePath, "--output", buildPath },
                },
            };
            rojoProcess.Start();
            await rojoProcess.WaitForExitAsync();
            
            // Return if the process failed.
            if (rojoProcess.ExitCode != 0)
            {
                Logger.Error($"Rojo returned exit code {rojoProcess.ExitCode}.");
                return null;
            }
        }
        finally
        {
            // Remove the Avant runner.
            if (avantRuntimeInjectPath != null && File.Exists(avantRuntimeInjectPath))
            {
                Logger.Debug($"Deleting injected AvantRuntime to: {avantRuntimeInjectPath}");
                File.Delete(avantRuntimeInjectPath);
            }
        }
        
        // Return the build path.
        return buildPath;
    }
}