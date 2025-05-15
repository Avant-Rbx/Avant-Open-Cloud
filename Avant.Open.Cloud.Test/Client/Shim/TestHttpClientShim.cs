using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Avant.Open.Cloud.Client.Shim;

namespace Avant.Open.Cloud.Test.Client.Shim;

public class TestHttpClientShim : IHttpClientShim
{
    /// <summary>
    /// Test responses for the URLs.
    /// </summary>
    private readonly Dictionary<string, HttpResponseMessage> _responses = new Dictionary<string, HttpResponseMessage>();

    /// <summary>
    /// Adds a test request.
    /// </summary>
    /// <param name="url">URL of the request to match.</param>
    /// <param name="statusCode">Status code of the response.</param>
    /// <param name="responseBody">Body of the response.</param>
    public void AddRequest(string url, HttpStatusCode statusCode, string responseBody)
    {
        this._responses[url] = new HttpResponseMessage()
        {
            StatusCode = statusCode,
            Content = new StringContent(responseBody)
        };
    }
    
    /// <summary>
    /// Sends an HTTP request.
    /// </summary>
    /// <param name="request">Request to send.</param>
    /// <returns>Response of the request.</returns>
    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
    {
        var path = request.RequestUri!.ToString();
        if (!this._responses.TryGetValue(path, out var response))
        {
            throw new Exception($"Request path not found: {path}");
        }
        return Task.FromResult(response);
    }
}