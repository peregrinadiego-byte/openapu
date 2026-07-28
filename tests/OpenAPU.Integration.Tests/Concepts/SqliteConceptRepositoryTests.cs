using OpenAPU.Application.Apus;
using OpenAPU.Application.Concepts;
using OpenAPU.Application.Resources;
using OpenAPU.Infrastructure.Repositories;

namespace OpenAPU.Integration.Tests.Concepts;

public sealed class SqliteConceptRepositoryTests : IDisposable
{
    private readonly string _databasePath =
        Path.Combine(
            Path.GetTempPath(),
            $"openapu-concept-{Guid.NewGuid():N}.db");

    [Fact]
    public async Task Persists_concept_and_percentages_between_instances()
    {
        var resourceRepository =
            new SqliteResourceRepository(_databasePath);

        var apuRepository =
            new SqliteApuRepository(_databasePath);

        var conceptRepository =
            new SqliteConceptRepository(_databasePath);

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
                    25m));

        var concept = await new CreateConceptHandler(
                conceptRepository,
                apuRepository)
            .HandleAsync(
                new CreateConceptCommand(
                    "CON-001",
                    "Muro",
                    "M2",
                    "m²",
                    "Metro cuadrado",
                    apu.Id));

        await new UpdateConceptPercentagesHandler(conceptRepository)
            .HandleAsync(
                new UpdateConceptPercentagesCommand(
                    concept.Id,
                    10m,
                    3m,
                    12m,
                    2m));

        var secondRepository =
            new SqliteConceptRepository(_databasePath);

        var stored = await secondRepository.GetByIdAsync(
            OpenAPU.Domain.Identifier.From(concept.Id));

        Assert.NotNull(stored);
        Assert.Equal(concept.Id, stored.Id.Value);
        Assert.Equal(100m, stored.DirectCost.Amount);
        Assert.Equal(127m, stored.UnitPrice.Amount);
        Assert.Equal(10m, stored.IndirectCost.Value);
        Assert.Equal(3m, stored.Financing.Value);
        Assert.Equal(12m, stored.Profit.Value);
        Assert.Equal(2m, stored.AdditionalCharges.Value);
    }

    [Fact]
    public async Task Concept_identity_and_apu_relation_are_preserved()
    {
        var resourceRepository =
            new SqliteResourceRepository(_databasePath);

        var apuRepository =
            new SqliteApuRepository(_databasePath);

        var conceptRepository =
            new SqliteConceptRepository(_databasePath);

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

        var concept = await new CreateConceptHandler(
                conceptRepository,
                apuRepository)
            .HandleAsync(
                new CreateConceptCommand(
                    "CON-001",
                    "Muro",
                    "M2",
                    "m²",
                    "Metro cuadrado",
                    apu.Id));

        var stored = await conceptRepository.GetByIdAsync(
            OpenAPU.Domain.Identifier.From(concept.Id));

        Assert.NotNull(stored);
        Assert.Equal(concept.Id, stored.Id.Value);
        Assert.Equal(apu.Id, stored.Apu.Id.Value);
        Assert.Equal(160m, stored.DirectCost.Amount);
    }

    [Fact]
    public async Task Duplicate_concept_key_is_rejected_in_sqlite()
    {
        var resourceRepository =
            new SqliteResourceRepository(_databasePath);

        var apuRepository =
            new SqliteApuRepository(_databasePath);

        var conceptRepository =
            new SqliteConceptRepository(_databasePath);

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
                    1m));

        var handler = new CreateConceptHandler(
            conceptRepository,
            apuRepository);

        var command = new CreateConceptCommand(
            "CON-001",
            "Muro",
            "M2",
            "m²",
            "Metro cuadrado",
            apu.Id);

        await handler.HandleAsync(command);

        await Assert.ThrowsAsync<OpenAPU.Application.ApplicationException>(
            () => handler.HandleAsync(command));
    }

    public void Dispose()
    {
        if (File.Exists(_databasePath))
        {
            File.Delete(_databasePath);
        }
    }
}
