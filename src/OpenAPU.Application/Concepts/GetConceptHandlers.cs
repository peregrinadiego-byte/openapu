using OpenAPU.Application.Abstractions;
using OpenAPU.Domain;

namespace OpenAPU.Application.Concepts;

public sealed class GetConceptHandler
{
    private readonly IConceptRepository _repository;

    public GetConceptHandler(IConceptRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<ConceptResult> HandleAsync(
        Guid conceptId,
        CancellationToken cancellationToken = default)
    {
        var id = Identifier.From(conceptId);
        var concept = await _repository.GetByIdAsync(id, cancellationToken);

        if (concept is null)
        {
            throw new OpenAPU.Application.ApplicationException(
                $"Concept '{id}' was not found.");
        }

        return CreateConceptHandler.Map(concept);
    }
}

public sealed class GetConceptsHandler
{
    private readonly IConceptRepository _repository;

    public GetConceptsHandler(IConceptRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<IReadOnlyCollection<ConceptResult>> HandleAsync(
        CancellationToken cancellationToken = default)
    {
        var concepts = await _repository.GetAllAsync(cancellationToken);

        return concepts
            .OrderBy(concept => concept.Key.Value)
            .Select(CreateConceptHandler.Map)
            .ToArray();
    }
}
