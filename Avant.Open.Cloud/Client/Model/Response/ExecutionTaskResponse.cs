using System.Text.Json.Serialization;

namespace Avant.Open.Cloud.Client.Model.Response;

public enum ExecutionState
{
    [JsonStringEnumMemberName("STATE_UNSPECIFIED")]
    StateUnspecified,
    [JsonStringEnumMemberName("QUEUED")]
    Queued,
    [JsonStringEnumMemberName("PROCESSING")]
    Processing,
    [JsonStringEnumMemberName("CANCELLED")]
    Cancelled,
    [JsonStringEnumMemberName("COMPLETE")]
    Complete,
    [JsonStringEnumMemberName("FAILED")]
    Failed,
}

public class ExecutionTaskResponse
{
    /// <summary>
    /// Path of the resourcee.
    /// </summary>
    [JsonPropertyName("path")]
    public string Path { get; set; } = null!;
    
    /// <summary>
    /// Create time of the resource.
    /// </summary>
    [JsonPropertyName("createTime")]
    public string? CreateTime { get; set; }
    
    /// <summary>
    /// Update time of the resource.
    /// </summary>
    [JsonPropertyName("updateTime")]
    public string? UpdateTime { get; set; }
    
    /// <summary>
    /// Roblox user that is running the execution.
    /// </summary>
    [JsonPropertyName("user")]
    public string User { get;set; } = null!;

    /// <summary>
    /// State of the execution.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<ExecutionState>))] 
    [JsonPropertyName("state")]
    public ExecutionState State { get; set; }

    /// <summary>
    /// Script that was requested to be executed.
    /// </summary>
    [JsonPropertyName("script")]
    public string Script { get; set; } = null!;
}

[JsonSerializable(typeof(ExecutionTaskResponse))]
[JsonSourceGenerationOptions(WriteIndented = true, IncludeFields = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class ExecutionTaskResponseJsonContext : JsonSerializerContext
{
}