using System.Net;

namespace OpenAPU.Api.Tests;

public sealed class DeploymentReadinessTests :
    IClassFixture<OpenApuApiFactory>
{
    private readonly HttpClient _client;

    public DeploymentReadinessTests(OpenApuApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_and_system_status_are_available()
    {
        var health = await _client.GetAsync("/health");
        var status = await _client.GetAsync("/system/status");

        Assert.Equal(HttpStatusCode.OK, health.StatusCode);
        Assert.Equal(HttpStatusCode.OK, status.StatusCode);
    }
}

