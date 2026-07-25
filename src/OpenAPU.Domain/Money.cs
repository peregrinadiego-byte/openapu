namespace OpenAPU.Domain;

public readonly record struct Money
{
    public decimal Amount { get; }
    public static Money Zero => new(0m);

    private Money(decimal amount)
    {
        if (amount < 0m) throw new DomainException("Money cannot be negative.");
        Amount = amount;
    }

    public static Money From(decimal amount) => new(amount);
    public Money Add(Money other) => new(Amount + other.Amount);
    public Money Multiply(Quantity quantity) => new(Amount * quantity.Value);
    public Money Apply(Percentage percentage) => new(Amount * percentage.Fraction);
    public override string ToString() => Amount.ToString("0.####");
}
