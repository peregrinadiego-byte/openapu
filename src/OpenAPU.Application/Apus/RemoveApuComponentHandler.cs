using OpenAPU.Application.Abstractions;
using OpenAPU.Domain;

namespace OpenAPU.Application.Apus;

public sealed class RemoveApuComponentHandler
{
    private readonly IApuRepository _repository;

    public RemoveApuComponentHandler(IApuRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<ApuDetailResult> HandleAsync(
        RemoveApuComponentCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var apuId = Identifier.From(command.ApuId);
        var apu = await _repository.GetByIdAsync(apuId, cancellationToken);

        if (apu is null)
        {
            throw new OpenAPU.Application.ApplicationException(
                $"APU '{apuId}' was not found.");
        }

        apu.RemoveComponent(
            Identifier.From(command.ComponentId));

        await _repository.UpdateAsync(apu, cancellationToken);

        return GetApuHandler.Map(apu);
    }
}
