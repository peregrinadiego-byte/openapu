namespace OpenAPU.Domain;

public readonly record struct Percentage
{
    public decimal Value { get; }
    public decimal Fraction => Value / 100m;
    public static Percentage Zero => new(0m);

    private Percentage(decimal value)
    {
        if (value < 0m || value > 100m) throw new DomainException("Percentage must be between 0 and 100.");
        Value = value;
    }

    public static Percentage From(decimal value) => new(value);
}
