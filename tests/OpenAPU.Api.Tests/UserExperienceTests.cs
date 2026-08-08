using System.Net;

namespace OpenAPU.Api.Tests;

public sealed class UserExperienceTests :
    IClassFixture<OpenApuApiFactory>
{
    private readonly HttpClient _client;

    public UserExperienceTests(OpenApuApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Interface_contains_summary_and_validation_layer()
    {
        var htmlResponse = await _client.GetAsync(
            "/index.html");

        var scriptResponse = await _client.GetAsync(
            "/ux.js");

        var html = await htmlResponse.Content
            .ReadAsStringAsync();

        var script = await scriptResponse.Content
            .ReadAsStringAsync();

        Assert.Equal(
            HttpStatusCode.OK,
            htmlResponse.StatusCode);

        Assert.Equal(
            HttpStatusCode.OK,
            scriptResponse.StatusCode);

        Assert.Contains("summary-resources", html);
        Assert.Contains("summary-total", html);
        Assert.Contains("global-message", html);
        Assert.Contains("validateForm", script);
        Assert.Contains("openapu.activeView", script);
    }
}

