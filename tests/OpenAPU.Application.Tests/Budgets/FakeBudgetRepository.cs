using OpenAPU.Application.Abstractions;
using OpenAPU.Domain;

namespace OpenAPU.Application.Tests.Budgets;

internal sealed class FakeBudgetRepository : IBudgetRepository
{
    public List<Budget> Budgets { get; } = [];

    public Task<bool> ExistsByKeyAsync(
        Key key,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            Budgets.Any(budget => budget.Key == key));
    }

    public Task<Budget?> GetByIdAsync(
        Identifier id,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            Budgets.SingleOrDefault(budget => budget.Id == id));
    }

    public Task AddAsync(
        Budget budget,
        CancellationToken cancellationToken = default)
    {
        Budgets.Add(budget);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(
        Budget budget,
        CancellationToken cancellationToken = default)
    {
        var index = Budgets.FindIndex(existing => existing.Id == budget.Id);

        if (index < 0)
        {
            throw new InvalidOperationException("Budget was not found.");
        }

        Budgets[index] = budget;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<Budget>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Budget> result = Budgets.ToArray();
        return Task.FromResult(result);
    }
}
