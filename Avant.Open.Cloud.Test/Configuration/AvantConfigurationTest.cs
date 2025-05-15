using System.Collections.Generic;
using Avant.Open.Cloud.Configuration;

namespace Avant.Open.Cloud.Test.Configuration;

public class AvantConfigurationTest
{
    [Test]
    public void TestVerifyConfigurationComplete()
    {
        Assert.That(new AvantConfiguration()
        {
            RojoBuildStrategy = new RojoBuildStrategyConfiguration()
            {
                AvantInjectionDirectory = "AvantInjectionDirectory",
                ProjectFile = "ProjectFile",
            },
            OpenCloud = new OpenCloudConfiguration()
            {
                ApiKeyEnvironmentVariable = "ApiKeyEnvironmentVariable",
                UniverseId = 12345,
                PlaceId = 12345,
            },
        }.VerifyConfiguration(), Is.EqualTo(new List<string>()));
    }
    [Test]
    public void TestVerifyConfigurationWithIssues()
    {
        Assert.That(new AvantConfiguration().VerifyConfiguration(), Is.EqualTo(new List<string>()
        {
            "RojoBuildStrategy is not configured.",
            "OpenCloud is not configured.",
        }));
        
        Assert.That(new AvantConfiguration()
        {
            RojoBuildStrategy = new RojoBuildStrategyConfiguration(),
            OpenCloud = new OpenCloudConfiguration(),
        }.VerifyConfiguration(), Is.EqualTo(new List<string>()
        {
            "RojoBuildStrategy.ProjectFile is not configured.",
            "OpenCloud.ApiKeyEnvironmentVariable is not configured.",
            "OpenCloud.UniverseId is not configured.",
            "OpenCloud.PlaceId is not configured.",
        }));
    }
}