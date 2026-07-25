using OpenAPU.Application.Resources;
using OpenAPU.Domain;

namespace OpenAPU.Application.Tests.Resources;

public sealed class GetResourcesHandlerTests
{
    [Fact]
    public async Task Returns_empty_collection_when_repository_is_empty()
    {
        var repository = new FakeResourceRepository();
        var handler = new GetResourcesHandler(repository);

        var result = await handler.HandleAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task Returns_resources_ordered_by_key()
    {
        var repository = new FakeResourceRepository();

        repository.Resources.Add(Resource.Create(
            Key.From("MAT-002"),
            "Arena",
            ResourceType.Material,
            Unit.Create("M3", "mÂ³", "Metro cÃºbico"),
            Money.From(350m)));

        repository.Resources.Add(Resource.Create(
            Key.From("MAT-001"),
            "Cemento",
            ResourceType.Material,
            Unit.Create("KG", "kg", "Kilogramo"),
            Money.From(4.50m)));

        var handler = new GetResourcesHandler(repository);

        var result = (await handler.HandleAsync()).ToArray();

        Assert.Equal(2, result.Length);
        Assert.Equal("MAT-001", result[0].Key);
        Assert.Equal("MAT-002", result[1].Key);
    }

    [Fact]
    public async Task Maps_resource_data()
    {
        var repository = new FakeResourceRepository();

        repository.Resources.Add(Resource.Create(
            Key.From("MO-001"),
            "Oficial albaÃ±il",
            ResourceType.Labor,
            Unit.Create("H", "h", "Hora"),
            Money.From(80m)));

        var handler = new GetResourcesHandler(repository);

        var item = Assert.Single(await handler.HandleAsync());

        Assert.Equal("MO-001", item.Key);
        Assert.Equal("Oficial albaÃ±il", item.Name);
        Assert.Equal("Labor", item.Type);
        Assert.Equal("h", item.Unit);
        Assert.Equal(80m, item.Price);
        Assert.Equal("Active", item.Status);
    }
}
