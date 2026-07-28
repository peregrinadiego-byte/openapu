using OpenAPU.Application.Abstractions;
using OpenAPU.Domain;

namespace OpenAPU.Infrastructure.Repositories;

public sealed class InMemoryBudgetRepository : IBudgetRepository
{
    private readonly List<Budget> _budgets = [];
    private readonly object _sync = new();

    public Task<bool> ExistsByKeyAsync(
        Key key,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            return Task.FromResult(
                _budgets.Any(budget => budget.Key == key));
        }
    }

    public Task<Budget?> GetByIdAsync(
        Identifier id,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            return Task.FromResult(
                _budgets.SingleOrDefault(budget => budget.Id == id));
        }
    }

    public Task AddAsync(
        Budget budget,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(budget);
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            if (_budgets.Any(existing => existing.Key == budget.Key))
            {
                throw new InvalidOperationException(
                    $"Budget key '{budget.Key}' already exists.");
            }

            _budgets.Add(budget);
        }

        return Task.CompletedTask;
    }

    public Task UpdateAsync(
        Budget budget,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(budget);
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            var index = _budgets.FindIndex(existing => existing.Id == budget.Id);

            if (index < 0)
            {
                throw new InvalidOperationException(
                    $"Budget '{budget.Id}' was not found.");
            }

            _budgets[index] = budget;
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<Budget>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            IReadOnlyCollection<Budget> result = _budgets.ToArray();
            return Task.FromResult(result);
        }
    }
}
