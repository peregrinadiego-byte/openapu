using OpenAPU.Application.Apus;
using OpenAPU.Application.Concepts;
using OpenAPU.Application.Resources;
using OpenAPU.Infrastructure.Repositories;

namespace OpenAPU.Integration.Tests.Concepts;

public sealed class ConceptWorkflowTests
{
    [Fact]
    public async Task Creates_concept_and_calculates_full_unit_price()
    {
        var resourceRepository = new InMemoryResourceRepository();
        var apuRepository = new InMemoryApuRepository();
        var conceptRepository = new InMemoryConceptRepository();

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

        var updated = await new UpdateConceptPercentagesHandler(
                conceptRepository)
            .HandleAsync(
                new UpdateConceptPercentagesCommand(
                    concept.Id,
                    10m,
                    3m,
                    12m,
                    2m));

        Assert.Equal(100m, updated.DirectCost);
        Assert.Equal(127m, updated.UnitPrice);
    }
}
