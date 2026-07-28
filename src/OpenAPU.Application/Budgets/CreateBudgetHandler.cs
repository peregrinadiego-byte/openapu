using OpenAPU.Application.Abstractions;
using OpenAPU.Domain;

namespace OpenAPU.Application.Budgets;

public sealed class CreateBudgetHandler
{
    private readonly IBudgetRepository _repository;

    public CreateBudgetHandler(IBudgetRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<BudgetResult> HandleAsync(
        CreateBudgetCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var key = Key.From(command.Key);

        if (await _repository.ExistsByKeyAsync(key, cancellationToken))
        {
            throw new OpenAPU.Application.ApplicationException(
                $"Budget key '{key}' already exists.");
        }

        var budget = Budget.Create(key, command.Name);

        await _repository.AddAsync(budget, cancellationToken);

        return Map(budget);
    }

    internal static BudgetResult Map(Budget budget) => new(
        budget.Id.Value,
        budget.Key.Value,
        budget.Name,
        budget.Total.Amount,
        budget.Items
            .Select(item => new BudgetItemResult(
                item.Id.Value,
                item.Concept.Id.Value,
                item.Concept.Key.Value,
                item.Concept.Name,
                item.Quantity.Value,
                item.UnitPrice.Amount,
                item.Total.Amount))
            .ToArray());
}
