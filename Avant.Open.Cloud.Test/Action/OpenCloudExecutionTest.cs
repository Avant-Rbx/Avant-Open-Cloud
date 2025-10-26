using System.IO;
using System.Net;
using Avant.Open.Cloud.Action;
using Avant.Open.Cloud.Configuration;
using Avant.Open.Cloud.Test.Client.Shim;

namespace Avant.Open.Cloud.Test.Action;

public class OpenCloudExecutionTest
{
    private TestHttpClientShim _clientShim;
    private OpenCloudExecution _openCloudExecution;

    [SetUp]
    public void SetUp()
    {
        this._clientShim = new TestHttpClientShim();
        this._openCloudExecution = new OpenCloudExecution("TestKey", new OpenCloudConfiguration()
        {
            UniverseId = 123,
            PlaceId = 456,
        }, this._clientShim);
    }

    [Test]
    public void TestRunOpenCloudExecutionAsync()
    {
        this._clientShim.AddRequest($"https://apis.roblox.com/universes/v1/123/places/456/versions?versionType=Saved", HttpStatusCode.OK, "{\"versionNumber\":789}");
        this._clientShim.AddRequest($"https://apis.roblox.com/cloud/v2/universes/123/places/456/versions/789/luau-execution-session-tasks", HttpStatusCode.OK, "{\"path\":\"TestPath\",\"state\":\"PROCESSING\"}");
        this._clientShim.AddRequest($"https://apis.roblox.com/cloud/v2/TestPath", HttpStatusCode.OK, "{\"path\":\"TestPath\",\"state\":\"COMPLETE\"}");
        this._clientShim.AddRequest($"https://apis.roblox.com/cloud/v2/TestPath/logs?view=STRUCTURED", HttpStatusCode.OK, "{\"luauExecutionSessionTaskLogs\":[{\"structuredMessages\":[]}]}");

        Assert.That(this._openCloudExecution.RunOpenCloudExecutionAsync(Path.GetTempFileName(), "_TestSuffix").Result, Is.True);
    }

    [Test]
    public void TestRunOpenCloudExecutionAsyncFailed()
    {
        this._clientShim.AddRequest($"https://apis.roblox.com/universes/v1/123/places/456/versions?versionType=Saved", HttpStatusCode.OK, "{\"versionNumber\":789}");
        this._clientShim.AddRequest($"https://apis.roblox.com/cloud/v2/universes/123/places/456/versions/789/luau-execution-session-tasks", HttpStatusCode.OK, "{\"path\":\"TestPath\",\"state\":\"PROCESSING\"}");
        this._clientShim.AddRequest($"https://apis.roblox.com/cloud/v2/TestPath", HttpStatusCode.OK, "{\"path\":\"TestPath\",\"state\":\"FAILED\"}");
        this._clientShim.AddRequest($"https://apis.roblox.com/cloud/v2/TestPath/logs?view=STRUCTURED", HttpStatusCode.OK, "{\"luauExecutionSessionTaskLogs\":[{}]}");

        Assert.That(this._openCloudExecution.RunOpenCloudExecutionAsync(Path.GetTempFileName(), "_TestSuffix").Result, Is.False);
    }

    [Test]
    public void TestRunOpenCloudExecutionAsyncError()
    {
        this._clientShim.AddRequest($"https://apis.roblox.com/universes/v1/123/places/456/versions?versionType=Saved", HttpStatusCode.Unauthorized, "{}");
        
        Assert.That(this._openCloudExecution.RunOpenCloudExecutionAsync(Path.GetTempFileName(), "_TestSuffix").Result, Is.False);
    }
}