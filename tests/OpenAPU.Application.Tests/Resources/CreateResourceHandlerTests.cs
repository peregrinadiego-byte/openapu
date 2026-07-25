using OpenAPU.Application.Abstractions;
using OpenAPU.Application.Resources;
using OpenAPU.Domain;

namespace OpenAPU.Application.Tests.Resources;

public sealed class CreateResourceHandlerTests
{
    [Fact]
    public async Task Creates_and_persists_resource()
    {
        var repository = new FakeResourceRepository();
        var handler = new CreateResourceHandler(repository);

        var command = new CreateResourceCommand(
            "MAT-001",
            "Cemento",
            ResourceTypeDto.Material,
            "KG",
            "kg",
            "Kilogramo",
            4.50m);

        var result = await handler.HandleAsync(command);

        Assert.Equal("MAT-001", result.Key);
        Assert.Equal("Cemento", result.Name);
        Assert.Equal("kg", result.Unit);
        Assert.Equal(4.50m, result.Price);
        Assert.Equal("Active", result.Status);
        Assert.Single(repository.Resources);
    }

    [Fact]
    public async Task Rejects_duplicate_key()
    {
        var repository = new FakeResourceRepository();
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

        var exception = await Assert.ThrowsAsync<ApplicationException>(
            () => handler.HandleAsync(command));

        Assert.Contains("MAT-001", exception.Message);
        Assert.Single(repository.Resources);
    }

    [Fact]
    public async Task Domain_validation_is_preserved()
    {
        var repository = new FakeResourceRepository();
        var handler = new CreateResourceHandler(repository);

        var command = new CreateResourceCommand(
            "MAT-002",
            "",
            ResourceTypeDto.Material,
            "KG",
            "kg",
            "Kilogramo",
            4.50m);

        await Assert.ThrowsAsync<DomainException>(
            () => handler.HandleAsync(command));

        Assert.Empty(repository.Resources);
    }

    private sealed class FakeResourceRepository : IResourceRepository
    {
        public List<Resource> Resources { get; } = [];

        public Task<bool> ExistsByKeyAsync(
            Key key,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                Resources.Any(resource => resource.Key == key));
        }

        public Task AddAsync(
            Resource resource,
            CancellationToken cancellationToken = default)
        {
            Resources.Add(resource);
            return Task.CompletedTask;
        }
    }
}
