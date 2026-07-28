using OpenAPU.Application.Apus;
using OpenAPU.Application.Budgets;
using OpenAPU.Application.Concepts;
using OpenAPU.Application.Resources;
using OpenAPU.Infrastructure.Repositories;

namespace OpenAPU.Integration.Tests.Budgets;

public sealed class SqliteBudgetRepositoryTests : IDisposable
{
    private readonly string _databasePath =
        Path.Combine(
            Path.GetTempPath(),
            $"openapu-budget-{Guid.NewGuid():N}.db");

    [Fact]
    public async Task Persists_budget_and_items_between_instances()
    {
        var setup = await CreateFullWorkflowAsync();

        var secondRepository =
            new SqliteBudgetRepository(_databasePath);

        var stored = await secondRepository.GetByIdAsync(
            OpenAPU.Domain.Identifier.From(setup.BudgetId));

        Assert.NotNull(stored);
        Assert.Equal(setup.BudgetId, stored.Id.Value);
        Assert.Single(stored.Items);
        Assert.Equal(1270m, stored.Total.Amount);
    }

    [Fact]
    public async Task Budget_item_identity_is_preserved()
    {
        var setup = await CreateFullWorkflowAsync();

        var firstRepository =
            new SqliteBudgetRepository(_databasePath);

        var first = await firstRepository.GetByIdAsync(
            OpenAPU.Domain.Identifier.From(setup.BudgetId));

        var itemId = Assert.Single(first!.Items).Id;

        var secondRepository =
            new SqliteBudgetRepository(_databasePath);

        var second = await secondRepository.GetByIdAsync(
            OpenAPU.Domain.Identifier.From(setup.BudgetId));

        Assert.Equal(
            itemId,
            Assert.Single(second!.Items).Id);
    }

    [Fact]
    public async Task Duplicate_concept_is_rejected_after_reload()
    {
        var setup = await CreateFullWorkflowAsync();

        var budgetRepository =
            new SqliteBudgetRepository(_databasePath);

        var conceptRepository =
            new SqliteConceptRepository(_databasePath);

        var handler = new AddBudgetItemHandler(
            budgetRepository,
            conceptRepository);

        await Assert.ThrowsAsync<OpenAPU.Domain.DomainException>(
            () => handler.HandleAsync(
                new AddBudgetItemCommand(
                    setup.BudgetId,
                    setup.ConceptId,
                    1m)));
    }

    private async Task<SetupResult> CreateFullWorkflowAsync()
    {
        var resourceRepository =
            new SqliteResourceRepository(_databasePath);

        var apuRepository =
            new SqliteApuRepository(_databasePath);

        var conceptRepository =
            new SqliteConceptRepository(_databasePath);

        var budgetRepository =
            new SqliteBudgetRepository(_databasePath);

        var resource = await new CreateResourceHandler(resourceRepository)
            .HandleAsync(new CreateResourceCommand(
                "MAT-001",
                "Cemento",
                ResourceTypeDto.Material,
                "KG",
                "kg",
                "Kilogramo",
                4m));

        var apu = await new CreateApuHandler(apuRepository)
            .HandleAsync(new CreateApuCommand(
                "APU-001",
                "Muro",
                "M2",
                "m²",
                "Metro cuadrado"));

        await new AddApuComponentHandler(
                apuRepository,
                resourceRepository)
            .HandleAsync(new AddApuComponentCommand(
                apu.Id,
                resource.Id,
                25m));

        var concept = await new CreateConceptHandler(
                conceptRepository,
                apuRepository)
            .HandleAsync(new CreateConceptCommand(
                "CON-001",
                "Muro",
                "M2",
                "m²",
                "Metro cuadrado",
                apu.Id));

        await new UpdateConceptPercentagesHandler(conceptRepository)
            .HandleAsync(new UpdateConceptPercentagesCommand(
                concept.Id,
                10m,
                3m,
                12m,
                2m));

        var budget = await new CreateBudgetHandler(budgetRepository)
            .HandleAsync(new CreateBudgetCommand(
                "PRE-001",
                "Presupuesto"));

        await new AddBudgetItemHandler(
                budgetRepository,
                conceptRepository)
            .HandleAsync(new AddBudgetItemCommand(
                budget.Id,
                concept.Id,
                10m));

        return new SetupResult(
            budget.Id,
            concept.Id);
    }

    public void Dispose()
    {
        if (File.Exists(_databasePath))
        {
            File.Delete(_databasePath);
        }
    }

    private sealed record SetupResult(
        Guid BudgetId,
        Guid ConceptId);
}
