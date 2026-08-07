using OpenAPU.Application.Resources;
using OpenAPU.Infrastructure.Repositories;

namespace OpenAPU.Integration.Tests.Persistence;

public sealed class SqliteResourceRepositoryTests : IDisposable
{
    private readonly string _databasePath =
        Path.Combine(
            Path.GetTempPath(),
            $"openapu-{Guid.NewGuid():N}.db");

    [Fact]
    public async Task Persists_resource_between_repository_instances()
    {
        var firstRepository = new SqliteResourceRepository(_databasePath);
        var createHandler = new CreateResourceHandler(firstRepository);

        var created = await createHandler.HandleAsync(
            new CreateResourceCommand(
                "MAT-001",
                "Cemento",
                ResourceTypeDto.Material,
                "KG",
                "kg",
                "Kilogramo",
                4.50m));

        var secondRepository = new SqliteResourceRepository(_databasePath);
        var queryHandler = new GetResourcesHandler(secondRepository);

        var stored = Assert.Single(await queryHandler.HandleAsync());

        Assert.Equal(created.Id, stored.Id);
        Assert.Equal("MAT-001", stored.Key);
        Assert.Equal("Cemento", stored.Name);
        Assert.Equal(4.50m, stored.Price);
    }

    [Fact]
    public async Task Duplicate_key_is_rejected_in_sqlite()
    {
        var repository = new SqliteResourceRepository(_databasePath);
        var handler = new CreateResourceHandler(repository);

        var command = new CreateResourceCommand(
            "MAT-001",
            "Cemento",
            ResourceTypeDto.Material,
            "KG",
            "kg",
            "Kilogramo",
            4.50m);

        await handler.HandleAsync(command);

        await Assert.ThrowsAsync<OpenAPU.Application.ApplicationException>(
            () => handler.HandleAsync(command));
    }

    [Fact]
    public async Task Empty_database_returns_empty_collection()
    {
        var repository = new SqliteResourceRepository(_databasePath);
        var handler = new GetResourcesHandler(repository);

        var result = await handler.HandleAsync();

        Assert.Empty(result);
    }

    public void Dispose()
    {
        if (File.Exists(_databasePath))
        {
            File.Delete(_databasePath);
        }
    }
}
