namespace OpenAPU.Domain;

public sealed record Identifier
{
    public Guid Value { get; }

    private Identifier(Guid value)
    {
        if (value == Guid.Empty) throw new DomainException("Identifier cannot be empty.");
        Value = value;
    }

    public static Identifier Create() => new(Guid.NewGuid());
    public static Identifier From(Guid value) => new(value);
    public static Identifier From(string value) => Guid.TryParse(value, out var parsed)
        ? new Identifier(parsed)
        : throw new DomainException("Identifier must be a valid UUID.");

    public override string ToString() => Value.ToString("D");
}
