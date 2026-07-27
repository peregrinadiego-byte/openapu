using OpenAPU.Application.Apus;
using OpenAPU.Application.Resources;
using OpenAPU.Infrastructure.Repositories;

namespace OpenAPU.Integration.Tests.Apus;

public sealed class SqliteApuRepositoryTests : IDisposable
{
    private readonly string _databasePath =
        Path.Combine(
            Path.GetTempPath(),
            $"openapu-apu-{Guid.NewGuid():N}.db");

    [Fact]
    public async Task Persists_apu_and_components_between_instances()
    {
        var resourceRepository =
            new SqliteResourceRepository(_databasePath);

        var apuRepository =
            new SqliteApuRepository(_databasePath);

        var resource = await new CreateResourceHandler(resourceRepository)
            .HandleAsync(
                new CreateResourceCommand(
                    "MAT-001",
                    "Cemento",
                    ResourceTypeDto.Material,
                    "KG",
                    "kg",
                    "Kilogramo",
                    4m));

        var apu = await new CreateApuHandler(apuRepository)
            .HandleAsync(
                new CreateApuCommand(
                    "APU-001",
                    "Muro",
                    "M2",
                    "m²",
                    "Metro cuadrado"));

        await new AddApuComponentHandler(
                apuRepository,
                resourceRepository)
            .HandleAsync(
                new AddApuComponentCommand(
                    apu.Id,
                    resource.Id,
                    10m));

        var secondRepository =
            new SqliteApuRepository(_databasePath);

        var stored = await secondRepository.GetByIdAsync(
            OpenAPU.Domain.Identifier.From(apu.Id));

        Assert.NotNull(stored);
        Assert.Equal(apu.Id, stored.Id.Value);
        Assert.Single(stored.Components);
        Assert.Equal(40m, stored.DirectCost.Amount);
    }

    [Fact]
    public async Task Component_identity_is_preserved()
    {
        var resourceRepository =
            new SqliteResourceRepository(_databasePath);

        var apuRepository =
            new SqliteApuRepository(_databasePath);

        var resource = await new CreateResourceHandler(resourceRepository)
            .HandleAsync(
                new CreateResourceCommand(
                    "MO-001",
                    "Oficial albañil",
                    ResourceTypeDto.Labor,
                    "H",
                    "h",
                    "Hora",
                    80m));

        var apu = await new CreateApuHandler(apuRepository)
            .HandleAsync(
                new CreateApuCommand(
                    "APU-001",
                    "Muro",
                    "M2",
                    "m²",
                    "Metro cuadrado"));

        await new AddApuComponentHandler(
                apuRepository,
                resourceRepository)
            .HandleAsync(
                new AddApuComponentCommand(
                    apu.Id,
                    resource.Id,
                    2m));

        var firstRead = await apuRepository.GetByIdAsync(
            OpenAPU.Domain.Identifier.From(apu.Id));

        var componentId = Assert.Single(firstRead!.Components).Id;

        var secondRepository =
            new SqliteApuRepository(_databasePath);

        var secondRead = await secondRepository.GetByIdAsync(
            OpenAPU.Domain.Identifier.From(apu.Id));

        Assert.Equal(
            componentId,
            Assert.Single(secondRead!.Components).Id);
    }

    [Fact]
    public async Task Duplicate_resource_is_rejected_after_reload()
    {
        var resourceRepository =
            new SqliteResourceRepository(_databasePath);

        var apuRepository =
            new SqliteApuRepository(_databasePath);

        var resource = await new CreateResourceHandler(resourceRepository)
            .HandleAsync(
                new CreateResourceCommand(
                    "MAT-001",
                    "Cemento",
                    ResourceTypeDto.Material,
                    "KG",
                    "kg",
                    "Kilogramo",
                    4m));

        var apu = await new CreateApuHandler(apuRepository)
            .HandleAsync(
                new CreateApuCommand(
                    "APU-001",
                    "Muro",
                    "M2",
                    "m²",
                    "Metro cuadrado"));

        var handler = new AddApuComponentHandler(
            apuRepository,
            resourceRepository);

        await handler.HandleAsync(
            new AddApuComponentCommand(
                apu.Id,
                resource.Id,
                1m));

        await Assert.ThrowsAsync<OpenAPU.Domain.DomainException>(
            () => handler.HandleAsync(
                new AddApuComponentCommand(
                    apu.Id,
                    resource.Id,
                    1m)));
    }

    public void Dispose()
    {
        if (File.Exists(_databasePath))
        {
            File.Delete(_databasePath);
        }
    }
}
