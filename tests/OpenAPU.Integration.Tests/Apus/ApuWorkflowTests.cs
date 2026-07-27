using OpenAPU.Application.Apus;
using OpenAPU.Application.Resources;
using OpenAPU.Infrastructure.Repositories;

namespace OpenAPU.Integration.Tests.Apus;

public sealed class ApuWorkflowTests
{
    [Fact]
    public async Task Creates_apu_and_adds_persisted_resource()
    {
        var resourceRepository = new InMemoryResourceRepository();
        var apuRepository = new InMemoryApuRepository();

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

        var updated = await new AddApuComponentHandler(
                apuRepository,
                resourceRepository)
            .HandleAsync(
                new AddApuComponentCommand(
                    apu.Id,
                    resource.Id,
                    10m));

        Assert.Equal(40m, updated.DirectCost);
        Assert.Equal(1, updated.ComponentCount);
    }
}
