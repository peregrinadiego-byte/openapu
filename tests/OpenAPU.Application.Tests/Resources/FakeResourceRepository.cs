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

    public Task AddAsync(
        Resource resource,
        CancellationToken cancellationToken = default)
    {
        Resources.Add(resource);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<Resource>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Resource> result = Resources.ToArray();
        return Task.FromResult(result);
    }
}
