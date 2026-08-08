using System.Net;
using System.Net.Http.Json;

namespace OpenAPU.Api.Tests;

public sealed class ReleaseStatusTests :
    IClassFixture<OpenApuApiFactory>
{
    private readonly HttpClient _client;

    public ReleaseStatusTests(OpenApuApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Version_1_1_reports_database_ready()
    {
        var response = await _client.GetAsync(
            "/system/status");

        var status = await response.Content
            .ReadFromJsonAsync<SystemStatus>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(status);
        Assert.Equal("OpenAPU", status.Name);
        Assert.Equal("1.3.0", status.Version);
        Assert.Equal("ready", status.Database);
        Assert.True(status.Resources >= 0);
        Assert.True(status.Apus >= 0);
        Assert.True(status.Concepts >= 0);
        Assert.True(status.Budgets >= 0);
    }

    private sealed record SystemStatus(
        string Name,
        string Version,
        string Database,
        int Resources,
        int Apus,
        int Concepts,
        int Budgets);
}



