using OpenAPU.Application.Abstractions;
using OpenAPU.Domain;

namespace OpenAPU.Application.Budgets;

public sealed class GetBudgetHandler
{
    private readonly IBudgetRepository _repository;

    public GetBudgetHandler(IBudgetRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<BudgetResult> HandleAsync(
        Guid budgetId,
        CancellationToken cancellationToken = default)
    {
        var id = Identifier.From(budgetId);
        var budget = await _repository.GetByIdAsync(id, cancellationToken);

        if (budget is null)
        {
            throw new OpenAPU.Application.ApplicationException(
                $"Budget '{id}' was not found.");
        }

        return CreateBudgetHandler.Map(budget);
    }
}

public sealed class GetBudgetsHandler
{
    private readonly IBudgetRepository _repository;

    public GetBudgetsHandler(IBudgetRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<IReadOnlyCollection<BudgetResult>> HandleAsync(
        CancellationToken cancellationToken = default)
    {
        var budgets = await _repository.GetAllAsync(cancellationToken);

        return budgets
            .OrderBy(budget => budget.Key.Value)
            .Select(CreateBudgetHandler.Map)
            .ToArray();
    }
}
