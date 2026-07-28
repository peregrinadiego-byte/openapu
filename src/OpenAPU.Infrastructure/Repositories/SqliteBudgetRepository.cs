using Microsoft.EntityFrameworkCore;
using OpenAPU.Application.Abstractions;
using OpenAPU.Domain;
using OpenAPU.Infrastructure.Persistence;

namespace OpenAPU.Infrastructure.Repositories;

public sealed class SqliteBudgetRepository : IBudgetRepository
{
    private readonly DbContextOptions<OpenApuDbContext> _options;

    public SqliteBudgetRepository(string databasePath)
    {
        if (string.IsNullOrWhiteSpace(databasePath))
        {
            throw new ArgumentException(
                "Database path is required.",
                nameof(databasePath));
        }

        var fullPath = Path.GetFullPath(databasePath);
        var directory = Path.GetDirectoryName(fullPath);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        _options = new DbContextOptionsBuilder<OpenApuDbContext>()
            .UseSqlite($"Data Source={fullPath};Pooling=False")
            .Options;

        using var context = CreateContext();
        context.Database.EnsureCreated();
    }

    public async Task<bool> ExistsByKeyAsync(
        Key key,
        CancellationToken cancellationToken = default)
    {
        await using var context = CreateContext();

        return await context.Budgets
            .AsNoTracking()
            .AnyAsync(row => row.Key == key.Value, cancellationToken);
    }

    public async Task<Budget?> GetByIdAsync(
        Identifier id,
        CancellationToken cancellationToken = default)
    {
        await using var context = CreateContext();

        var row = await context.Budgets
            .AsNoTracking()
            .Include(budget => budget.Items)
            .SingleOrDefaultAsync(
                budget => budget.Id == id.Value,
                cancellationToken);

        return row is null
            ? null
            : await ToDomainAsync(context, row, cancellationToken);
    }

    public async Task AddAsync(
        Budget budget,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(budget);

        await using var context = CreateContext();
        context.Budgets.Add(ToRow(budget));

        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
        {
            throw new InvalidOperationException(
                $"Budget key '{budget.Key}' already exists.",
                exception);
        }
    }

    public async Task UpdateAsync(
        Budget budget,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(budget);

        await using var context = CreateContext();

        var row = await context.Budgets
            .Include(existing => existing.Items)
            .SingleOrDefaultAsync(
                existing => existing.Id == budget.Id.Value,
                cancellationToken);

        if (row is null)
        {
            throw new InvalidOperationException(
                $"Budget '{budget.Id}' was not found.");
        }

        row.Name = budget.Name;

        context.BudgetItems.RemoveRange(row.Items);
        row.Items = budget.Items
            .Select(item => new BudgetItemRow
            {
                Id = item.Id.Value,
                BudgetId = budget.Id.Value,
                ConceptId = item.Concept.Id.Value,
                Quantity = item.Quantity.Value,
                UnitPrice = item.UnitPrice.Amount
            })
            .ToList();

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Budget>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        await using var context = CreateContext();

        var rows = await context.Budgets
            .AsNoTracking()
            .Include(budget => budget.Items)
            .OrderBy(budget => budget.Key)
            .ToArrayAsync(cancellationToken);

        var result = new List<Budget>(rows.Length);

        foreach (var row in rows)
        {
            result.Add(await ToDomainAsync(context, row, cancellationToken));
        }

        return result;
    }

    private OpenApuDbContext CreateContext() => new(_options);

    private static BudgetRow ToRow(Budget budget) => new()
    {
        Id = budget.Id.Value,
        Key = budget.Key.Value,
        Name = budget.Name,
        Items = budget.Items
            .Select(item => new BudgetItemRow
            {
                Id = item.Id.Value,
                BudgetId = budget.Id.Value,
                ConceptId = item.Concept.Id.Value,
                Quantity = item.Quantity.Value,
                UnitPrice = item.UnitPrice.Amount
            })
            .ToList()
    };

    private static async Task<Budget> ToDomainAsync(
        OpenApuDbContext context,
        BudgetRow row,
        CancellationToken cancellationToken)
    {
        var snapshots = new List<BudgetItemSnapshot>();

        foreach (var item in row.Items)
        {
            var conceptRow = await context.Concepts
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    concept => concept.Id == item.ConceptId,
                    cancellationToken);

            if (conceptRow is null)
            {
                throw new InvalidOperationException(
                    $"Concept '{item.ConceptId}' was not found.");
            }

            var conceptRepository = new SqliteConceptRepository(
                context.Database.GetDbConnection().DataSource);

            var concept = await conceptRepository.GetByIdAsync(
                Identifier.From(item.ConceptId),
                cancellationToken);

            if (concept is null)
            {
                throw new InvalidOperationException(
                    $"Concept '{item.ConceptId}' was not found.");
            }

            snapshots.Add(new BudgetItemSnapshot(
                Identifier.From(item.Id),
                concept,
                Quantity.From(item.Quantity),
                Money.From(item.UnitPrice)));
        }

        return Budget.Rehydrate(
            Identifier.From(row.Id),
            Key.From(row.Key),
            row.Name,
            snapshots);
    }
}
