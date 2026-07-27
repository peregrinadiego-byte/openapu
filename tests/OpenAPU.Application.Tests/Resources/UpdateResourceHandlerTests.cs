using OpenAPU.Application.Resources;
using OpenAPU.Domain;

namespace OpenAPU.Application.Tests.Resources;

public sealed class UpdateResourceHandlerTests
{
    [Fact]
    public async Task Updates_name_price_and_status()
    {
        var repository = new FakeResourceRepository();

        var resource = Resource.Create(
            Key.From("MAT-001"),
            "Cemento gris",
            ResourceType.Material,
            Unit.Create("KG", "kg", "Kilogramo"),
            Money.From(4.50m));

        repository.Resources.Add(resource);

        var handler = new UpdateResourceHandler(repository);

        var result = await handler.HandleAsync(
            new UpdateResourceCommand(
                resource.Id.Value,
                "Cemento Portland",
                5.25m,
                false));

        Assert.Equal(resource.Id.Value, result.Id);
        Assert.Equal("MAT-001", result.Key);
        Assert.Equal("Cemento Portland", result.Name);
        Assert.Equal(5.25m, result.Price);
        Assert.Equal("Inactive", result.Status);
    }

    [Fact]
    public async Task Rejects_unknown_resource()
    {
        var repository = new FakeResourceRepository();
        var handler = new UpdateResourceHandler(repository);

        await Assert.ThrowsAsync<OpenAPU.Application.ApplicationException>(
            () => handler.HandleAsync(
                new UpdateResourceCommand(
                    Guid.NewGuid(),
                    "Cemento",
                    5m,
                    true)));
    }

    [Fact]
    public async Task Preserves_domain_validation()
    {
        var repository = new FakeResourceRepository();

        var resource = Resource.Create(
            Key.From("MAT-001"),
            "Cemento",
            ResourceType.Material,
            Unit.Create("KG", "kg", "Kilogramo"),
            Money.From(4.50m));

        repository.Resources.Add(resource);

        var handler = new UpdateResourceHandler(repository);

        await Assert.ThrowsAsync<DomainException>(
            () => handler.HandleAsync(
                new UpdateResourceCommand(
                    resource.Id.Value,
                    "",
                    5m,
                    true)));
    }
}
