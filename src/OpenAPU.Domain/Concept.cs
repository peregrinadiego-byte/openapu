namespace OpenAPU.Domain;

public sealed class Concept
{
    public Identifier Id { get; }
    public Key Key { get; }
    public string Name { get; private set; }
    public Unit Unit { get; }
    public Apu Apu { get; }
    public Percentage IndirectCost { get; private set; }
    public Percentage Financing { get; private set; }
    public Percentage Profit { get; private set; }
    public Percentage AdditionalCharges { get; private set; }

    public Money DirectCost => Apu.DirectCost;

    public Money UnitPrice => Money.From(
        DirectCost.Amount *
        (1m +
         IndirectCost.Fraction +
         Financing.Fraction +
         Profit.Fraction +
         AdditionalCharges.Fraction));

    private Concept(
        Identifier id,
        Key key,
        string name,
        Unit unit,
        Apu apu,
        Percentage indirectCost,
        Percentage financing,
        Percentage profit,
        Percentage additionalCharges)
    {
        Id = id ?? throw new DomainException("Concept identifier is required.");
        Key = key ?? throw new DomainException("Concept key is required.");
        Unit = unit ?? throw new DomainException("Concept unit is required.");
        Apu = apu ?? throw new DomainException("Concept APU is required.");

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Concept name is required.");
        }

        Name = name.Trim();
        IndirectCost = indirectCost;
        Financing = financing;
        Profit = profit;
        AdditionalCharges = additionalCharges;
    }

    public static Concept Create(
        Key key,
        string name,
        Unit unit,
        Apu apu)
    {
        return new Concept(
            Identifier.Create(),
            key,
            name,
            unit,
            apu,
            Percentage.Zero,
            Percentage.Zero,
            Percentage.Zero,
            Percentage.Zero);
    }

    public static Concept Rehydrate(
        Identifier id,
        Key key,
        string name,
        Unit unit,
        Apu apu,
        Percentage indirectCost,
        Percentage financing,
        Percentage profit,
        Percentage additionalCharges)
    {
        return new Concept(
            id,
            key,
            name,
            unit,
            apu,
            indirectCost,
            financing,
            profit,
            additionalCharges);
    }

    public void SetIndirectCost(Percentage value) => IndirectCost = value;
    public void SetFinancing(Percentage value) => Financing = value;
    public void SetProfit(Percentage value) => Profit = value;
    public void SetAdditionalCharges(Percentage value) => AdditionalCharges = value;
}
