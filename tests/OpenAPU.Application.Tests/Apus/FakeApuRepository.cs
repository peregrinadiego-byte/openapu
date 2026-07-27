using OpenAPU.Application.Abstractions;
using OpenAPU.Domain;

namespace OpenAPU.Application.Tests.Apus;

internal sealed class FakeApuRepository : IApuRepository
{
    public List<Apu> Apus { get; } = [];

    public Task<bool> ExistsByKeyAsync(
        Key key,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            Apus.Any(apu => apu.Key == key));
    }

    public Task<Apu?> GetByIdAsync(
        Identifier id,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            Apus.SingleOrDefault(apu => apu.Id == id));
    }

    public Task AddAsync(
        Apu apu,
        CancellationToken cancellationToken = default)
    {
        Apus.Add(apu);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(
        Apu apu,
        CancellationToken cancellationToken = default)
    {
        var index = Apus.FindIndex(existing => existing.Id == apu.Id);

        if (index < 0)
        {
            throw new InvalidOperationException("APU was not found.");
        }

        Apus[index] = apu;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<Apu>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Apu> result = Apus.ToArray();
        return Task.FromResult(result);
    }
}
