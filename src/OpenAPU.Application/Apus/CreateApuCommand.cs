namespace OpenAPU.Application.Apus;

public sealed record CreateApuCommand(
    string Key,
    string Name,
    string UnitCode,
    string UnitSymbol,
    string UnitName);
