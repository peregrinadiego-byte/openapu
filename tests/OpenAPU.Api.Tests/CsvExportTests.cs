using System.Net;
using System.Net.Http.Headers;

namespace OpenAPU.Api.Tests;

public sealed class CsvExportTests :
    IClassFixture<OpenApuApiFactory>
{
    private readonly HttpClient _client;

    public CsvExportTests(OpenApuApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Apus_can_be_exported_as_csv()
    {
        var response = await _client.GetAsync("/exports/apus.csv");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/csv", response.Content.Headers.ContentType?.MediaType);
        Assert.Contains("Clave APU", content);
        Assert.Equal(
            new ContentDispositionHeaderValue("attachment")
            {
                FileNameStar = "openapu-apus.csv"
            }.DispositionType,
            response.Content.Headers.ContentDisposition?.DispositionType);
    }

    [Fact]
    public async Task Budgets_can_be_exported_as_csv()
    {
        var response = await _client.GetAsync("/exports/budgets.csv");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/csv", response.Content.Headers.ContentType?.MediaType);
        Assert.Contains("Clave presupuesto", content);
        Assert.Contains("Total presupuesto", content);
    }
}
