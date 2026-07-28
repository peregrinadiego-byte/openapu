using OpenAPU.Application.Concepts;
using OpenAPU.Domain;

namespace OpenAPU.Application.Tests.Concepts;

public sealed class UpdateConceptPercentagesHandlerTests
{
    [Fact]
    public async Task Applies_percentages_and_calculates_unit_price()
    {
        var repository = new FakeConceptRepository();
        var apu = CreateConceptHandlerTests.CreateApuWithDirectCost(100m);

        var concept = Concept.Create(
            Key.From("CON-001"),
            "Muro",
            Unit.Create("M2", "m²", "Metro cuadrado"),
            apu);

        repository.Concepts.Add(concept);

        var result = await new UpdateConceptPercentagesHandler(repository)
            .HandleAsync(
                new UpdateConceptPercentagesCommand(
                    concept.Id.Value,
                    10m,
                    3m,
                    12m,
                    2m));

        Assert.Equal(10m, result.IndirectCost);
        Assert.Equal(3m, result.Financing);
        Assert.Equal(12m, result.Profit);
        Assert.Equal(2m, result.AdditionalCharges);
        Assert.Equal(127m, result.UnitPrice);
    }

    [Fact]
    public async Task Rejects_invalid_percentage()
    {
        var repository = new FakeConceptRepository();
        var apu = CreateConceptHandlerTests.CreateApuWithDirectCost(100m);

        var concept = Concept.Create(
            Key.From("CON-001"),
            "Muro",
            Unit.Create("M2", "m²", "Metro cuadrado"),
            apu);

        repository.Concepts.Add(concept);

        await Assert.ThrowsAsync<DomainException>(
            () => new UpdateConceptPercentagesHandler(repository)
                .HandleAsync(
                    new UpdateConceptPercentagesCommand(
                        concept.Id.Value,
                        101m,
                        0m,
                        0m,
                        0m)));
    }

    [Fact]
    public async Task Rejects_unknown_concept()
    {
        var handler = new UpdateConceptPercentagesHandler(
            new FakeConceptRepository());

        await Assert.ThrowsAsync<OpenAPU.Application.ApplicationException>(
            () => handler.HandleAsync(
                new UpdateConceptPercentagesCommand(
                    Guid.NewGuid(),
                    10m,
                    3m,
                    12m,
                    2m)));
    }
}
