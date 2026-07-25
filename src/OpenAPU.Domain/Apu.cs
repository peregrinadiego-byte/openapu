namespace OpenAPU.Domain;

public sealed class ApuComponent
{
    public Identifier Id { get; } = Identifier.Create();
    public Resource Resource { get; }
    public Quantity Quantity { get; private set; }
    public Money UnitPrice { get; private set; }
    public Money Total => UnitPrice.Multiply(Quantity);

    internal ApuComponent(Resource resource, Quantity quantity)
    {
        Resource = resource ?? throw new DomainException("Resource is required.");
        Quantity = quantity;
        UnitPrice = resource.Price;
    }

    internal void ChangeQuantity(Quantity quantity) => Quantity = quantity;
    internal void RefreshPrice() => UnitPrice = Resource.Price;
}

public sealed class Apu
{
    private readonly List<ApuComponent> _components = [];
    public Identifier Id { get; } = Identifier.Create();
    public Key Key { get; }
    public string Name { get; private set; }
    public Unit Unit { get; }
    public IReadOnlyCollection<ApuComponent> Components => _components.AsReadOnly();
    public Money DirectCost => Money.From(_components.Sum(x => x.Total.Amount));

    private Apu(Key key, string name, Unit unit)
    {
        Key = key ?? throw new DomainException("APU key is required.");
        Unit = unit ?? throw new DomainException("APU unit is required.");
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("APU name is required.");
        Name = name.Trim();
    }

    public static Apu Create(Key key, string name, Unit unit) => new(key, name, unit);
    public void AddComponent(Resource resource, Quantity quantity)
    {
        if (_components.Any(x => x.Resource.Id == resource.Id)) throw new DomainException("Resource already exists in APU.");
        _components.Add(new ApuComponent(resource, quantity));
    }
    public void RemoveComponent(Identifier id)
    {
        var item = _components.SingleOrDefault(x => x.Id == id) ?? throw new DomainException("APU component was not found.");
        _components.Remove(item);
    }
    public void ChangeQuantity(Identifier id, Quantity quantity) => Find(id).ChangeQuantity(quantity);
    public void RefreshPrices() => _components.ForEach(x => x.RefreshPrice());
    private ApuComponent Find(Identifier id) => _components.SingleOrDefault(x => x.Id == id) ?? throw new DomainException("APU component was not found.");
}
