namespace OpenAPU.Domain;

public sealed record Unit
{
    public string Code { get; }
    public string Symbol { get; }
    public string Name { get; }

    private Unit(string code, string symbol, string name)
    {
        if (string.IsNullOrWhiteSpace(code)) throw new DomainException("Unit code is required.");
        if (string.IsNullOrWhiteSpace(symbol)) throw new DomainException("Unit symbol is required.");
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Unit name is required.");
        Code = code.Trim().ToUpperInvariant();
        Symbol = symbol.Trim();
        Name = name.Trim();
    }

    public static Unit Create(string code, string symbol, string name) => new(code, symbol, name);
    public bool Equals(Unit? other) => other is not null && Code == other.Code;
    public override int GetHashCode() => Code.GetHashCode(StringComparison.Ordinal);
    public override string ToString() => Symbol;
}
