using OpenAPU.Application.Abstractions;
using OpenAPU.Domain;

namespace OpenAPU.Application.Concepts;

public sealed class CreateConceptHandler
{
    private readonly IConceptRepository _conceptRepository;
    private readonly IApuRepository _apuRepository;

    public CreateConceptHandler(
        IConceptRepository conceptRepository,
        IApuRepository apuRepository)
    {
        _conceptRepository = conceptRepository
            ?? throw new ArgumentNullException(nameof(conceptRepository));

        _apuRepository = apuRepository
            ?? throw new ArgumentNullException(nameof(apuRepository));
    }

    public async Task<ConceptResult> HandleAsync(
        CreateConceptCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var key = Key.From(command.Key);

        if (await _conceptRepository.ExistsByKeyAsync(key, cancellationToken))
        {
            throw new OpenAPU.Application.ApplicationException(
                $"Concept key '{key}' already exists.");
        }

        var apuId = Identifier.From(command.ApuId);
        var apu = await _apuRepository.GetByIdAsync(apuId, cancellationToken);

        if (apu is null)
        {
            throw new OpenAPU.Application.ApplicationException(
                $"APU '{apuId}' was not found.");
        }

        var concept = Concept.Create(
            key,
            command.Name,
            Unit.Create(
                command.UnitCode,
                command.UnitSymbol,
                command.UnitName),
            apu);

        await _conceptRepository.AddAsync(concept, cancellationToken);

        return Map(concept);
    }

    internal static ConceptResult Map(Concept concept) => new(
        concept.Id.Value,
        concept.Key.Value,
        concept.Name,
        concept.Unit.Symbol,
        concept.Apu.Id.Value,
        concept.DirectCost.Amount,
        concept.IndirectCost.Value,
        concept.Financing.Value,
        concept.Profit.Value,
        concept.AdditionalCharges.Value,
        concept.UnitPrice.Amount);
}
