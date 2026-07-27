namespace OpenAPU.Application.Apus;

public sealed record ChangeApuComponentQuantityCommand(
    Guid ApuId,
    Guid ComponentId,
    decimal Quantity);
