namespace OpenAPU.Application.Apus;

public sealed record ApuResult(
    Guid Id,
    string Key,
    string Name,
    string Unit,
    decimal DirectCost,
    int ComponentCount);
