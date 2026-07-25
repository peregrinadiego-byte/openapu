namespace OpenAPU.Application.Resources;

public sealed record CreateResourceCommand(
    string Key,
    string Name,
    ResourceTypeDto Type,
    string UnitCode,
    string UnitSymbol,
    string UnitName,
    decimal Price);

public enum ResourceTypeDto
{
    Material,
    Labor,
    Equipment,
    Tool,
    Auxiliary
}
