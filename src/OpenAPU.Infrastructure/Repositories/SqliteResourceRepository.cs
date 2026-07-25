using Microsoft.EntityFrameworkCore;
using OpenAPU.Application.Abstractions;
using OpenAPU.Domain;
using OpenAPU.Infrastructure.Persistence;

namespace OpenAPU.Infrastructure.Repositories;

public sealed class SqliteResourceRepository : IResourceRepository
{
    private readonly DbContextOptions<OpenApuDbContext> _options;

    public SqliteResourceRepository(string databasePath)
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

        return await context.Resources
            .AsNoTracking()
            .AnyAsync(row => row.Key == key.Value, cancellationToken);
    }

    public async Task AddAsync(
        Resource resource,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(resource);

        await using var context = CreateContext();

        context.Resources.Add(new ResourceRow
        {
            Id = resource.Id.Value,
            Key = resource.Key.Value,
            Name = resource.Name,
            Type = resource.Type.ToString(),
            UnitCode = resource.Unit.Code,
            UnitSymbol = resource.Unit.Symbol,
            UnitName = resource.Unit.Name,
            Price = resource.Price.Amount,
            Status = resource.Status.ToString()
        });

        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
        {
            throw new InvalidOperationException(
                $"Resource key '{resource.Key}' already exists.",
                exception);
        }
    }

    public async Task<IReadOnlyCollection<Resource>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        await using var context = CreateContext();

        var rows = await context.Resources
            .AsNoTracking()
            .OrderBy(row => row.Key)
            .ToArrayAsync(cancellationToken);

        return rows.Select(ToDomain).ToArray();
    }

    private OpenApuDbContext CreateContext() => new(_options);

    private static Resource ToDomain(ResourceRow row)
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
            Unit.Create(row.UnitCode, row.UnitSymbol, row.UnitName),
            Money.From(row.Price),
            status);
    }
}

