namespace OpenAPU.Application.Concepts;

public sealed record CreateConceptCommand(
    string Key,
    string Name,
    string UnitCode,
    string UnitSymbol,
    string UnitName,
    Guid ApuId);
