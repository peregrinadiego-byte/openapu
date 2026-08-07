using OpenAPU.Application.Resources;
using OpenAPU.Infrastructure.Repositories;

namespace OpenAPU.Integration.Tests.Resources;

public sealed class ResourceWorkflowTests
{
    [Fact]
    public async Task Create_and_query_resources_end_to_end()
    {
        var repository = new InMemoryResourceRepository();
        var createHandler = new CreateResourceHandler(repository);
        var queryHandler = new GetResourcesHandler(repository);

        await createHandler.HandleAsync(new CreateResourceCommand(
            "MAT-002",
            "Arena",
            ResourceTypeDto.Material,
            "M3",
            "mÂ³",
            "Metro cÃºbico",
            350m));

        await createHandler.HandleAsync(new CreateResourceCommand(
            "MAT-001",
            "Cemento",
            ResourceTypeDto.Material,
            "KG",
            "kg",
            "Kilogramo",
            4.50m));

        var result = (await queryHandler.HandleAsync()).ToArray();

        Assert.Equal(2, result.Length);
        Assert.Equal("MAT-001", result[0].Key);
        Assert.Equal("MAT-002", result[1].Key);
    }

    [Fact]
    public async Task Duplicate_key_is_rejected_through_application_layer()
    {
        var repository = new InMemoryResourceRepository();
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
    public async Task Repository_returns_snapshot_not_mutable_collection()
    {
        var repository = new InMemoryResourceRepository();
        var createHandler = new CreateResourceHandler(repository);

        await createHandler.HandleAsync(new CreateResourceCommand(
            "MO-001",
            "Oficial albaÃ±il",
            ResourceTypeDto.Labor,
            "H",
            "h",
            "Hora",
            80m));

        var firstRead = await repository.GetAllAsync();

        Assert.Single(firstRead);

        var secondRead = await repository.GetAllAsync();

        Assert.NotSame(firstRead, secondRead);
        Assert.Single(secondRead);
    }
}
