using OpenAPU.Application.Abstractions;
using OpenAPU.Domain;

namespace OpenAPU.Application.Apus;

public sealed class GetApuHandler
{
    private readonly IApuRepository _repository;

    public GetApuHandler(IApuRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<ApuDetailResult> HandleAsync(
        Guid apuId,
        CancellationToken cancellationToken = default)
    {
        var id = Identifier.From(apuId);
        var apu = await _repository.GetByIdAsync(id, cancellationToken);

        if (apu is null)
        {
            throw new OpenAPU.Application.ApplicationException(
                $"APU '{id}' was not found.");
        }

        return Map(apu);
    }

    internal static ApuDetailResult Map(Apu apu) => new(
        apu.Id.Value,
        apu.Key.Value,
        apu.Name,
        apu.Unit.Symbol,
        apu.DirectCost.Amount,
        apu.Components
            .Select(component => new ApuComponentResult(
                component.Id.Value,
                component.Resource.Id.Value,
                component.Resource.Key.Value,
                component.Resource.Name,
                component.Quantity.Value,
                component.UnitPrice.Amount,
                component.Total.Amount))
            .ToArray());
}
