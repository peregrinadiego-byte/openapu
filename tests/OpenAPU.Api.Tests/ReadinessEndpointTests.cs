using System.Net;
using System.Net.Http.Json;

namespace OpenAPU.Api.Tests;

public sealed class ReadinessEndpointTests :
    IClassFixture<OpenApuApiFactory>
{
    private readonly HttpClient _client;

    public ReadinessEndpointTests(OpenApuApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Ready_endpoint_reports_writable_database()
    {
        var response = await _client.GetAsync("/ready");

        var status = await response.Content
            .ReadFromJsonAsync<ReadinessStatus>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(status);
        Assert.Equal("OpenAPU", status.Name);
        Assert.Equal("1.4.0", status.Version);
        Assert.True(status.Ready);
        Assert.False(
            string.IsNullOrWhiteSpace(status.DatabasePath));
    }

    private sealed record ReadinessStatus(
        string Name,
        string Version,
        bool Ready,
        string DatabasePath,
        string DatabaseDirectory);
}



