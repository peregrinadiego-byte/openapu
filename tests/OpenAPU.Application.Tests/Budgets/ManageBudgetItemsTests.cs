using OpenAPU.Application.Budgets;
using OpenAPU.Application.Tests.Budgets;
using OpenAPU.Application.Tests.Concepts;
using OpenAPU.Domain;

namespace OpenAPU.Application.Tests.Budgets;

public sealed class ManageBudgetItemsTests
{
    [Fact]
    public async Task Changes_item_quantity()
    {
        var repository = new FakeBudgetRepository();
        var budget = CreateBudget();
        repository.Budgets.Add(budget);

        var item = Assert.Single(budget.Items);

        var result = await new ChangeBudgetItemQuantityHandler(repository)
            .HandleAsync(new ChangeBudgetItemQuantityCommand(
                budget.Id.Value,
                item.Id.Value,
                5m));

        Assert.Equal(500m, result.Total);
    }

    [Fact]
    public async Task Removes_item()
    {
        var repository = new FakeBudgetRepository();
        var budget = CreateBudget();
        repository.Budgets.Add(budget);

        var item = Assert.Single(budget.Items);

        var result = await new RemoveBudgetItemHandler(repository)
            .HandleAsync(new RemoveBudgetItemCommand(
                budget.Id.Value,
                item.Id.Value));

        Assert.Empty(result.Items);
        Assert.Equal(0m, result.Total);
    }

    private static Budget CreateBudget()
    {
        var resource = Resource.Create(
            Key.From("MAT-001"),
            "Material",
            ResourceType.Material,
            Unit.Create("PZA", "pza", "Pieza"),
            Money.From(100m));

        var apu = Apu.Create(
            Key.From("APU-001"),
            "APU",
            Unit.Create("M2", "m²", "Metro cuadrado"));

        apu.AddComponent(resource, Quantity.From(1m));

        var concept = Concept.Create(
            Key.From("CON-001"),
            "Concepto",
            Unit.Create("M2", "m²", "Metro cuadrado"),
            apu);

        var budget = Budget.Create(
            Key.From("PRE-001"),
            "Presupuesto");

        budget.AddItem(concept, Quantity.From(2m));

        return budget;
    }
}
