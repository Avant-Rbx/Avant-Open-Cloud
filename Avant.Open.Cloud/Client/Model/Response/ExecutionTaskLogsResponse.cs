using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Avant.Open.Cloud.Client.Model.Response;

public enum MessageType
{
    [EnumMember(Value = "MESSAGE_TYPE_UNSPECIFIED")]
    Unspecified,
    [EnumMember(Value = "OUTPUT")]
    Output,
    [EnumMember(Value = "INFO")]
    Info,
    [EnumMember(Value = "WARNING")]
    Warning,
    [EnumMember(Value = "ERROR")]
    Error,
}

public class StructuredMessage
{
    /// <summary>
    /// Content of the message.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = null!;
    
    /// <summary>
    /// Time the message was created.
    /// </summary>
    [JsonPropertyName("createTime")]
    public DateTime CreateTime { get; set; }
    
    /// <summary>
    /// Type of the message.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<MessageType>))] 
    [JsonPropertyName("messageType")]
    public MessageType MessageType { get; set; }
}

public class LuauExecutionSessionTaskLogs
{
    /// <summary>
    /// Path of the resource.
    /// </summary>
    [JsonPropertyName("path")]
    public string Path { get; set; } = null!;
    
    /// <summary>
    /// Unstructured messages in the logs.
    /// </summary>
    [JsonPropertyName("messages")]
    public List<string>? Messages { get; set; }
    
    /// <summary>
    /// Structured messages in the logs.
    /// </summary>
    [JsonPropertyName("structuredMessages")]
    public List<StructuredMessage>? StructuredMessages { get; set; }
}

public class ExecutionTaskLogsResponse
{
    /// <summary>
    /// Logs for the request.
    /// </summary>
    [JsonPropertyName("luauExecutionSessionTaskLogs")]
    public List<LuauExecutionSessionTaskLogs> LuauExecutionSessionTaskLogs { get; set; } = null!;

    /// <summary>
    /// Token for the next page of logs.
    /// </summary>
    [JsonPropertyName("nextPageToken")]
    public string? NextPageToken { get; set; }
}

[JsonSerializable(typeof(ExecutionTaskLogsResponse))]
[JsonSourceGenerationOptions(WriteIndented = true, IncludeFields = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class ExecutionTaskLogsResponseJsonContext : JsonSerializerContext
{
}