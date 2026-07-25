namespace OpenAPU.Domain;

public sealed record Key
{
    public string Value { get; }

    private Key(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException("Key is required.");
        Value = value.Trim().ToUpperInvariant();
    }

    public static Key From(string value) => new(value);
    public override string ToString() => Value;
}
