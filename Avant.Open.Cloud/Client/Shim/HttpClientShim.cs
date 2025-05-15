using System.Net.Http;
using System.Threading.Tasks;

namespace Avant.Open.Cloud.Client.Shim;

public class HttpClientShim : IHttpClientShim
{
    /// <summary>
    /// HTTP client to send the request.
    /// </summary>
    private readonly HttpClient _httpClient = new HttpClient();
    
    /// <summary>
    /// Sends an HTTP request.
    /// </summary>
    /// <param name="request">Request to send.</param>
    /// <returns>Response of the request.</returns>
    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
    {
        return await this._httpClient.SendAsync(request);
    }
}