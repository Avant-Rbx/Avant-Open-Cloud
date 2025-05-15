using System.Collections.Generic;
using System.Text.Json.Serialization;
using Avant.Open.Cloud.Diagnostic;

namespace Avant.Open.Cloud.Configuration;

public class RojoBuildStrategyConfiguration
{
    /// <summary>
    /// Path to the Rojo project file.
    /// </summary>
    public string? ProjectFile { get; set; }    
    
    /// <summary>
    /// Directory to add the Avant executor library before exporting.
    /// </summary>
    public string? AvantInjectionDirectory { get; set; }
}

public class OpenCloudConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    public string? ApiKeyEnvironmentVariable { get; set; }
    
    /// <summary>
    /// Universe id to upload the place file to run tests with.
    /// </summary>
    public long? UniverseId { get; set; }
    
    /// <summary>
    /// Place id to upload the place file to run tests with.
    /// </summary>
    public long? PlaceId { get; set; }
}

public class AvantConfiguration
{
    /// <summary>
    /// Strategy for building Rojo before uploading to Roblox.
    /// </summary>
    public RojoBuildStrategyConfiguration? RojoBuildStrategy { get; set; }
    
    /// <summary>
    /// Open Cloud configuration for uploading place files and executing.
    /// </summary>
    public OpenCloudConfiguration? OpenCloud { get; set; }

    /// <summary>
    /// Verifies the configuration.
    /// </summary>
    /// <returns>A list of validation errors.</returns>
    public List<string> VerifyConfiguration()
    {
        // Verify the Rojo build strategy.
        var validationErrors = new List<string>();
        if (this.RojoBuildStrategy == null)
        {
            validationErrors.Add("RojoBuildStrategy is not configured.");
        }
        else
        {
            if (this.RojoBuildStrategy.ProjectFile == null)
            {
                validationErrors.Add("RojoBuildStrategy.ProjectFile is not configured.");
            }
        }
        
        // Validate the Open Cloud configuration.
        if (this.OpenCloud == null)
        {
            validationErrors.Add("OpenCloud is not configured.");
        }
        else
        {
            if (this.OpenCloud.ApiKeyEnvironmentVariable == null)
            {
                validationErrors.Add("OpenCloud.ApiKeyEnvironmentVariable is not configured.");
            }
            if (this.OpenCloud.UniverseId == null)
            {
                validationErrors.Add("OpenCloud.UniverseId is not configured.");
            }
            if (this.OpenCloud.PlaceId == null)
            {
                validationErrors.Add("OpenCloud.PlaceId is not configured.");
            }
        }
        
        // Log the validation errors.
        foreach (var validationError in validationErrors)
        {
            Logger.Error(validationError);
        }
        return validationErrors;
    }
}

[JsonSerializable(typeof(AvantConfiguration))]
[JsonSourceGenerationOptions(WriteIndented = true, IncludeFields = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class AvantConfigurationJsonContext : JsonSerializerContext
{
}