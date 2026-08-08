using System.Net;

namespace OpenAPU.Api.Tests;

public sealed class WebEditingTests :
    IClassFixture<OpenApuApiFactory>
{
    private readonly HttpClient _client;

    public WebEditingTests(OpenApuApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Interface_contains_editing_actions()
    {
        var html = await _client.GetStringAsync("/index.html");
        var script = await _client.GetStringAsync("/app.js");

        Assert.Contains("Acciones", html);
        Assert.Contains("editResource", script);
        Assert.Contains("¿Eliminar este componente?", script);
        Assert.Contains("¿Eliminar esta partida?", script);
    }
}

