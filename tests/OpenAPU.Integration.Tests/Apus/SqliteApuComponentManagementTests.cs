using OpenAPU.Application.Apus;
using OpenAPU.Application.Resources;
using OpenAPU.Infrastructure.Repositories;

namespace OpenAPU.Integration.Tests.Apus;

public sealed class SqliteApuComponentManagementTests : IDisposable
{
    private readonly string _databasePath =
        Path.Combine(
            Path.GetTempPath(),
            $"openapu-manage-{Guid.NewGuid():N}.db");

    [Fact]
    public async Task Quantity_change_is_persisted()
    {
        var setup = await CreateApuWithComponentAsync();

        var detail = await new GetApuHandler(setup.ApuRepository)
            .HandleAsync(setup.ApuId);

        var componentId = Assert.Single(detail.Components).Id;

        await new ChangeApuComponentQuantityHandler(setup.ApuRepository)
            .HandleAsync(new ChangeApuComponentQuantityCommand(
                setup.ApuId,
                componentId,
                20m));

        var reloaded = await new GetApuHandler(
                new SqliteApuRepository(_databasePath))
            .HandleAsync(setup.ApuId);

        Assert.Equal(80m, reloaded.DirectCost);
        Assert.Equal(20m, Assert.Single(reloaded.Components).Quantity);
    }

    [Fact]
    public async Task Removed_component_stays_removed()
    {
        var setup = await CreateApuWithComponentAsync();

        var detail = await new GetApuHandler(setup.ApuRepository)
            .HandleAsync(setup.ApuId);

        var componentId = Assert.Single(detail.Components).Id;

        await new RemoveApuComponentHandler(setup.ApuRepository)
            .HandleAsync(new RemoveApuComponentCommand(
                setup.ApuId,
                componentId));

        var reloaded = await new GetApuHandler(
                new SqliteApuRepository(_databasePath))
            .HandleAsync(setup.ApuId);

        Assert.Empty(reloaded.Components);
        Assert.Equal(0m, reloaded.DirectCost);
    }

    [Fact]
    public async Task Price_refresh_uses_latest_persisted_resource_price()
    {
        var setup = await CreateApuWithComponentAsync();

        await new UpdateResourceHandler(setup.ResourceRepository)
            .HandleAsync(new UpdateResourceCommand(
                setup.ResourceId,
                "Cemento",
                6m,
                true));

        await new RefreshApuPricesHandler(
                setup.ApuRepository,
                setup.ResourceRepository)
            .HandleAsync(setup.ApuId);

        var reloaded = await new GetApuHandler(
                new SqliteApuRepository(_databasePath))
            .HandleAsync(setup.ApuId);

        Assert.Equal(60m, reloaded.DirectCost);
        Assert.Equal(6m, Assert.Single(reloaded.Components).UnitPrice);
    }

    private async Task<SetupResult> CreateApuWithComponentAsync()
    {
        var resourceRepository =
            new SqliteResourceRepository(_databasePath);

        var apuRepository =
            new SqliteApuRepository(_databasePath);

        var resource = await new CreateResourceHandler(resourceRepository)
            .HandleAsync(new CreateResourceCommand(
                "MAT-001",
                "Cemento",
                ResourceTypeDto.Material,
                "KG",
                "kg",
                "Kilogramo",
                4m));

        var apu = await new CreateApuHandler(apuRepository)
            .HandleAsync(new CreateApuCommand(
                "APU-001",
                "Muro",
                "M2",
                "m²",
                "Metro cuadrado"));

        await new AddApuComponentHandler(
                apuRepository,
                resourceRepository)
            .HandleAsync(new AddApuComponentCommand(
                apu.Id,
                resource.Id,
                10m));

        return new SetupResult(
            resourceRepository,
            apuRepository,
            resource.Id,
            apu.Id);
    }

    public void Dispose()
    {
        if (File.Exists(_databasePath))
        {
            File.Delete(_databasePath);
        }
    }

    private sealed record SetupResult(
        SqliteResourceRepository ResourceRepository,
        SqliteApuRepository ApuRepository,
        Guid ResourceId,
        Guid ApuId);
}
