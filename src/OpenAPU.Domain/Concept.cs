namespace OpenAPU.Domain;

public sealed class Concept
{
    public Identifier Id { get; } = Identifier.Create();
    public Key Key { get; }
    public string Name { get; private set; }
    public Unit Unit { get; }
    public Apu Apu { get; }
    public Percentage IndirectCost { get; private set; } = Percentage.Zero;
    public Percentage Financing { get; private set; } = Percentage.Zero;
    public Percentage Profit { get; private set; } = Percentage.Zero;
    public Percentage AdditionalCharges { get; private set; } = Percentage.Zero;
    public Money DirectCost => Apu.DirectCost;
    public Money UnitPrice => Money.From(DirectCost.Amount * (1m + IndirectCost.Fraction + Financing.Fraction + Profit.Fraction + AdditionalCharges.Fraction));

    private Concept(Key key, string name, Unit unit, Apu apu)
    {
        Key = key ?? throw new DomainException("Concept key is required.");
        Unit = unit ?? throw new DomainException("Concept unit is required.");
        Apu = apu ?? throw new DomainException("Concept APU is required.");
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Concept name is required.");
        Name = name.Trim();
    }

    public static Concept Create(Key key, string name, Unit unit, Apu apu) => new(key, name, unit, apu);
    public void SetIndirectCost(Percentage value) => IndirectCost = value;
    public void SetFinancing(Percentage value) => Financing = value;
    public void SetProfit(Percentage value) => Profit = value;
    public void SetAdditionalCharges(Percentage value) => AdditionalCharges = value;
}
