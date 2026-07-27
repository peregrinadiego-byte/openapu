using OpenAPU.Application.Abstractions;
using OpenAPU.Domain;

namespace OpenAPU.Infrastructure.Repositories;

public sealed class InMemoryApuRepository : IApuRepository
{
    private readonly List<Apu> _apus = [];
    private readonly object _sync = new();

    public Task<bool> ExistsByKeyAsync(
        Key key,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            return Task.FromResult(
                _apus.Any(apu => apu.Key == key));
        }
    }

    public Task<Apu?> GetByIdAsync(
        Identifier id,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            return Task.FromResult(
                _apus.SingleOrDefault(apu => apu.Id == id));
        }
    }

    public Task AddAsync(
        Apu apu,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(apu);
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            if (_apus.Any(existing => existing.Key == apu.Key))
            {
                throw new InvalidOperationException(
                    $"APU key '{apu.Key}' already exists.");
            }

            _apus.Add(apu);
        }

        return Task.CompletedTask;
    }

    public Task UpdateAsync(
        Apu apu,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(apu);
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            var index = _apus.FindIndex(existing => existing.Id == apu.Id);

            if (index < 0)
            {
                throw new InvalidOperationException(
                    $"APU '{apu.Id}' was not found.");
            }

            _apus[index] = apu;
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<Apu>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            IReadOnlyCollection<Apu> result = _apus.ToArray();
            return Task.FromResult(result);
        }
    }
}
