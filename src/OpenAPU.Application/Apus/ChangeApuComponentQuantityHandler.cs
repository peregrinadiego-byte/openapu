using OpenAPU.Application.Abstractions;
using OpenAPU.Domain;

namespace OpenAPU.Application.Apus;

public sealed class ChangeApuComponentQuantityHandler
{
    private readonly IApuRepository _repository;

    public ChangeApuComponentQuantityHandler(IApuRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<ApuDetailResult> HandleAsync(
        ChangeApuComponentQuantityCommand command,
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

        apu.ChangeQuantity(
            Identifier.From(command.ComponentId),
            Quantity.From(command.Quantity));

        await _repository.UpdateAsync(apu, cancellationToken);

        return GetApuHandler.Map(apu);
    }
}
