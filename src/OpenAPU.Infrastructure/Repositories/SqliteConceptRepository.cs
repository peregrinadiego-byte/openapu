using Microsoft.EntityFrameworkCore;
using OpenAPU.Application.Abstractions;
using OpenAPU.Domain;
using OpenAPU.Infrastructure.Persistence;

namespace OpenAPU.Infrastructure.Repositories;

public sealed class SqliteConceptRepository : IConceptRepository
{
    private readonly DbContextOptions<OpenApuDbContext> _options;

    public SqliteConceptRepository(string databasePath)
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
        context.Database.Migrate();
    }

    public async Task<bool> ExistsByKeyAsync(
        Key key,
        CancellationToken cancellationToken = default)
    {
        await using var context = CreateContext();

        return await context.Concepts
            .AsNoTracking()
            .AnyAsync(row => row.Key == key.Value, cancellationToken);
    }

    public async Task<Concept?> GetByIdAsync(
        Identifier id,
        CancellationToken cancellationToken = default)
    {
        await using var context = CreateContext();

        var row = await context.Concepts
            .AsNoTracking()
            .SingleOrDefaultAsync(
                concept => concept.Id == id.Value,
                cancellationToken);

        return row is null
            ? null
            : await ToDomainAsync(context, row, cancellationToken);
    }

    public async Task AddAsync(
        Concept concept,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(concept);

        await using var context = CreateContext();

        context.Concepts.Add(ToRow(concept));

        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
        {
            throw new InvalidOperationException(
                $"Concept key '{concept.Key}' already exists.",
                exception);
        }
    }

    public async Task UpdateAsync(
        Concept concept,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(concept);

        await using var context = CreateContext();

        var row = await context.Concepts
            .SingleOrDefaultAsync(
                existing => existing.Id == concept.Id.Value,
                cancellationToken);

        if (row is null)
        {
            throw new InvalidOperationException(
                $"Concept '{concept.Id}' was not found.");
        }

        row.Name = concept.Name;
        row.IndirectCost = concept.IndirectCost.Value;
        row.Financing = concept.Financing.Value;
        row.Profit = concept.Profit.Value;
        row.AdditionalCharges = concept.AdditionalCharges.Value;

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Concept>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        await using var context = CreateContext();

        var rows = await context.Concepts
            .AsNoTracking()
            .OrderBy(row => row.Key)
            .ToArrayAsync(cancellationToken);

        var result = new List<Concept>(rows.Length);

        foreach (var row in rows)
        {
            result.Add(await ToDomainAsync(context, row, cancellationToken));
        }

        return result;
    }

    private OpenApuDbContext CreateContext() => new(_options);

    private static ConceptRow ToRow(Concept concept) => new()
    {
        Id = concept.Id.Value,
        Key = concept.Key.Value,
        Name = concept.Name,
        UnitCode = concept.Unit.Code,
        UnitSymbol = concept.Unit.Symbol,
        UnitName = concept.Unit.Name,
        ApuId = concept.Apu.Id.Value,
        IndirectCost = concept.IndirectCost.Value,
        Financing = concept.Financing.Value,
        Profit = concept.Profit.Value,
        AdditionalCharges = concept.AdditionalCharges.Value
    };

    private static async Task<Concept> ToDomainAsync(
        OpenApuDbContext context,
        ConceptRow row,
        CancellationToken cancellationToken)
    {
        var apuRow = await context.Apus
            .AsNoTracking()
            .Include(apu => apu.Components)
            .SingleOrDefaultAsync(
                apu => apu.Id == row.ApuId,
                cancellationToken);

        if (apuRow is null)
        {
            throw new InvalidOperationException(
                $"APU '{row.ApuId}' was not found.");
        }

        var apu = await SqliteApuMapper.ToDomainAsync(
            context,
            apuRow,
            cancellationToken);

        return Concept.Rehydrate(
            Identifier.From(row.Id),
            Key.From(row.Key),
            row.Name,
            Unit.Create(row.UnitCode, row.UnitSymbol, row.UnitName),
            apu,
            Percentage.From(row.IndirectCost),
            Percentage.From(row.Financing),
            Percentage.From(row.Profit),
            Percentage.From(row.AdditionalCharges));
    }
}

