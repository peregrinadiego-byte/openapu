using OpenAPU.Application.Abstractions;

namespace OpenAPU.Application.Apus;

public sealed class GetApusHandler
{
    private readonly IApuRepository _repository;

    public GetApusHandler(IApuRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<IReadOnlyCollection<ApuResult>> HandleAsync(
        CancellationToken cancellationToken = default)
    {
        var apus = await _repository.GetAllAsync(cancellationToken);

        return apus
            .OrderBy(apu => apu.Key.Value)
            .Select(CreateApuHandler.Map)
            .ToArray();
    }
}
