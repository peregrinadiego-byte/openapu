using Microsoft.EntityFrameworkCore;
using OpenAPU.Application.Abstractions;
using OpenAPU.Domain;
using OpenAPU.Infrastructure.Persistence;

namespace OpenAPU.Infrastructure.Repositories;

public sealed class SqliteApuRepository : IApuRepository
{
    private readonly DbContextOptions<OpenApuDbContext> _options;

    public SqliteApuRepository(string databasePath)
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

        return await context.Apus
            .AsNoTracking()
            .AnyAsync(row => row.Key == key.Value, cancellationToken);
    }

    public async Task<Apu?> GetByIdAsync(
        Identifier id,
        CancellationToken cancellationToken = default)
    {
        await using var context = CreateContext();

        var row = await context.Apus
            .AsNoTracking()
            .Include(apu => apu.Components)
            .SingleOrDefaultAsync(
                apu => apu.Id == id.Value,
                cancellationToken);

        return row is null
            ? null
            : await ToDomainAsync(context, row, cancellationToken);
    }

    public async Task AddAsync(
        Apu apu,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(apu);

        await using var context = CreateContext();

        context.Apus.Add(ToRow(apu));

        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
        {
            throw new InvalidOperationException(
                $"APU key '{apu.Key}' already exists.",
                exception);
        }
    }

    public async Task UpdateAsync(
        Apu apu,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(apu);

        await using var context = CreateContext();

        var row = await context.Apus
            .Include(existing => existing.Components)
            .SingleOrDefaultAsync(
                existing => existing.Id == apu.Id.Value,
                cancellationToken);

        if (row is null)
        {
            throw new InvalidOperationException(
                $"APU '{apu.Id}' was not found.");
        }

        row.Name = apu.Name;

        context.ApuComponents.RemoveRange(row.Components);
        row.Components = apu.Components
            .Select(component => new ApuComponentRow
            {
                Id = component.Id.Value,
                ApuId = apu.Id.Value,
                ResourceId = component.Resource.Id.Value,
                Quantity = component.Quantity.Value,
                UnitPrice = component.UnitPrice.Amount
            })
            .ToList();

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Apu>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        await using var context = CreateContext();

        var rows = await context.Apus
            .AsNoTracking()
            .Include(apu => apu.Components)
            .OrderBy(apu => apu.Key)
            .ToArrayAsync(cancellationToken);

        var result = new List<Apu>(rows.Length);

        foreach (var row in rows)
        {
            result.Add(
                await ToDomainAsync(
                    context,
                    row,
                    cancellationToken));
        }

        return result;
    }

    private OpenApuDbContext CreateContext() => new(_options);

    private static ApuRow ToRow(Apu apu) => new()
    {
        Id = apu.Id.Value,
        Key = apu.Key.Value,
        Name = apu.Name,
        UnitCode = apu.Unit.Code,
        UnitSymbol = apu.Unit.Symbol,
        UnitName = apu.Unit.Name,
        Components = apu.Components
            .Select(component => new ApuComponentRow
            {
                Id = component.Id.Value,
                ApuId = apu.Id.Value,
                ResourceId = component.Resource.Id.Value,
                Quantity = component.Quantity.Value,
                UnitPrice = component.UnitPrice.Amount
            })
            .ToList()
    };

    private static async Task<Apu> ToDomainAsync(
        OpenApuDbContext context,
        ApuRow row,
        CancellationToken cancellationToken)
    {
        var resourceIds = row.Components
            .Select(component => component.ResourceId)
            .Distinct()
            .ToArray();

        var resources = await context.Resources
            .AsNoTracking()
            .Where(resource => resourceIds.Contains(resource.Id))
            .ToDictionaryAsync(
                resource => resource.Id,
                cancellationToken);

        var snapshots = row.Components.Select(component =>
        {
            if (!resources.TryGetValue(component.ResourceId, out var resourceRow))
            {
                throw new InvalidOperationException(
                    $"Resource '{component.ResourceId}' was not found.");
            }

            return new ApuComponentSnapshot(
                Identifier.From(component.Id),
                ToResource(resourceRow),
                Quantity.From(component.Quantity),
                Money.From(component.UnitPrice));
        });

        return Apu.Rehydrate(
            Identifier.From(row.Id),
            Key.From(row.Key),
            row.Name,
            Unit.Create(
                row.UnitCode,
                row.UnitSymbol,
                row.UnitName),
            snapshots);
    }

    private static Resource ToResource(ResourceRow row)
    {
        if (!Enum.TryParse<ResourceType>(row.Type, out var type))
        {
            throw new InvalidOperationException(
                $"Stored resource type '{row.Type}' is invalid.");
        }

        if (!Enum.TryParse<ResourceStatus>(row.Status, out var status))
        {
            throw new InvalidOperationException(
                $"Stored resource status '{row.Status}' is invalid.");
        }

        return Resource.Rehydrate(
            Identifier.From(row.Id),
            Key.From(row.Key),
            row.Name,
            type,
            Unit.Create(
                row.UnitCode,
                row.UnitSymbol,
                row.UnitName),
            Money.From(row.Price),
            status);
    }
}
