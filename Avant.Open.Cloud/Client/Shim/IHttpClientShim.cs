using System.Net.Http;
using System.Threading.Tasks;

namespace Avant.Open.Cloud.Client.Shim;

public interface IHttpClientShim
{
    /// <summary>
    /// Sends an HTTP request.
    /// </summary>
    /// <param name="request">Request to send.</param>
    /// <returns>Response of the request.</returns>
    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
}