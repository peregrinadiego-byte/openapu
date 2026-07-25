namespace OpenAPU.Domain;

public readonly record struct Quantity
{
    public decimal Value { get; }

    private Quantity(decimal value)
    {
        if (value <= 0m) throw new DomainException("Quantity must be greater than zero.");
        Value = value;
    }

    public static Quantity From(decimal value) => new(value);
    public Quantity Add(Quantity other) => new(Value + other.Value);
    public Money Multiply(Money money) => money.Multiply(this);
    public override string ToString() => Value.ToString("0.####");
}
