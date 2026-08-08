using System.Net;
using System.Text;

namespace OpenAPU.Api.Tests;

public sealed class BackupIntegrityTests :
    IClassFixture<OpenApuApiFactory>
{
    private readonly HttpClient _client;

    public BackupIntegrityTests(OpenApuApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Backup_endpoint_returns_valid_sqlite_file()
    {
        var response = await _client.GetAsync(
            "/database/backup");

        var bytes = await response.Content
            .ReadAsByteArrayAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(bytes.Length > 100);

        var header = Encoding.ASCII.GetString(
            bytes,
            0,
            Math.Min(16, bytes.Length));

        Assert.StartsWith("SQLite format 3", header);
    }
}

