using OpenAPU.Application.Abstractions;

namespace OpenAPU.Application.Resources;

public sealed class GetResourcesHandler
{
    private readonly IResourceRepository _repository;

    public GetResourcesHandler(IResourceRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<IReadOnlyCollection<ResourceListItem>> HandleAsync(
        CancellationToken cancellationToken = default)
    {
        var resources = await _repository.GetAllAsync(cancellationToken);

        return resources
            .OrderBy(resource => resource.Key.Value)
            .Select(resource => new ResourceListItem(
                resource.Id.Value,
                resource.Key.Value,
                resource.Name,
                resource.Type.ToString(),
                resource.Unit.Symbol,
                resource.Price.Amount,
                resource.Status.ToString()))
            .ToArray();
    }
}
