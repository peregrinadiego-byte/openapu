using System.Net;
using System.Net.Http.Json;

namespace OpenAPU.Api.Tests;

public sealed class VersionEndpointTests :
    IClassFixture<OpenApuApiFactory>
{
    private readonly HttpClient _client;

    public VersionEndpointTests(OpenApuApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Root_endpoint_identifies_version_one()
    {
        var response = await _client.GetAsync("/");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content
            .ReadFromJsonAsync<VersionResponse>();

        Assert.NotNull(result);
        Assert.Equal("OpenAPU", result.Name);
        Assert.Equal("1.0", result.Version);
        Assert.Equal("ready", result.Status);
    }

    private sealed record VersionResponse(
        string Name,
        string Version,
        string Status);
}
