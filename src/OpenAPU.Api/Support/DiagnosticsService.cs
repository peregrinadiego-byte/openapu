using System.Reflection;

using OpenAPU.Api.Configuration;
using OpenAPU.Application.Abstractions;

namespace OpenAPU.Api.Support;

public sealed class DiagnosticsService
{
    private readonly IResourceRepository _resourceRepository;
    private readonly IApuRepository _apuRepository;
    private readonly IConceptRepository _conceptRepository;
    private readonly IBudgetRepository _budgetRepository;
    private readonly DatabaseStartupStatus _databaseStatus;

    public DiagnosticsService(
        IResourceRepository resourceRepository,
        IApuRepository apuRepository,
        IConceptRepository conceptRepository,
        IBudgetRepository budgetRepository,
        DatabaseStartupStatus databaseStatus)
    {
        _resourceRepository = resourceRepository;
        _apuRepository = apuRepository;
        _conceptRepository = conceptRepository;
        _budgetRepository = budgetRepository;
        _databaseStatus = databaseStatus;
    }

    public async Task<object> CreateAsync(
        CancellationToken cancellationToken)
    {
        var resources = await _resourceRepository
            .GetAllAsync(cancellationToken);

        var apus = await _apuRepository
            .GetAllAsync(cancellationToken);

        var concepts = await _conceptRepository
            .GetAllAsync(cancellationToken);

        var budgets = await _budgetRepository
            .GetAllAsync(cancellationToken);

        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly
            .GetName()
            .Version?
            .ToString(3) ?? "unknown";

        return new
        {
            product = "OpenAPU",
            version,
            generatedAtUtc = DateTimeOffset.UtcNow,
            runtime = new
            {
                framework = Environment.Version.ToString(),
                operatingSystem = Environment.OSVersion.ToString(),
                processArchitecture =
                    System.Runtime.InteropServices
                        .RuntimeInformation
                        .ProcessArchitecture
                        .ToString()
            },
            database = new
            {
                ready =
                    _databaseStatus.DirectoryExists &&
                    _databaseStatus.DirectoryWritable,
                path = _databaseStatus.Path,
                directory = _databaseStatus.Directory,
                writable = _databaseStatus.DirectoryWritable
            },
            counts = new
            {
                resources = resources.Count,
                apus = apus.Count,
                concepts = concepts.Count,
                budgets = budgets.Count
            }
        };
    }
}
