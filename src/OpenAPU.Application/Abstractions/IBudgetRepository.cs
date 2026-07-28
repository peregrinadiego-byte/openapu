using OpenAPU.Domain;

namespace OpenAPU.Application.Abstractions;

public interface IBudgetRepository
{
    Task<bool> ExistsByKeyAsync(
        Key key,
        CancellationToken cancellationToken = default);

    Task<Budget?> GetByIdAsync(
        Identifier id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Budget budget,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Budget budget,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Budget>> GetAllAsync(
        CancellationToken cancellationToken = default);
}
