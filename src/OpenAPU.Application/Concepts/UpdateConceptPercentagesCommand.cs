namespace OpenAPU.Application.Concepts;

public sealed record UpdateConceptPercentagesCommand(
    Guid ConceptId,
    decimal IndirectCost,
    decimal Financing,
    decimal Profit,
    decimal AdditionalCharges);
