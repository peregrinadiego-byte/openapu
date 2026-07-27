using OpenAPU.Application.Abstractions;
using OpenAPU.Domain;

namespace OpenAPU.Application.Apus;

public sealed class AddApuComponentHandler
{
    private readonly IApuRepository _apuRepository;
    private readonly IResourceRepository _resourceRepository;

    public AddApuComponentHandler(
        IApuRepository apuRepository,
        IResourceRepository resourceRepository)
    {
        _apuRepository = apuRepository
            ?? throw new ArgumentNullException(nameof(apuRepository));

        _resourceRepository = resourceRepository
            ?? throw new ArgumentNullException(nameof(resourceRepository));
    }

    public async Task<ApuResult> HandleAsync(
        AddApuComponentCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var apuId = Identifier.From(command.ApuId);
        var resourceId = Identifier.From(command.ResourceId);

        var apu = await _apuRepository.GetByIdAsync(
            apuId,
            cancellationToken);

        if (apu is null)
        {
            throw new OpenAPU.Application.ApplicationException(
                $"APU '{apuId}' was not found.");
        }

        var resource = await _resourceRepository.GetByIdAsync(
            resourceId,
            cancellationToken);

        if (resource is null)
        {
            throw new OpenAPU.Application.ApplicationException(
                $"Resource '{resourceId}' was not found.");
        }

        apu.AddComponent(
            resource,
            Quantity.From(command.Quantity));

        await _apuRepository.UpdateAsync(
            apu,
            cancellationToken);

        return CreateApuHandler.Map(apu);
    }
}
