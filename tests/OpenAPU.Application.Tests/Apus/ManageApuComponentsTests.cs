using OpenAPU.Application.Apus;
using OpenAPU.Application.Tests.Resources;
using OpenAPU.Domain;

namespace OpenAPU.Application.Tests.Apus;

public sealed class ManageApuComponentsTests
{
    [Fact]
    public async Task Gets_apu_detail_with_components()
    {
        var repository = new FakeApuRepository();
        var resource = CreateResource(4m);
        var apu = CreateApu(resource, 10m);

        repository.Apus.Add(apu);

        var result = await new GetApuHandler(repository)
            .HandleAsync(apu.Id.Value);

        var component = Assert.Single(result.Components);

        Assert.Equal(40m, result.DirectCost);
        Assert.Equal("MAT-001", component.ResourceKey);
        Assert.Equal(10m, component.Quantity);
        Assert.Equal(4m, component.UnitPrice);
        Assert.Equal(40m, component.Total);
    }

    [Fact]
    public async Task Changes_component_quantity()
    {
        var repository = new FakeApuRepository();
        var apu = CreateApu(CreateResource(4m), 10m);
        repository.Apus.Add(apu);

        var componentId = Assert.Single(apu.Components).Id.Value;

        var result = await new ChangeApuComponentQuantityHandler(repository)
            .HandleAsync(new ChangeApuComponentQuantityCommand(
                apu.Id.Value,
                componentId,
                15m));

        Assert.Equal(60m, result.DirectCost);
        Assert.Equal(15m, Assert.Single(result.Components).Quantity);
    }

    [Fact]
    public async Task Removes_component()
    {
        var repository = new FakeApuRepository();
        var apu = CreateApu(CreateResource(4m), 10m);
        repository.Apus.Add(apu);

        var componentId = Assert.Single(apu.Components).Id.Value;

        var result = await new RemoveApuComponentHandler(repository)
            .HandleAsync(new RemoveApuComponentCommand(
                apu.Id.Value,
                componentId));

        Assert.Empty(result.Components);
        Assert.Equal(0m, result.DirectCost);
    }

    [Fact]
    public async Task Refreshes_component_prices()
    {
        var apuRepository = new FakeApuRepository();
        var resourceRepository = new FakeResourceRepository();

        var oldResource = CreateResource(4m);
        var apu = CreateApu(oldResource, 10m);

        var updatedResource = Resource.Rehydrate(
            oldResource.Id,
            oldResource.Key,
            oldResource.Name,
            oldResource.Type,
            oldResource.Unit,
            Money.From(5m),
            oldResource.Status);

        apuRepository.Apus.Add(apu);
        resourceRepository.Resources.Add(updatedResource);

        var result = await new RefreshApuPricesHandler(
                apuRepository,
                resourceRepository)
            .HandleAsync(apu.Id.Value);

        Assert.Equal(50m, result.DirectCost);
        Assert.Equal(5m, Assert.Single(result.Components).UnitPrice);
    }

    private static Resource CreateResource(decimal price) =>
        Resource.Create(
            Key.From("MAT-001"),
            "Cemento",
            ResourceType.Material,
            Unit.Create("KG", "kg", "Kilogramo"),
            Money.From(price));

    private static Apu CreateApu(Resource resource, decimal quantity)
    {
        var apu = Apu.Create(
            Key.From("APU-001"),
            "Muro",
            Unit.Create("M2", "m²", "Metro cuadrado"));

        apu.AddComponent(resource, Quantity.From(quantity));
        return apu;
    }
}
