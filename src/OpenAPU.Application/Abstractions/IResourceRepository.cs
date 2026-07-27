using OpenAPU.Domain;

namespace OpenAPU.Application.Abstractions;

public interface IResourceRepository
{
    Task<bool> ExistsByKeyAsync(
        Key key,
        CancellationToken cancellationToken = default);

    Task<Resource?> GetByIdAsync(
        Identifier id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Resource resource,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Resource resource,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Resource>> GetAllAsync(
        CancellationToken cancellationToken = default);
}
