using Microsoft.Data.Sqlite;

namespace OpenAPU.Infrastructure.Backup;

public sealed record DatabaseRestoreResult(
    bool Restored,
    long Bytes,
    DateTimeOffset RestoredAtUtc);

public sealed class SqliteDatabaseTransferService
{
    private static readonly string[] RequiredTables =
    [
        "resources",
        "apus",
        "apu_components",
        "concepts",
        "budgets",
        "budget_items",
        "__EFMigrationsHistory"
    ];

    private readonly string _databasePath;

    public SqliteDatabaseTransferService(string databasePath)
    {
        if (string.IsNullOrWhiteSpace(databasePath))
        {
            throw new ArgumentException(
                "Database path is required.",
                nameof(databasePath));
        }

        _databasePath = Path.GetFullPath(databasePath);

        var directory = Path.GetDirectoryName(_databasePath);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    public async Task<byte[]> CreateBackupAsync(
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_databasePath))
        {
            throw new InvalidOperationException(
                "The OpenAPU database does not exist.");
        }

        var temporaryPath = CreateTemporaryPath();

        try
        {
            await using var source = CreateConnection(_databasePath);
            await using var destination = CreateConnection(temporaryPath);

            await source.OpenAsync(cancellationToken);
            await destination.OpenAsync(cancellationToken);

            source.BackupDatabase(destination);

            await source.CloseAsync();
            await destination.CloseAsync();

            return await File.ReadAllBytesAsync(
                temporaryPath,
                cancellationToken);
        }
        finally
        {
            DeleteTemporaryDatabase(temporaryPath);
        }
    }

    public async Task<DatabaseRestoreResult> RestoreAsync(
        Stream sourceStream,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sourceStream);

        var temporaryPath = CreateTemporaryPath();

        try
        {
            await using (var destination = File.Create(temporaryPath))
            {
                await sourceStream.CopyToAsync(
                    destination,
                    cancellationToken);
            }

            var bytes = new FileInfo(temporaryPath).Length;

            if (bytes < 100)
            {
                throw new InvalidDataException(
                    "The backup file is empty or incomplete.");
            }

            await ValidateDatabaseAsync(
                temporaryPath,
                cancellationToken);

            await using var source = CreateConnection(temporaryPath);
            await using var destinationDatabase =
                CreateConnection(_databasePath);

            await source.OpenAsync(cancellationToken);
            await destinationDatabase.OpenAsync(cancellationToken);

            source.BackupDatabase(destinationDatabase);

            await source.CloseAsync();
            await destinationDatabase.CloseAsync();

            return new DatabaseRestoreResult(
                true,
                bytes,
                DateTimeOffset.UtcNow);
        }
        finally
        {
            DeleteTemporaryDatabase(temporaryPath);
        }
    }

    private static async Task ValidateDatabaseAsync(
        string databasePath,
        CancellationToken cancellationToken)
    {
        try
        {
            await using var connection =
                CreateConnection(databasePath);

            await connection.OpenAsync(cancellationToken);

            await using var command = connection.CreateCommand();
            command.CommandText =
                """
                SELECT name
                FROM sqlite_master
                WHERE type = 'table';
                """;

            var tables = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);

            await using var reader =
                await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                tables.Add(reader.GetString(0));
            }

            var missing = RequiredTables
                .Where(table => !tables.Contains(table))
                .ToArray();

            if (missing.Length > 0)
            {
                throw new InvalidDataException(
                    $"The file is not a valid OpenAPU backup. Missing tables: {string.Join(", ", missing)}.");
            }
        }
        catch (SqliteException exception)
        {
            throw new InvalidDataException(
                "The uploaded file is not a valid SQLite database.",
                exception);
        }
    }

    private static SqliteConnection CreateConnection(
        string databasePath)
    {
        return new SqliteConnection(
            $"Data Source={databasePath};Pooling=False");
    }

    private static string CreateTemporaryPath()
    {
        return Path.Combine(
            Path.GetTempPath(),
            $"openapu-backup-{Guid.NewGuid():N}.db");
    }

    private static void DeleteTemporaryDatabase(
        string databasePath)
    {
        foreach (var path in new[]
        {
            databasePath,
            $"{databasePath}-shm",
            $"{databasePath}-wal"
        })
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
