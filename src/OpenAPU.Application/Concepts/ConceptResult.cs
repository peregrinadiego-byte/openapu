namespace OpenAPU.Application.Concepts;

public sealed record ConceptResult(
    Guid Id,
    string Key,
    string Name,
    string Unit,
    Guid ApuId,
    decimal DirectCost,
    decimal IndirectCost,
    decimal Financing,
    decimal Profit,
    decimal AdditionalCharges,
    decimal UnitPrice);
