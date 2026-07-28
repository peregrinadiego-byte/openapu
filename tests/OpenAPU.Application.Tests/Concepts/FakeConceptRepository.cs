using OpenAPU.Application.Abstractions;
using OpenAPU.Domain;

namespace OpenAPU.Application.Tests.Concepts;

internal sealed class FakeConceptRepository : IConceptRepository
{
    public List<Concept> Concepts { get; } = [];

    public Task<bool> ExistsByKeyAsync(
        Key key,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            Concepts.Any(concept => concept.Key == key));
    }

    public Task<Concept?> GetByIdAsync(
        Identifier id,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            Concepts.SingleOrDefault(concept => concept.Id == id));
    }

    public Task AddAsync(
        Concept concept,
        CancellationToken cancellationToken = default)
    {
        Concepts.Add(concept);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(
        Concept concept,
        CancellationToken cancellationToken = default)
    {
        var index = Concepts.FindIndex(
            existing => existing.Id == concept.Id);

        if (index < 0)
        {
            throw new InvalidOperationException("Concept was not found.");
        }

        Concepts[index] = concept;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<Concept>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Concept> result = Concepts.ToArray();
        return Task.FromResult(result);
    }
}
