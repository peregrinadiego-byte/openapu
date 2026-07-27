namespace OpenAPU.Application.Apus;

public sealed record AddApuComponentCommand(
    Guid ApuId,
    Guid ResourceId,
    decimal Quantity);
