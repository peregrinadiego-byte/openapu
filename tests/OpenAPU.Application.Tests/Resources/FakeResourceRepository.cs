using OpenAPU.Application.Abstractions;
using OpenAPU.Domain;

namespace OpenAPU.Application.Tests.Resources;

internal sealed class FakeResourceRepository : IResourceRepository
{
    public List<Resource> Resources { get; } = [];

    public Task<bool> ExistsByKeyAsync(
        Key key,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            Resources.Any(resource => resource.Key == key));
    }

    public Task<Resource?> GetByIdAsync(
        Identifier id,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            Resources.SingleOrDefault(resource => resource.Id == id));
    }

    public Task AddAsync(
        Resource resource,
        CancellationToken cancellationToken = default)
    {
        Resources.Add(resource);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(
        Resource resource,
        CancellationToken cancellationToken = default)
    {
        var index = Resources.FindIndex(existing => existing.Id == resource.Id);

        if (index < 0)
        {
            throw new InvalidOperationException("Resource was not found.");
        }

        Resources[index] = resource;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<Resource>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Resource> result = Resources.ToArray();
        return Task.FromResult(result);
    }
}
