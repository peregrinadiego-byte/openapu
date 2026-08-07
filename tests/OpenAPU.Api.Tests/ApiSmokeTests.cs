using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using OpenAPU.Application.Resources;

namespace OpenAPU.Api.Tests;

public sealed class ApiSmokeTests : IClassFixture<OpenApuApiFactory>
{
    private readonly HttpClient _client;

    public ApiSmokeTests(OpenApuApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_endpoint_returns_ok()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Creates_and_lists_resource()
    {
        var createResponse = await _client.PostAsJsonAsync(
            "/resources",
            new CreateResourceCommand(
                "MAT-001",
                "Cemento",
                ResourceTypeDto.Material,
                "KG",
                "kg",
                "Kilogramo",
                4.50m));

        Assert.Equal(
            HttpStatusCode.Created,
            createResponse.StatusCode);

        var resources = await _client.GetFromJsonAsync<
            ResourceListItem[]>("/resources");

        var resource = Assert.Single(resources!);

        Assert.Equal("MAT-001", resource.Key);
        Assert.Equal(4.50m, resource.Price);
    }
}

public sealed class OpenApuApiFactory :
    WebApplicationFactory<Program>,
    IDisposable
{
    private readonly string _databasePath =
        Path.Combine(
            Path.GetTempPath(),
            $"openapu-api-{Guid.NewGuid():N}.db");

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseSetting(
            "ConnectionStrings:OpenAPU",
            $"Data Source={_databasePath};Pooling=False");

        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["ConnectionStrings:OpenAPU"] =
                        $"Data Source={_databasePath};Pooling=False"
                });
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (File.Exists(_databasePath))
        {
            File.Delete(_databasePath);
        }
    }
}
