using System.Text.Json.Serialization;

namespace Avant.Open.Cloud.Client.Model.Response;

public class PublishResponse
{
    /// <summary>
    /// Version number that was published.
    /// </summary>
    [JsonPropertyName("versionNumber")]
    public long VersionNumber { get; set; }
}

[JsonSerializable(typeof(PublishResponse))]
[JsonSourceGenerationOptions(WriteIndented = true, IncludeFields = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class PublishResponseJsonContext : JsonSerializerContext
{
}