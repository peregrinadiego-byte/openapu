using OpenAPU.Application.Abstractions;
using OpenAPU.Domain;

namespace OpenAPU.Application.Apus;

public sealed class RefreshApuPricesHandler
{
    private readonly IApuRepository _apuRepository;
    private readonly IResourceRepository _resourceRepository;

    public RefreshApuPricesHandler(
        IApuRepository apuRepository,
        IResourceRepository resourceRepository)
    {
        _apuRepository = apuRepository
            ?? throw new ArgumentNullException(nameof(apuRepository));

        _resourceRepository = resourceRepository
            ?? throw new ArgumentNullException(nameof(resourceRepository));
    }

    public async Task<ApuDetailResult> HandleAsync(
        Guid apuId,
        CancellationToken cancellationToken = default)
    {
        var id = Identifier.From(apuId);
        var apu = await _apuRepository.GetByIdAsync(id, cancellationToken);

        if (apu is null)
        {
            throw new OpenAPU.Application.ApplicationException(
                $"APU '{id}' was not found.");
        }

        var refreshedComponents = new List<ApuComponentSnapshot>();

        foreach (var component in apu.Components)
        {
            var resource = await _resourceRepository.GetByIdAsync(
                component.Resource.Id,
                cancellationToken);

            if (resource is null)
            {
                throw new OpenAPU.Application.ApplicationException(
                    $"Resource '{component.Resource.Id}' was not found.");
            }

            refreshedComponents.Add(new ApuComponentSnapshot(
                component.Id,
                resource,
                component.Quantity,
                resource.Price));
        }

        var refreshedApu = Apu.Rehydrate(
            apu.Id,
            apu.Key,
            apu.Name,
            apu.Unit,
            refreshedComponents);

        await _apuRepository.UpdateAsync(
            refreshedApu,
            cancellationToken);

        return GetApuHandler.Map(refreshedApu);
    }
}
