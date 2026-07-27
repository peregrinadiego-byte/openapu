using OpenAPU.Application.Abstractions;
using OpenAPU.Domain;

namespace OpenAPU.Application.Apus;

public sealed class CreateApuHandler
{
    private readonly IApuRepository _repository;

    public CreateApuHandler(IApuRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<ApuResult> HandleAsync(
        CreateApuCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var key = Key.From(command.Key);

        if (await _repository.ExistsByKeyAsync(key, cancellationToken))
        {
            throw new OpenAPU.Application.ApplicationException(
                $"APU key '{key}' already exists.");
        }

        var apu = Apu.Create(
            key,
            command.Name,
            Unit.Create(
                command.UnitCode,
                command.UnitSymbol,
                command.UnitName));

        await _repository.AddAsync(apu, cancellationToken);

        return Map(apu);
    }

    internal static ApuResult Map(Apu apu) => new(
        apu.Id.Value,
        apu.Key.Value,
        apu.Name,
        apu.Unit.Symbol,
        apu.DirectCost.Amount,
        apu.Components.Count);
}
