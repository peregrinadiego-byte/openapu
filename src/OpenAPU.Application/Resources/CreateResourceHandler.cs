using OpenAPU.Application.Abstractions;
using OpenAPU.Domain;

namespace OpenAPU.Application.Resources;

public sealed class CreateResourceHandler
{
    private readonly IResourceRepository _repository;

    public CreateResourceHandler(IResourceRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<CreateResourceResult> HandleAsync(
        CreateResourceCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var key = Key.From(command.Key);

        if (await _repository.ExistsByKeyAsync(key, cancellationToken))
        {
            throw new ApplicationException($"Resource key '{key}' already exists.");
        }

        var unit = Unit.Create(
            command.UnitCode,
            command.UnitSymbol,
            command.UnitName);

        var type = command.Type switch
        {
            ResourceTypeDto.Material => ResourceType.Material,
            ResourceTypeDto.Labor => ResourceType.Labor,
            ResourceTypeDto.Equipment => ResourceType.Equipment,
            ResourceTypeDto.Tool => ResourceType.Tool,
            ResourceTypeDto.Auxiliary => ResourceType.Auxiliary,
            _ => throw new ApplicationException("Unsupported resource type.")
        };

        var resource = Resource.Create(
            key,
            command.Name,
            type,
            unit,
            Money.From(command.Price));

        await _repository.AddAsync(resource, cancellationToken);

        return new CreateResourceResult(
            resource.Id.Value,
            resource.Key.Value,
            resource.Name,
            resource.Unit.Symbol,
            resource.Price.Amount,
            resource.Status.ToString());
    }
}
