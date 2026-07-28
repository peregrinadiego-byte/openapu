using OpenAPU.Application.Abstractions;
using OpenAPU.Domain;

namespace OpenAPU.Application.Resources;

public sealed class GetResourceHandler
{
    private readonly IResourceRepository _repository;

    public GetResourceHandler(IResourceRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<ResourceListItem> HandleAsync(
        Guid resourceId,
        CancellationToken cancellationToken = default)
    {
        var id = Identifier.From(resourceId);
        var resource = await _repository.GetByIdAsync(id, cancellationToken);

        if (resource is null)
        {
            throw new OpenAPU.Application.ApplicationException(
                $"Resource '{id}' was not found.");
        }

        return new ResourceListItem(
            resource.Id.Value,
            resource.Key.Value,
            resource.Name,
            resource.Type.ToString(),
            resource.Unit.Symbol,
            resource.Price.Amount,
            resource.Status.ToString());
    }
}
