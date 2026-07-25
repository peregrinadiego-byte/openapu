using OpenAPU.Domain;

namespace OpenAPU.Domain.Tests;

public class KernelTests
{
    [Fact]
    public void Identifier_rejects_empty_uuid() =>
        Assert.Throws<DomainException>(() => Identifier.From(Guid.Empty));

    [Fact]
    public void Key_normalizes_value() =>
        Assert.Equal("MAT-001", Key.From(" mat-001 ").Value);

    [Fact]
    public void Money_rejects_negative_amount() =>
        Assert.Throws<DomainException>(() => Money.From(-1m));

    [Fact]
    public void Quantity_must_be_positive() =>
        Assert.Throws<DomainException>(() => Quantity.From(0m));

    [Fact]
    public void Percentage_must_be_between_zero_and_one_hundred() =>
        Assert.Throws<DomainException>(() => Percentage.From(101m));

    [Fact]
    public void Unit_equality_uses_code()
    {
        var first = Unit.Create("M2", "m²", "Metro cuadrado");
        var second = Unit.Create("m2", "sqm", "Square metre");

        Assert.Equal(first, second);
    }

    [Fact]
    public void Complete_flow_calculates_reproducible_budget()
    {
        var kg = Unit.Create("KG", "kg", "Kilogramo");
        var hour = Unit.Create("H", "h", "Hora");
        var squareMetre = Unit.Create("M2", "m²", "Metro cuadrado");

        var cement = Resource.Create(
            Key.From("MAT-001"),
            "Cemento",
            ResourceType.Material,
            kg,
            Money.From(4m));

        var labor = Resource.Create(
            Key.From("MO-001"),
            "Oficial albañil",
            ResourceType.Labor,
            hour,
            Money.From(80m));

        var apu = Apu.Create(
            Key.From("APU-001"),
            "Muro de prueba",
            squareMetre);

        apu.AddComponent(cement, Quantity.From(10m));
        apu.AddComponent(labor, Quantity.From(2m));

        Assert.Equal(200m, apu.DirectCost.Amount);

        var concept = Concept.Create(
            Key.From("CON-001"),
            "Muro de prueba",
            squareMetre,
            apu);

        concept.SetIndirectCost(Percentage.From(10m));
        concept.SetProfit(Percentage.From(15m));

        Assert.Equal(250m, concept.UnitPrice.Amount);

        var budget = Budget.Create(
            Key.From("PRE-001"),
            "Presupuesto de prueba");

        budget.AddItem(concept, Quantity.From(12m));

        Assert.Equal(3000m, budget.Total.Amount);
    }

    [Fact]
    public void Apu_rejects_duplicate_resource()
    {
        var unit = Unit.Create("PZA", "pza", "Pieza");

        var resource = Resource.Create(
            Key.From("MAT-001"),
            "Block",
            ResourceType.Material,
            unit,
            Money.From(20m));

        var apu = Apu.Create(
            Key.From("APU-001"),
            "Muro",
            unit);

        apu.AddComponent(resource, Quantity.From(1m));

        Assert.Throws<DomainException>(() =>
            apu.AddComponent(resource, Quantity.From(1m)));
    }

    [Fact]
    public void Refreshing_prices_updates_budget_snapshot()
    {
        var unit = Unit.Create("PZA", "pza", "Pieza");

        var resource = Resource.Create(
            Key.From("MAT-001"),
            "Block",
            ResourceType.Material,
            unit,
            Money.From(10m));

        var apu = Apu.Create(
            Key.From("APU-001"),
            "Muro",
            unit);

        apu.AddComponent(resource, Quantity.From(2m));

        var concept = Concept.Create(
            Key.From("CON-001"),
            "Muro",
            unit,
            apu);

        var budget = Budget.Create(
            Key.From("PRE-001"),
            "Obra");

        budget.AddItem(concept, Quantity.From(3m));

        Assert.Equal(60m, budget.Total.Amount);

        resource.ChangePrice(Money.From(15m));
        apu.RefreshPrices();
        budget.RefreshPrices();

        Assert.Equal(90m, budget.Total.Amount);
    }
}
