using System.Net;
using System.Net.Http.Json;
using OpenAPU.Application.Budgets;

namespace OpenAPU.Api.Tests;

public sealed class PrintableReportTests :
    IClassFixture<OpenApuApiFactory>
{
    private readonly HttpClient _client;

    public PrintableReportTests(OpenApuApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Apu_summary_report_is_printable_html()
    {
        var response = await _client.GetAsync(
            "/reports/apus");

        var html = await response.Content
            .ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(
            "text/html",
            response.Content.Headers.ContentType?.MediaType);

        Assert.Contains(
            "Resumen de análisis de precios unitarios",
            html);

        Assert.Contains(
            "window.print()",
            html);
    }

    [Fact]
    public async Task Budget_detail_report_contains_total()
    {
        var createResponse = await _client.PostAsJsonAsync(
            "/budgets",
            new CreateBudgetCommand(
                $"REP-{Guid.NewGuid():N}"[..12],
                "Presupuesto imprimible"));

        var budget = await createResponse.Content
            .ReadFromJsonAsync<BudgetResult>();

        var response = await _client.GetAsync(
            $"/reports/budgets/{budget!.Id}");

        var html = await response.Content
            .ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Presupuesto imprimible", html);
        Assert.Contains("Total", html);
        Assert.Contains("Total de partidas: 0", html);
    }
}

