using OpenAPU.Application.Abstractions;
using OpenAPU.Domain;

namespace OpenAPU.Application.Budgets;

public sealed class AddBudgetItemHandler
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IConceptRepository _conceptRepository;

    public AddBudgetItemHandler(
        IBudgetRepository budgetRepository,
        IConceptRepository conceptRepository)
    {
        _budgetRepository = budgetRepository
            ?? throw new ArgumentNullException(nameof(budgetRepository));

        _conceptRepository = conceptRepository
            ?? throw new ArgumentNullException(nameof(conceptRepository));
    }

    public async Task<BudgetResult> HandleAsync(
        AddBudgetItemCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var budgetId = Identifier.From(command.BudgetId);
        var conceptId = Identifier.From(command.ConceptId);

        var budget = await _budgetRepository.GetByIdAsync(
            budgetId,
            cancellationToken);

        if (budget is null)
        {
            throw new OpenAPU.Application.ApplicationException(
                $"Budget '{budgetId}' was not found.");
        }

        var concept = await _conceptRepository.GetByIdAsync(
            conceptId,
            cancellationToken);

        if (concept is null)
        {
            throw new OpenAPU.Application.ApplicationException(
                $"Concept '{conceptId}' was not found.");
        }

        budget.AddItem(
            concept,
            Quantity.From(command.Quantity));

        await _budgetRepository.UpdateAsync(
            budget,
            cancellationToken);

        return CreateBudgetHandler.Map(budget);
    }
}
