using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace OpenAPU.Api.Tests;

public sealed class ResourceCsvImportTests :
    IClassFixture<OpenApuApiFactory>
{
    private readonly HttpClient _client;

    public ResourceCsvImportTests(OpenApuApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Template_can_be_downloaded()
    {
        var response = await _client.GetAsync(
            "/imports/resources/template.csv");

        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("CodigoUnidad", content);
        Assert.Contains("MAT-001", content);
    }

    [Fact]
    public async Task Valid_resources_can_be_imported()
    {
        var csv =
            "Clave,Nombre,Tipo,CodigoUnidad,SimboloUnidad,NombreUnidad,Precio\r\n" +
            "IMP-001,Arena,Material,M3,m³,Metro cúbico,350.50\r\n" +
            "IMP-002,Ayudante,Mano de obra,H,h,Hora,75\r\n";

        using var content = CreateFileContent(csv);
        var response = await _client.PostAsync(
            "/imports/resources.csv",
            content);

        var json = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(json);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(
            2,
            document.RootElement
                .GetProperty("imported")
                .GetInt32());
        Assert.Equal(
            0,
            document.RootElement
                .GetProperty("rejected")
                .GetInt32());
    }

    [Fact]
    public async Task Invalid_row_is_reported_without_stopping_import()
    {
        var csv =
            "Clave,Nombre,Tipo,CodigoUnidad,SimboloUnidad,NombreUnidad,Precio\r\n" +
            "IMP-003,Equipo menor,Equipo,H,h,Hora,25\r\n" +
            "IMP-004,Desconocido,Tipo inválido,PZA,pza,Pieza,10\r\n";

        using var content = CreateFileContent(csv);
        var response = await _client.PostAsync(
            "/imports/resources.csv",
            content);

        var json = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(json);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(
            1,
            document.RootElement
                .GetProperty("imported")
                .GetInt32());
        Assert.Equal(
            1,
            document.RootElement
                .GetProperty("rejected")
                .GetInt32());
    }

    private static MultipartFormDataContent CreateFileContent(
        string csv)
    {
        var form = new MultipartFormDataContent();
        var file = new ByteArrayContent(
            Encoding.UTF8.GetBytes(csv));

        file.Headers.ContentType =
            new MediaTypeHeaderValue("text/csv");

        form.Add(file, "file", "recursos.csv");

        return form;
    }
}

