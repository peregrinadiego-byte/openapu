namespace OpenAPU.Application.Budgets;

public sealed record ChangeBudgetItemQuantityCommand(
    Guid BudgetId,
    Guid ItemId,
    decimal Quantity);

public sealed record RemoveBudgetItemCommand(
    Guid BudgetId,
    Guid ItemId);
