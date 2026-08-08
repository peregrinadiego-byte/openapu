using System.Net;

namespace OpenAPU.Api.Tests;

public sealed class FullWebInterfaceTests :
    IClassFixture<OpenApuApiFactory>
{
    private readonly HttpClient _client;

    public FullWebInterfaceTests(OpenApuApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Complete_workflow_interface_is_served()
    {
        var response = await _client.GetAsync("/index.html");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Nuevo recurso", content);
        Assert.Contains("Nuevo APU", content);
        Assert.Contains("Nuevo concepto", content);
        Assert.Contains("Nuevo presupuesto", content);
        Assert.Contains("Agregar partida", content);
    }
}

