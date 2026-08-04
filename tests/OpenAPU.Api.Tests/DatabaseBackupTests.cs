using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace OpenAPU.Api.Tests;

public sealed class DatabaseBackupTests :
    IClassFixture<OpenApuApiFactory>
{
    private readonly HttpClient _client;

    public DatabaseBackupTests(OpenApuApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Database_backup_can_be_downloaded()
    {
        var response = await _client.GetAsync("/database/backup");
        var bytes = await response.Content.ReadAsByteArrayAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(
            "application/vnd.sqlite3",
            response.Content.Headers.ContentType?.MediaType);

        var header = Encoding.ASCII.GetString(
            bytes.Take(16).ToArray());

        Assert.StartsWith("SQLite format 3", header);
    }

    [Fact]
    public async Task Valid_backup_can_be_restored()
    {
        var backup = await _client.GetByteArrayAsync(
            "/database/backup");

        using var form = CreateFileContent(
            backup,
            "openapu-backup.db");

        var response = await _client.PostAsync(
            "/database/restore",
            form);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Invalid_backup_is_rejected()
    {
        using var form = CreateFileContent(
            Encoding.UTF8.GetBytes("not a database"),
            "invalid.db");

        var response = await _client.PostAsync(
            "/database/restore",
            form);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    private static MultipartFormDataContent CreateFileContent(
        byte[] bytes,
        string fileName)
    {
        var form = new MultipartFormDataContent();
        var file = new ByteArrayContent(bytes);

        file.Headers.ContentType =
            new MediaTypeHeaderValue(
                "application/vnd.sqlite3");

        form.Add(file, "file", fileName);

        return form;
    }
}
