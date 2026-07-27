namespace OpenAPU.Application.Apus;

public sealed record ApuComponentResult(
    Guid Id,
    Guid ResourceId,
    string ResourceKey,
    string ResourceName,
    decimal Quantity,
    decimal UnitPrice,
    decimal Total);

public sealed record ApuDetailResult(
    Guid Id,
    string Key,
    string Name,
    string Unit,
    decimal DirectCost,
    IReadOnlyCollection<ApuComponentResult> Components);
