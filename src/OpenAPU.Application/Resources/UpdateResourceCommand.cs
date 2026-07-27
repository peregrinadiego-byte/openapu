namespace OpenAPU.Application.Resources;

public sealed record UpdateResourceCommand(
    Guid Id,
    string Name,
    decimal Price,
    bool IsActive);
