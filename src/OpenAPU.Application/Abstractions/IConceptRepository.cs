using OpenAPU.Domain;

namespace OpenAPU.Application.Abstractions;

public interface IConceptRepository
{
    Task<bool> ExistsByKeyAsync(
        Key key,
        CancellationToken cancellationToken = default);

    Task<Concept?> GetByIdAsync(
        Identifier id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Concept concept,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Concept concept,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Concept>> GetAllAsync(
        CancellationToken cancellationToken = default);
}
