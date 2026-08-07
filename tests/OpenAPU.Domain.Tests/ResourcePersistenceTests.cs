using OpenAPU.Domain;

namespace OpenAPU.Domain.Tests;

public sealed class ResourcePersistenceTests
{
    [Fact]
    public void Rehydrate_preserves_identity_and_state()
    {
        var id = Identifier.Create();

        var resource = Resource.Rehydrate(
            id,
            Key.From("MAT-001"),
            "Cemento",
            ResourceType.Material,
            Unit.Create("KG", "kg", "Kilogramo"),
            Money.From(4.50m),
            ResourceStatus.Inactive);

        Assert.Equal(id, resource.Id);
        Assert.Equal("MAT-001", resource.Key.Value);
        Assert.Equal(ResourceStatus.Inactive, resource.Status);
    }
}
