using OpenAPU.Application.Abstractions;
using OpenAPU.Domain;

namespace OpenAPU.Application.Concepts;

public sealed class UpdateConceptPercentagesHandler
{
    private readonly IConceptRepository _repository;

    public UpdateConceptPercentagesHandler(IConceptRepository repository)
    {
        _repository = repository
            ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<ConceptResult> HandleAsync(
        UpdateConceptPercentagesCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var id = Identifier.From(command.ConceptId);
        var concept = await _repository.GetByIdAsync(id, cancellationToken);

        if (concept is null)
        {
            throw new OpenAPU.Application.ApplicationException(
                $"Concept '{id}' was not found.");
        }

        concept.SetIndirectCost(
            Percentage.From(command.IndirectCost));

        concept.SetFinancing(
            Percentage.From(command.Financing));

        concept.SetProfit(
            Percentage.From(command.Profit));

        concept.SetAdditionalCharges(
            Percentage.From(command.AdditionalCharges));

        await _repository.UpdateAsync(concept, cancellationToken);

        return CreateConceptHandler.Map(concept);
    }
}
