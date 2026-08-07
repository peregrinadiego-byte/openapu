namespace OpenAPU.Application.Resources;

public sealed record CreateResourceResult(
    Guid Id,
    string Key,
    string Name,
    string Unit,
    decimal Price,
    string Status);
