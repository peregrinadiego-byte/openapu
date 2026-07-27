using OpenAPU.Application.Abstractions;
using OpenAPU.Domain;

namespace OpenAPU.Application.Resources;

public sealed class UpdateResourceHandler
{
    private readonly IResourceRepository _repository;

    public UpdateResourceHandler(IResourceRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<CreateResourceResult> HandleAsync(
        UpdateResourceCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var id = Identifier.From(command.Id);
        var resource = await _repository.GetByIdAsync(id, cancellationToken);

        if (resource is null)
        {
            throw new OpenAPU.Application.ApplicationException(
                $"Resource '{id}' was not found.");
        }

        resource.Rename(command.Name);
        resource.ChangePrice(Money.From(command.Price));

        if (command.IsActive)
        {
            resource.Activate();
        }
        else
        {
            resource.Deactivate();
        }

        await _repository.UpdateAsync(resource, cancellationToken);

        return new CreateResourceResult(
            resource.Id.Value,
            resource.Key.Value,
            resource.Name,
            resource.Unit.Symbol,
            resource.Price.Amount,
            resource.Status.ToString());
    }
}
