using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avant.Open.Cloud.Configuration;
using Avant.Open.Cloud.Diagnostic;

namespace Avant.Open.Cloud.Action;

public class RojoBuild
{
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
            if (avantRuntimeFilePath == null)
            {
                Logger.Error("Embedded AvantRuntime.rbxmx or AvantRuntime.rbxm not found. If compiled from source, this can be fetched from GitHub (https://github.com/Avant-Rbx/Avant-Runtime/releases) and added to Avant.Open.Cloud/Resources.");
                return null;
            }
            await using var avantRuntimeFileStream = assembly.GetManifestResourceStream(avantRuntimeFilePath)!;
            
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