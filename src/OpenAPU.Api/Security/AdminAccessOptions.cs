namespace OpenAPU.Api.Security;

public sealed record AdminAccessOptions
{
    public const int MinimumKeyLength = 24;

    private AdminAccessOptions(string? key)
    {
        Key = key;
    }

    public string? Key { get; }

    public bool Enabled =>
        !string.IsNullOrWhiteSpace(Key);

    public static AdminAccessOptions Create(
        string? key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return new AdminAccessOptions((string?)null);
        }

        if (key.Length < MinimumKeyLength)
        {
            throw new InvalidOperationException(
                $"OPENAPU_ADMIN_KEY must contain at least {MinimumKeyLength} characters.");
        }

        return new AdminAccessOptions(key);
    }
}

