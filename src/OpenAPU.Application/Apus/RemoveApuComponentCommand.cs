namespace OpenAPU.Application.Apus;

public sealed record RemoveApuComponentCommand(
    Guid ApuId,
    Guid ComponentId);
