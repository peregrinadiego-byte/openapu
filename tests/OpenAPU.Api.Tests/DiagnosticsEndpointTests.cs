using System.Net;
using System.Net.Http.Json;

namespace OpenAPU.Api.Tests;

public sealed class DiagnosticsEndpointTests :
    IClassFixture<OpenApuApiFactory>
{
    private readonly HttpClient _client;

    public DiagnosticsEndpointTests(OpenApuApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Diagnostics_endpoint_reports_safe_operational_data()
    {
        var response = await _client.GetAsync(
            "/support/diagnostics");

        var diagnostics = await response.Content
            .ReadFromJsonAsync<DiagnosticsResult>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(diagnostics);
        Assert.Equal("OpenAPU", diagnostics.Product);
        Assert.False(string.IsNullOrWhiteSpace(
            diagnostics.Version));
        Assert.True(diagnostics.Database.Ready);
        Assert.True(diagnostics.Counts.Resources >= 0);
        Assert.True(diagnostics.Counts.Apus >= 0);
        Assert.True(diagnostics.Counts.Concepts >= 0);
        Assert.True(diagnostics.Counts.Budgets >= 0);
    }

    [Fact]
    public async Task Diagnostics_download_returns_json_attachment()
    {
        var response = await _client.GetAsync(
            "/support/diagnostics/download");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(
            "application/json",
            response.Content.Headers.ContentType?.MediaType);

        Assert.NotNull(
            response.Content.Headers.ContentDisposition);

        Assert.Contains(
            "openapu-diagnostics-",
            response.Content.Headers.ContentDisposition?
                .FileNameStar ??
            response.Content.Headers.ContentDisposition?
                .FileName);
    }

    private sealed record DiagnosticsResult(
        string Product,
        string Version,
        DatabaseResult Database,
        CountsResult Counts);

    private sealed record DatabaseResult(
        bool Ready,
        string Path,
        string Directory,
        bool Writable);

    private sealed record CountsResult(
        int Resources,
        int Apus,
        int Concepts,
        int Budgets);
}
