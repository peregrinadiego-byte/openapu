using OpenAPU.Domain;

namespace OpenAPU.Application.Abstractions;

public interface IApuRepository
{
    Task<bool> ExistsByKeyAsync(
        Key key,
        CancellationToken cancellationToken = default);

    Task<Apu?> GetByIdAsync(
        Identifier id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Apu apu,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Apu apu,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Apu>> GetAllAsync(
        CancellationToken cancellationToken = default);
}
