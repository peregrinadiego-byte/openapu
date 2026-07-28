namespace OpenAPU.Application.Budgets;

public sealed record BudgetItemResult(
    Guid Id,
    Guid ConceptId,
    string ConceptKey,
    string ConceptName,
    decimal Quantity,
    decimal UnitPrice,
    decimal Total);

public sealed record BudgetResult(
    Guid Id,
    string Key,
    string Name,
    decimal Total,
    IReadOnlyCollection<BudgetItemResult> Items);
