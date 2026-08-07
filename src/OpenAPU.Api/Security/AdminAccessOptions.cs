namespace OpenAPU.Api.Security;

public sealed record AdminAccessOptions(
    string? Key)
{
    public bool Enabled =>
        !string.IsNullOrWhiteSpace(Key);
}
