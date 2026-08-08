using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using OpenAPU.Application.Apus;
using OpenAPU.Application.Budgets;
using OpenAPU.Application.Concepts;
using OpenAPU.Application.Resources;

namespace OpenAPU.Api.Tests;

public sealed class ApiWorkflowTests : IClassFixture<OpenApuApiFactory>
{
    private readonly HttpClient _client;

    public ApiWorkflowTests(OpenApuApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Complete_workflow_is_available_over_http()
    {
        var resourceResponse = await _client.PostAsJsonAsync(
            "/resources",
            new CreateResourceCommand(
                "MAT-API",
                "Cemento API",
                ResourceTypeDto.Material,
                "KG",
                "kg",
                "Kilogramo",
                4m));

        var resource = await resourceResponse.Content
            .ReadFromJsonAsync<CreateResourceResult>();

        var apuResponse = await _client.PostAsJsonAsync(
            "/apus",
            new CreateApuCommand(
                "APU-API",
                "Muro API",
                "M2",
                "m²",
                "Metro cuadrado"));

        var apu = await apuResponse.Content.ReadFromJsonAsync<ApuResult>();

        await _client.PostAsJsonAsync(
            $"/apus/{apu!.Id}/components",
            new { resourceId = resource!.Id, quantity = 25m });

        var conceptResponse = await _client.PostAsJsonAsync(
            "/concepts",
            new CreateConceptCommand(
                "CON-API",
                "Concepto API",
                "M2",
                "m²",
                "Metro cuadrado",
                apu.Id));

        var concept = await conceptResponse.Content
            .ReadFromJsonAsync<ConceptResult>();

        await _client.PutAsJsonAsync(
            $"/concepts/{concept!.Id}/percentages",
            new
            {
                indirectCost = 10m,
                financing = 3m,
                profit = 12m,
                additionalCharges = 2m
            });

        var budgetResponse = await _client.PostAsJsonAsync(
            "/budgets",
            new CreateBudgetCommand(
                "PRE-API",
                "Presupuesto API"));

        var budget = await budgetResponse.Content
            .ReadFromJsonAsync<BudgetResult>();

        var itemResponse = await _client.PostAsJsonAsync(
            $"/budgets/{budget!.Id}/items",
            new { conceptId = concept.Id, quantity = 10m });

        var updated = await itemResponse.Content
            .ReadFromJsonAsync<BudgetResult>();

        Assert.Equal(HttpStatusCode.OK, itemResponse.StatusCode);
        Assert.Equal(1270m, updated!.Total);

        var detail = await _client.GetFromJsonAsync<BudgetResult>(
            $"/budgets/{budget.Id}");

        Assert.Equal(1270m, detail!.Total);
    }
}

