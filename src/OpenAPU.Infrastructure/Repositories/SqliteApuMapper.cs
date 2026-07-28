using Microsoft.EntityFrameworkCore;
using OpenAPU.Domain;
using OpenAPU.Infrastructure.Persistence;

namespace OpenAPU.Infrastructure.Repositories;

internal static class SqliteApuMapper
{
    public static async Task<Apu> ToDomainAsync(
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
            Unit.Create(row.UnitCode, row.UnitSymbol, row.UnitName),
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
