using System.Net;

namespace OpenAPU.Api.Tests;

public sealed class ObservabilityTests :
    IClassFixture<OpenApuApiFactory>
{
    private readonly HttpClient _client;

    public ObservabilityTests(OpenApuApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Response_contains_generated_correlation_id()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.True(
            response.Headers.Contains("X-Correlation-ID"));

        var value = response.Headers
            .GetValues("X-Correlation-ID")
            .Single();

        Assert.False(string.IsNullOrWhiteSpace(value));
    }

    [Fact]
    public async Task Supplied_correlation_id_is_preserved()
    {
        const string correlationId =
            "openapu-test-correlation";

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "/health");

        request.Headers.Add(
            "X-Correlation-ID",
            correlationId);

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.Equal(
            correlationId,
            response.Headers
                .GetValues("X-Correlation-ID")
                .Single());
    }
}
