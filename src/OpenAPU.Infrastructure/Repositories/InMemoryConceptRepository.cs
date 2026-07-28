using OpenAPU.Application.Abstractions;
using OpenAPU.Domain;

namespace OpenAPU.Infrastructure.Repositories;

public sealed class InMemoryConceptRepository : IConceptRepository
{
    private readonly List<Concept> _concepts = [];
    private readonly object _sync = new();

    public Task<bool> ExistsByKeyAsync(
        Key key,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            return Task.FromResult(
                _concepts.Any(concept => concept.Key == key));
        }
    }

    public Task<Concept?> GetByIdAsync(
        Identifier id,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            return Task.FromResult(
                _concepts.SingleOrDefault(concept => concept.Id == id));
        }
    }

    public Task AddAsync(
        Concept concept,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(concept);
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            if (_concepts.Any(existing => existing.Key == concept.Key))
            {
                throw new InvalidOperationException(
                    $"Concept key '{concept.Key}' already exists.");
            }

            _concepts.Add(concept);
        }

        return Task.CompletedTask;
    }

    public Task UpdateAsync(
        Concept concept,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(concept);
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            var index = _concepts.FindIndex(
                existing => existing.Id == concept.Id);

            if (index < 0)
            {
                throw new InvalidOperationException(
                    $"Concept '{concept.Id}' was not found.");
            }

            _concepts[index] = concept;
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<Concept>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            IReadOnlyCollection<Concept> result = _concepts.ToArray();
            return Task.FromResult(result);
        }
    }
}
