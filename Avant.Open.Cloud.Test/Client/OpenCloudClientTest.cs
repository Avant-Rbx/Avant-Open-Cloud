using System;
using System.IO;
using System.Net;
using Avant.Open.Cloud.Client;
using Avant.Open.Cloud.Client.Model.Response;
using Avant.Open.Cloud.Test.Client.Shim;

namespace Avant.Open.Cloud.Test.Client;

public class OpenCloudClientTest
{
    private TestHttpClientShim _clientShim;
    private OpenCloudClient _client;

    [SetUp]
    public void SetUp()
    {
        this._clientShim = new TestHttpClientShim();
        this._client = new OpenCloudClient("TestKey", _clientShim);
    }

    [Test]
    public void TestPublishPlaceAsync()
    {
        this._clientShim.AddRequest($"https://apis.roblox.com/universes/v1/123/places/456/versions?versionType=Saved", HttpStatusCode.OK, "{\"versionNumber\":789}");

        var response = this._client.PublishPlaceAsync(123L, 456L, Path.GetTempFileName()).Result;
        Assert.That(response.VersionNumber, Is.EqualTo(789));
    }

    [Test]
    public void TestPublishPlaceAsyncError()
    {
        this._clientShim.AddRequest($"https://apis.roblox.com/universes/v1/123/places/456/versions?versionType=Saved", HttpStatusCode.Unauthorized, "{}");

        var exception = Assert.Throws<AggregateException>(() =>
        {
            this._client.PublishPlaceAsync(123L, 456L, Path.GetTempFileName()).Wait();
        });
        Assert.That(exception.InnerException!.Message, Is.EqualTo("Got HTTP Unauthorized for publishing place: {}"));
    }

    [Test]
    public void TestStartExecutionTaskAsync()
    {
        this._clientShim.AddRequest($"https://apis.roblox.com/cloud/v2/universes/123/places/456/versions/789/luau-execution-session-tasks", HttpStatusCode.OK, "{\"path\":\"TestPath\",\"state\":\"PROCESSING\"}");

        var response = this._client.StartExecutionTaskAsync(123L, 456L, 789L, "TestScript").Result;
        Assert.That(response.Path, Is.EqualTo("TestPath"));
        Assert.That(response.State, Is.EqualTo(ExecutionState.Processing));
    }

    [Test]
    public void TestStartExecutionTaskAsyncError()
    {
        this._clientShim.AddRequest($"https://apis.roblox.com/cloud/v2/universes/123/places/456/versions/789/luau-execution-session-tasks", HttpStatusCode.Unauthorized, "{}");

        var exception = Assert.Throws<AggregateException>(() =>
        {
            this._client.StartExecutionTaskAsync(123L, 456L, 789L, "TestScript").Wait();
        });
        Assert.That(exception.InnerException!.Message, Is.EqualTo("Got HTTP Unauthorized for starting execution task: {}"));
    }

    [Test]
    public void TestGetExecutionTaskStateAsync()
    {
        this._clientShim.AddRequest($"https://apis.roblox.com/cloud/v2/TestPath", HttpStatusCode.OK, "{\"path\":\"TestPath\",\"state\":\"PROCESSING\"}");

        var response = this._client.GetExecutionTaskStateAsync("TestPath").Result;
        Assert.That(response.Path, Is.EqualTo("TestPath"));
        Assert.That(response.State, Is.EqualTo(ExecutionState.Processing));
    }

    [Test]
    public void TestGetExecutionTaskStateAsyncError()
    {
        this._clientShim.AddRequest($"https://apis.roblox.com/cloud/v2/TestPath", HttpStatusCode.Unauthorized, "{}");

        var exception = Assert.Throws<AggregateException>(() =>
        {
            this._client.GetExecutionTaskStateAsync("TestPath").Wait();
        });
        Assert.That(exception.InnerException!.Message, Is.EqualTo("Got HTTP Unauthorized for getting execution task: {}"));
    }

    [Test]
    public void TestGetExecutionTaskLogsAsync()
    {
        this._clientShim.AddRequest($"https://apis.roblox.com/cloud/v2/TestPath/logs?view=STRUCTURED", HttpStatusCode.OK, "{\"luauExecutionSessionTaskLogs\":[{}]}");

        var response = this._client.GetExecutionTaskLogsAsync("TestPath").Result;
        Assert.That(response.LuauExecutionSessionTaskLogs.Count, Is.EqualTo(1));
    }

    [Test]
    public void TestGetExecutionTaskLogsAsyncError()
    {
        this._clientShim.AddRequest($"https://apis.roblox.com/cloud/v2/TestPath/logs?view=STRUCTURED", HttpStatusCode.Unauthorized, "{}");

        var exception = Assert.Throws<AggregateException>(() =>
        {
            this._client.GetExecutionTaskLogsAsync("TestPath").Wait();
        });
        Assert.That(exception.InnerException!.Message, Is.EqualTo("Got HTTP Unauthorized for getting execution task logs: {}"));
    }
}