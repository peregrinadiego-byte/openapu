using OpenAPU.Application.Abstractions;
using OpenAPU.Domain;

namespace OpenAPU.Application.Budgets;

public sealed class ChangeBudgetItemQuantityHandler
{
    private readonly IBudgetRepository _repository;

    public ChangeBudgetItemQuantityHandler(IBudgetRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<BudgetResult> HandleAsync(
        ChangeBudgetItemQuantityCommand command,
        CancellationToken cancellationToken = default)
    {
        var id = Identifier.From(command.BudgetId);
        var budget = await _repository.GetByIdAsync(id, cancellationToken);

        if (budget is null)
        {
            throw new OpenAPU.Application.ApplicationException(
                $"Budget '{id}' was not found.");
        }

        budget.ChangeQuantity(
            Identifier.From(command.ItemId),
            Quantity.From(command.Quantity));

        await _repository.UpdateAsync(budget, cancellationToken);

        return CreateBudgetHandler.Map(budget);
    }
}

public sealed class RemoveBudgetItemHandler
{
    private readonly IBudgetRepository _repository;

    public RemoveBudgetItemHandler(IBudgetRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<BudgetResult> HandleAsync(
        RemoveBudgetItemCommand command,
        CancellationToken cancellationToken = default)
    {
        var id = Identifier.From(command.BudgetId);
        var budget = await _repository.GetByIdAsync(id, cancellationToken);

        if (budget is null)
        {
            throw new OpenAPU.Application.ApplicationException(
                $"Budget '{id}' was not found.");
        }

        budget.RemoveItem(Identifier.From(command.ItemId));

        await _repository.UpdateAsync(budget, cancellationToken);

        return CreateBudgetHandler.Map(budget);
    }
}

public sealed class RefreshBudgetPricesHandler
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IConceptRepository _conceptRepository;

    public RefreshBudgetPricesHandler(
        IBudgetRepository budgetRepository,
        IConceptRepository conceptRepository)
    {
        _budgetRepository = budgetRepository
            ?? throw new ArgumentNullException(nameof(budgetRepository));

        _conceptRepository = conceptRepository
            ?? throw new ArgumentNullException(nameof(conceptRepository));
    }

    public async Task<BudgetResult> HandleAsync(
        Guid budgetId,
        CancellationToken cancellationToken = default)
    {
        var id = Identifier.From(budgetId);
        var budget = await _budgetRepository.GetByIdAsync(id, cancellationToken);

        if (budget is null)
        {
            throw new OpenAPU.Application.ApplicationException(
                $"Budget '{id}' was not found.");
        }

        var snapshots = new List<BudgetItemSnapshot>();

        foreach (var item in budget.Items)
        {
            var concept = await _conceptRepository.GetByIdAsync(
                item.Concept.Id,
                cancellationToken);

            if (concept is null)
            {
                throw new OpenAPU.Application.ApplicationException(
                    $"Concept '{item.Concept.Id}' was not found.");
            }

            snapshots.Add(new BudgetItemSnapshot(
                item.Id,
                concept,
                item.Quantity,
                concept.UnitPrice));
        }

        var refreshed = Budget.Rehydrate(
            budget.Id,
            budget.Key,
            budget.Name,
            snapshots);

        await _budgetRepository.UpdateAsync(refreshed, cancellationToken);

        return CreateBudgetHandler.Map(refreshed);
    }
}
