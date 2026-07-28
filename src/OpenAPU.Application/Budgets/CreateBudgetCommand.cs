namespace OpenAPU.Application.Budgets;

public sealed record CreateBudgetCommand(
    string Key,
    string Name);
