namespace OpenAPU.Application.Resources;

public sealed record ResourceListItem(
    Guid Id,
    string Key,
    string Name,
    string Type,
    string Unit,
    decimal Price,
    string Status);
