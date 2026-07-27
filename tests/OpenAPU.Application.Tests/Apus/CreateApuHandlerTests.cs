using OpenAPU.Application.Apus;

namespace OpenAPU.Application.Tests.Apus;

public sealed class CreateApuHandlerTests
{
    [Fact]
    public async Task Creates_empty_apu()
    {
        var repository = new FakeApuRepository();
        var handler = new CreateApuHandler(repository);

        var result = await handler.HandleAsync(
            new CreateApuCommand(
                "APU-001",
                "Muro de block",
                "M2",
                "m²",
                "Metro cuadrado"));

        Assert.Equal("APU-001", result.Key);
        Assert.Equal("Muro de block", result.Name);
        Assert.Equal("m²", result.Unit);
        Assert.Equal(0m, result.DirectCost);
        Assert.Equal(0, result.ComponentCount);
        Assert.Single(repository.Apus);
    }

    [Fact]
    public async Task Rejects_duplicate_key()
    {
        var repository = new FakeApuRepository();
        var handler = new CreateApuHandler(repository);

        var command = new CreateApuCommand(
            "APU-001",
            "Muro",
            "M2",
            "m²",
            "Metro cuadrado");

        await handler.HandleAsync(command);

        await Assert.ThrowsAsync<OpenAPU.Application.ApplicationException>(
            () => handler.HandleAsync(command));
    }
}
