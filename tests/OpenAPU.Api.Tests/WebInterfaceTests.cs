using System.Net;

namespace OpenAPU.Api.Tests;

public sealed class WebInterfaceTests :
    IClassFixture<OpenApuApiFactory>
{
    private readonly HttpClient _client;

    public WebInterfaceTests(OpenApuApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Resource_interface_is_served()
    {
        var response = await _client.GetAsync("/index.html");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Nuevo recurso", content);
        Assert.Contains("Recursos registrados", content);
    }
}
