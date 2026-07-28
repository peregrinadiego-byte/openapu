using OpenAPU.Application.Apus;
using OpenAPU.Application.Concepts;
using OpenAPU.Application.Tests.Apus;
using OpenAPU.Domain;

namespace OpenAPU.Application.Tests.Concepts;

public sealed class CreateConceptHandlerTests
{
    [Fact]
    public async Task Creates_concept_from_existing_apu()
    {
        var apuRepository = new FakeApuRepository();
        var conceptRepository = new FakeConceptRepository();

        var apu = CreateApuWithDirectCost(200m);
        apuRepository.Apus.Add(apu);

        var handler = new CreateConceptHandler(
            conceptRepository,
            apuRepository);

        var result = await handler.HandleAsync(
            new CreateConceptCommand(
                "CON-001",
                "Muro de block",
                "M2",
                "m²",
                "Metro cuadrado",
                apu.Id.Value));

        Assert.Equal("CON-001", result.Key);
        Assert.Equal(200m, result.DirectCost);
        Assert.Equal(200m, result.UnitPrice);
        Assert.Single(conceptRepository.Concepts);
    }

    [Fact]
    public async Task Rejects_unknown_apu()
    {
        var handler = new CreateConceptHandler(
            new FakeConceptRepository(),
            new FakeApuRepository());

        await Assert.ThrowsAsync<OpenAPU.Application.ApplicationException>(
            () => handler.HandleAsync(
                new CreateConceptCommand(
                    "CON-001",
                    "Muro",
                    "M2",
                    "m²",
                    "Metro cuadrado",
                    Guid.NewGuid())));
    }

    [Fact]
    public async Task Rejects_duplicate_key()
    {
        var apuRepository = new FakeApuRepository();
        var conceptRepository = new FakeConceptRepository();

        var apu = CreateApuWithDirectCost(100m);
        apuRepository.Apus.Add(apu);

        var handler = new CreateConceptHandler(
            conceptRepository,
            apuRepository);

        var command = new CreateConceptCommand(
            "CON-001",
            "Muro",
            "M2",
            "m²",
            "Metro cuadrado",
            apu.Id.Value);

        await handler.HandleAsync(command);

        await Assert.ThrowsAsync<OpenAPU.Application.ApplicationException>(
            () => handler.HandleAsync(command));
    }

    internal static Apu CreateApuWithDirectCost(decimal directCost)
    {
        var resource = Resource.Create(
            Key.From("MAT-001"),
            "Material",
            ResourceType.Material,
            Unit.Create("PZA", "pza", "Pieza"),
            Money.From(directCost));

        var apu = Apu.Create(
            Key.From("APU-001"),
            "APU de prueba",
            Unit.Create("M2", "m²", "Metro cuadrado"));

        apu.AddComponent(resource, Quantity.From(1m));
        return apu;
    }
}
