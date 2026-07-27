using OpenAPU.Application.Apus;
using OpenAPU.Application.Tests.Resources;
using OpenAPU.Domain;

namespace OpenAPU.Application.Tests.Apus;

public sealed class AddApuComponentHandlerTests
{
    [Fact]
    public async Task Adds_resource_and_recalculates_direct_cost()
    {
        var apuRepository = new FakeApuRepository();
        var resourceRepository = new FakeResourceRepository();

        var apu = Apu.Create(
            Key.From("APU-001"),
            "Muro",
            Unit.Create("M2", "m²", "Metro cuadrado"));

        var resource = Resource.Create(
            Key.From("MAT-001"),
            "Cemento",
            ResourceType.Material,
            Unit.Create("KG", "kg", "Kilogramo"),
            Money.From(4m));

        apuRepository.Apus.Add(apu);
        resourceRepository.Resources.Add(resource);

        var handler = new AddApuComponentHandler(
            apuRepository,
            resourceRepository);

        var result = await handler.HandleAsync(
            new AddApuComponentCommand(
                apu.Id.Value,
                resource.Id.Value,
                10m));

        Assert.Equal(40m, result.DirectCost);
        Assert.Equal(1, result.ComponentCount);
    }

    [Fact]
    public async Task Rejects_unknown_apu()
    {
        var handler = new AddApuComponentHandler(
            new FakeApuRepository(),
            new FakeResourceRepository());

        await Assert.ThrowsAsync<OpenAPU.Application.ApplicationException>(
            () => handler.HandleAsync(
                new AddApuComponentCommand(
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    1m)));
    }

    [Fact]
    public async Task Rejects_duplicate_resource()
    {
        var apuRepository = new FakeApuRepository();
        var resourceRepository = new FakeResourceRepository();

        var resource = Resource.Create(
            Key.From("MAT-001"),
            "Cemento",
            ResourceType.Material,
            Unit.Create("KG", "kg", "Kilogramo"),
            Money.From(4m));

        var apu = Apu.Create(
            Key.From("APU-001"),
            "Muro",
            Unit.Create("M2", "m²", "Metro cuadrado"));

        apu.AddComponent(resource, Quantity.From(1m));

        apuRepository.Apus.Add(apu);
        resourceRepository.Resources.Add(resource);

        var handler = new AddApuComponentHandler(
            apuRepository,
            resourceRepository);

        await Assert.ThrowsAsync<DomainException>(
            () => handler.HandleAsync(
                new AddApuComponentCommand(
                    apu.Id.Value,
                    resource.Id.Value,
                    1m)));
    }
}
