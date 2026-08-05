using System.Net;
using Microsoft.Data.Sqlite;

namespace OpenAPU.Api.Tests;

public sealed class BackupRestoreValidationTests :
    IClassFixture<OpenApuApiFactory>
{
    private readonly HttpClient _client;

    public BackupRestoreValidationTests(OpenApuApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Backup_passes_sqlite_integrity_check()
    {
        var response = await _client.GetAsync(
            "/database/backup");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var bytes = await response.Content
            .ReadAsByteArrayAsync();

        var path = Path.Combine(
            Path.GetTempPath(),
            $"openapu-backup-{Guid.NewGuid():N}.db");

        try
        {
            await File.WriteAllBytesAsync(path, bytes);

            await ValidateIntegrityAsync(path);
        }
        finally
        {
            SqliteConnection.ClearAllPools();

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }

    private static async Task ValidateIntegrityAsync(
        string path)
    {
        await using var connection =
            new SqliteConnection(
                $"Data Source={path};Mode=ReadOnly;Pooling=False");

        await connection.OpenAsync();

        await using var command =
            connection.CreateCommand();

        command.CommandText = "PRAGMA integrity_check;";

        var result = await command
            .ExecuteScalarAsync();

        Assert.Equal("ok", result?.ToString());
    }
}
