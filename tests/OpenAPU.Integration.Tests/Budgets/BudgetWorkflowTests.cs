using OpenAPU.Application.Apus;
using OpenAPU.Application.Budgets;
using OpenAPU.Application.Concepts;
using OpenAPU.Application.Resources;
using OpenAPU.Infrastructure.Repositories;

namespace OpenAPU.Integration.Tests.Budgets;

public sealed class BudgetWorkflowTests
{
    [Fact]
    public async Task Creates_budget_and_calculates_total()
    {
        var resourceRepository = new InMemoryResourceRepository();
        var apuRepository = new InMemoryApuRepository();
        var conceptRepository = new InMemoryConceptRepository();
        var budgetRepository = new InMemoryBudgetRepository();

        var resource = await new CreateResourceHandler(resourceRepository)
            .HandleAsync(
                new CreateResourceCommand(
                    "MAT-001",
                    "Cemento",
                    ResourceTypeDto.Material,
                    "KG",
                    "kg",
                    "Kilogramo",
                    4m));

        var apu = await new CreateApuHandler(apuRepository)
            .HandleAsync(
                new CreateApuCommand(
                    "APU-001",
                    "Muro",
                    "M2",
                    "m²",
                    "Metro cuadrado"));

        await new AddApuComponentHandler(
                apuRepository,
                resourceRepository)
            .HandleAsync(
                new AddApuComponentCommand(
                    apu.Id,
                    resource.Id,
                    25m));

        var concept = await new CreateConceptHandler(
                conceptRepository,
                apuRepository)
            .HandleAsync(
                new CreateConceptCommand(
                    "CON-001",
                    "Muro",
                    "M2",
                    "m²",
                    "Metro cuadrado",
                    apu.Id));

        concept = await new UpdateConceptPercentagesHandler(
                conceptRepository)
            .HandleAsync(
                new UpdateConceptPercentagesCommand(
                    concept.Id,
                    10m,
                    3m,
                    12m,
                    2m));

        var budget = await new CreateBudgetHandler(budgetRepository)
            .HandleAsync(
                new CreateBudgetCommand(
                    "PRE-001",
                    "Presupuesto de obra"));

        var updated = await new AddBudgetItemHandler(
                budgetRepository,
                conceptRepository)
            .HandleAsync(
                new AddBudgetItemCommand(
                    budget.Id,
                    concept.Id,
                    10m));

        Assert.Equal(1270m, updated.Total);
        Assert.Single(updated.Items);
    }
}
