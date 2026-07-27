using OpenAPU.Application.Abstractions;
using OpenAPU.Domain;

namespace OpenAPU.Infrastructure.Repositories;

public sealed class InMemoryResourceRepository : IResourceRepository
{
    private readonly List<Resource> _resources = [];
    private readonly object _sync = new();

    public Task<bool> ExistsByKeyAsync(
        Key key,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            return Task.FromResult(
                _resources.Any(resource => resource.Key == key));
        }
    }

    public Task<Resource?> GetByIdAsync(
        Identifier id,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            return Task.FromResult(
                _resources.SingleOrDefault(resource => resource.Id == id));
        }
    }

    public Task AddAsync(
        Resource resource,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(resource);
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            if (_resources.Any(existing => existing.Key == resource.Key))
            {
                throw new InvalidOperationException(
                    $"Resource key '{resource.Key}' already exists.");
            }

            _resources.Add(resource);
        }

        return Task.CompletedTask;
    }

    public Task UpdateAsync(
        Resource resource,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(resource);
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            var index = _resources.FindIndex(existing => existing.Id == resource.Id);

            if (index < 0)
            {
                throw new InvalidOperationException(
                    $"Resource '{resource.Id}' was not found.");
            }

            _resources[index] = resource;
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<Resource>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            IReadOnlyCollection<Resource> snapshot = _resources.ToArray();
            return Task.FromResult(snapshot);
        }
    }
}
