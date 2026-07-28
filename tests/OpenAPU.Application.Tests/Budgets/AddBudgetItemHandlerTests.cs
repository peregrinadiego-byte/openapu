using OpenAPU.Application.Budgets;
using OpenAPU.Application.Tests.Concepts;
using OpenAPU.Domain;

namespace OpenAPU.Application.Tests.Budgets;

public sealed class AddBudgetItemHandlerTests
{
    [Fact]
    public async Task Adds_concept_and_calculates_total()
    {
        var budgetRepository = new FakeBudgetRepository();
        var conceptRepository = new FakeConceptRepository();

        var concept = CreateConcept(127m);
        var budget = Budget.Create(
            Key.From("PRE-001"),
            "Presupuesto");

        budgetRepository.Budgets.Add(budget);
        conceptRepository.Concepts.Add(concept);

        var result = await new AddBudgetItemHandler(
                budgetRepository,
                conceptRepository)
            .HandleAsync(
                new AddBudgetItemCommand(
                    budget.Id.Value,
                    concept.Id.Value,
                    10m));

        Assert.Equal(1270m, result.Total);
        Assert.Single(result.Items);
    }

    [Fact]
    public async Task Rejects_unknown_budget()
    {
        var handler = new AddBudgetItemHandler(
            new FakeBudgetRepository(),
            new FakeConceptRepository());

        await Assert.ThrowsAsync<OpenAPU.Application.ApplicationException>(
            () => handler.HandleAsync(
                new AddBudgetItemCommand(
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    1m)));
    }

    [Fact]
    public async Task Rejects_duplicate_concept()
    {
        var budgetRepository = new FakeBudgetRepository();
        var conceptRepository = new FakeConceptRepository();

        var concept = CreateConcept(100m);
        var budget = Budget.Create(
            Key.From("PRE-001"),
            "Presupuesto");

        budget.AddItem(concept, Quantity.From(1m));

        budgetRepository.Budgets.Add(budget);
        conceptRepository.Concepts.Add(concept);

        var handler = new AddBudgetItemHandler(
            budgetRepository,
            conceptRepository);

        await Assert.ThrowsAsync<DomainException>(
            () => handler.HandleAsync(
                new AddBudgetItemCommand(
                    budget.Id.Value,
                    concept.Id.Value,
                    1m)));
    }

    private static Concept CreateConcept(decimal unitPrice)
    {
        var resource = Resource.Create(
            Key.From("MAT-001"),
            "Material",
            ResourceType.Material,
            Unit.Create("PZA", "pza", "Pieza"),
            Money.From(unitPrice));

        var apu = Apu.Create(
            Key.From("APU-001"),
            "APU",
            Unit.Create("M2", "m²", "Metro cuadrado"));

        apu.AddComponent(resource, Quantity.From(1m));

        return Concept.Create(
            Key.From("CON-001"),
            "Concepto",
            Unit.Create("M2", "m²", "Metro cuadrado"),
            apu);
    }
}
