using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Avant.Open.Cloud.Client.Model.Request;
using Avant.Open.Cloud.Client.Model.Response;
using Avant.Open.Cloud.Client.Shim;
using Avant.Open.Cloud.Diagnostic;

namespace Avant.Open.Cloud.Client;

public class OpenCloudClient
{
    /// <summary>
    /// Open Cloud API key.
    /// </summary>
    public readonly string OpenCloudApiKey;
    
    /// <summary>
    /// HTTP client for sending requests.
    /// </summary>
    public readonly IHttpClientShim HttpClient;

    /// <summary>
    /// Creates an Open Cloud client.
    /// </summary>
    /// <param name="openCloudApiKey">Open Cloud API key.</param>
    /// <param name="httpClient">Optional HTTP client. Used for tests.</param>
    public OpenCloudClient(string openCloudApiKey, IHttpClientShim? httpClient = null)
    {
        this.OpenCloudApiKey = openCloudApiKey;
        this.HttpClient = httpClient ?? new HttpClientShim();
    }
    
    /// <summary>
    /// Publishes a place to Roblox.
    /// </summary>
    /// <param name="universeId">Universe id containing the place.</param>
    /// <param name="placeId">Place id of the place.</param>
    /// <param name="placeFilePath">Path of the place file.</param>
    /// <returns>Response for the publish.</returns>
    /// <exception cref="HttpRequestException">If a non-success HTTP response was returned.</exception>
    public async Task<PublishResponse> PublishPlaceAsync(long universeId, long placeId, string placeFilePath)
    {
        // Send the request.
        Logger.Debug($"Publishing to universe {universeId} with place {placeId} using file: {placeFilePath}");
        var response = await this.HttpClient.SendAsync(new HttpRequestMessage()
        {
            RequestUri = new Uri($"https://apis.roblox.com/universes/v1/{universeId}/places/{placeId}/versions?versionType=Saved"),
            Headers =
            {
                {"x-api-key", this.OpenCloudApiKey},
            },
            Method = HttpMethod.Post,
            Content = new ByteArrayContent(await File.ReadAllBytesAsync(placeFilePath)),
        });
        
        // Parse the response.
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Got HTTP {response.StatusCode} for publishing place: {content}");
        }
        
        var responseContent = JsonSerializer.Deserialize<PublishResponse>(content, PublishResponseJsonContext.Default.PublishResponse)!;
        Logger.Debug($"Publish successful with version number {responseContent.VersionNumber}.");
        return responseContent;
    }
    
    /// <summary>
    /// Starts an Open Cloud Luau execution.
    /// </summary>
    /// <param name="universeId">Universe id containing the place.</param>
    /// <param name="placeId">Place id of the place.</param>
    /// <param name="versionNumber">Version number of the place.</param>
    /// <param name="scriptBody">Body of the script.</param>
    /// <returns>Response for the execution task.</returns>
    /// <exception cref="HttpRequestException">If a non-success HTTP response was returned.</exception>
    public async Task<ExecutionTaskResponse> StartExecutionTaskAsync(long universeId, long placeId, long versionNumber, string scriptBody)
    {
        // Send the request.
        Logger.Debug($"Starting Luau execution with universe {universeId} with place {placeId} with version {versionNumber}.");
        var response = await this.HttpClient.SendAsync(new HttpRequestMessage()
        {
            RequestUri = new Uri($"https://apis.roblox.com/cloud/v2/universes/{universeId}/places/{placeId}/versions/{versionNumber}/luau-execution-session-tasks"),
            Headers =
            {
                {"x-api-key", this.OpenCloudApiKey},
            },
            Method = HttpMethod.Post,
            Content = JsonContent.Create(new StartExecutionTaskRequest()
            {
                Script = scriptBody,
            }, StartExecutionTaskRequestJsonContext.Default.StartExecutionTaskRequest),
        });
        
        // Parse the response.
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Got HTTP {response.StatusCode} for starting execution task: {content}");
        }
        
        var responseContent = JsonSerializer.Deserialize<ExecutionTaskResponse>(content, ExecutionTaskResponseJsonContext.Default.ExecutionTaskResponse)!;
        Logger.Debug($"Started execution {responseContent.Path} with state {responseContent.State}.");
        return responseContent;
    }
    
    /// <summary>
    /// Gets the status an Open Cloud Luau execution.
    /// </summary>
    /// <param name="path">Path of the resource for the execution.</param>
    /// <returns>Response for the execution task.</returns>
    /// <exception cref="HttpRequestException">If a non-success HTTP response was returned.</exception>
    public async Task<ExecutionTaskResponse> GetExecutionTaskStateAsync(string path)
    {
        // Send the request.
        var response = await this.HttpClient.SendAsync(new HttpRequestMessage()
        {
            RequestUri = new Uri($"https://apis.roblox.com/cloud/v2/{path}"),
            Headers =
            {
                {"x-api-key", this.OpenCloudApiKey},
            },
            Method = HttpMethod.Get,
        });
        
        // Parse the response.
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Got HTTP {response.StatusCode} for getting execution task: {content}");
        }
        return JsonSerializer.Deserialize<ExecutionTaskResponse>(content, ExecutionTaskResponseJsonContext.Default.ExecutionTaskResponse)!;
    }

    /// <summary>
    /// Gets the logs an Open Cloud Luau execution.
    /// </summary>
    /// <param name="path">Path of the resource for the execution.</param>
    /// <returns>Response for the execution task logs.</returns>
    /// <exception cref="HttpRequestException">If a non-success HTTP response was returned.</exception>
    public async Task<ExecutionTaskLogsResponse> GetExecutionTaskLogsAsync(string path)
    {
        // Send the request.
        // Pages of logs aren't supported yet since it doesn't seem to be implemented on Roblox's side.
        var response = await this.HttpClient.SendAsync(new HttpRequestMessage()
        {
            RequestUri = new Uri($"https://apis.roblox.com/cloud/v2/{path}/logs?view=STRUCTURED"),
            Headers =
            {
                {"x-api-key", this.OpenCloudApiKey},
            },
            Method = HttpMethod.Get,
        });
        
        // Parse the response.
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Got HTTP {response.StatusCode} for getting execution task logs: {content}");
        }
        return JsonSerializer.Deserialize<ExecutionTaskLogsResponse>(content, ExecutionTaskLogsResponseJsonContext.Default.ExecutionTaskLogsResponse)!;
    }
}