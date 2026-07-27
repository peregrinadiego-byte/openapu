using OpenAPU.Application.Resources;
using OpenAPU.Infrastructure.Repositories;

namespace OpenAPU.Integration.Tests.Persistence;

public sealed class SqliteResourceUpdateTests : IDisposable
{
    private readonly string _databasePath =
        Path.Combine(
            Path.GetTempPath(),
            $"openapu-update-{Guid.NewGuid():N}.db");

    [Fact]
    public async Task Updates_are_persisted_between_repository_instances()
    {
        var firstRepository = new SqliteResourceRepository(_databasePath);
        var createHandler = new CreateResourceHandler(firstRepository);

        var created = await createHandler.HandleAsync(
            new CreateResourceCommand(
                "MAT-001",
                "Cemento gris",
                ResourceTypeDto.Material,
                "KG",
                "kg",
                "Kilogramo",
                4.50m));

        var updateHandler = new UpdateResourceHandler(firstRepository);

        await updateHandler.HandleAsync(
            new UpdateResourceCommand(
                created.Id,
                "Cemento Portland",
                5.25m,
                false));

        var secondRepository = new SqliteResourceRepository(_databasePath);
        var queryHandler = new GetResourcesHandler(secondRepository);

        var stored = Assert.Single(await queryHandler.HandleAsync());

        Assert.Equal(created.Id, stored.Id);
        Assert.Equal("MAT-001", stored.Key);
        Assert.Equal("Cemento Portland", stored.Name);
        Assert.Equal(5.25m, stored.Price);
        Assert.Equal("Inactive", stored.Status);
    }

    [Fact]
    public async Task Update_preserves_immutable_fields()
    {
        var repository = new SqliteResourceRepository(_databasePath);
        var createHandler = new CreateResourceHandler(repository);

        var created = await createHandler.HandleAsync(
            new CreateResourceCommand(
                "EQ-001",
                "Revolvedora",
                ResourceTypeDto.Equipment,
                "H",
                "h",
                "Hora",
                120m));

        var updateHandler = new UpdateResourceHandler(repository);

        await updateHandler.HandleAsync(
            new UpdateResourceCommand(
                created.Id,
                "Revolvedora eléctrica",
                135m,
                true));

        var stored = Assert.Single(
            await new GetResourcesHandler(repository).HandleAsync());

        Assert.Equal("EQ-001", stored.Key);
        Assert.Equal("Equipment", stored.Type);
        Assert.Equal("h", stored.Unit);
    }

    public void Dispose()
    {
        if (File.Exists(_databasePath))
        {
            File.Delete(_databasePath);
        }
    }
}
