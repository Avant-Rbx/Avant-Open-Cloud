using System.Text.Json.Serialization;

namespace Avant.Open.Cloud.Client.Model.Request;

public class StartExecutionTaskRequest
{
    /// <summary>
    /// Script to run.
    /// </summary>
    [JsonPropertyName("script")]
    public string? Script { get; set; }
    
    /// <summary>
    /// Timeout for the execution.
    /// </summary>
    [JsonPropertyName("timeout")]
    public string? Timeout { get; set; }
}

[JsonSerializable(typeof(StartExecutionTaskRequest))]
[JsonSourceGenerationOptions(WriteIndented = true, IncludeFields = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class StartExecutionTaskRequestJsonContext : JsonSerializerContext
{
}