using System.Net;

namespace OpenAPU.Api.Tests;

public sealed class SecurityHeadersTests :
    IClassFixture<OpenApuApiFactory>
{
    private readonly HttpClient _client;

    public SecurityHeadersTests(OpenApuApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Responses_include_defensive_headers()
    {
        var response = await _client.GetAsync("/");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.Equal(
            "nosniff",
            response.Headers.GetValues(
                "X-Content-Type-Options").Single());

        Assert.Equal(
            "DENY",
            response.Headers.GetValues(
                "X-Frame-Options").Single());

        Assert.Equal(
            "no-referrer",
            response.Headers.GetValues(
                "Referrer-Policy").Single());

        var policy = response.Headers.GetValues(
            "Content-Security-Policy").Single();

        Assert.Contains("frame-ancestors 'none'", policy);
        Assert.Contains("object-src 'none'", policy);
    }
}

