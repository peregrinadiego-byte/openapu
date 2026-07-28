namespace OpenAPU.Application.Budgets;

public sealed record AddBudgetItemCommand(
    Guid BudgetId,
    Guid ConceptId,
    decimal Quantity);
