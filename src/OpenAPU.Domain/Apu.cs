namespace OpenAPU.Domain;

public sealed class ApuComponent
{
    public Identifier Id { get; }
    public Resource Resource { get; }
    public Quantity Quantity { get; private set; }
    public Money UnitPrice { get; private set; }
    public Money Total => UnitPrice.Multiply(Quantity);

    internal ApuComponent(
        Identifier id,
        Resource resource,
        Quantity quantity,
        Money unitPrice)
    {
        Id = id ?? throw new DomainException("Component identifier is required.");
        Resource = resource ?? throw new DomainException("Resource is required.");
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    internal static ApuComponent Create(
        Resource resource,
        Quantity quantity)
    {
        return new ApuComponent(
            Identifier.Create(),
            resource,
            quantity,
            resource.Price);
    }

    internal static ApuComponent Rehydrate(
        Identifier id,
        Resource resource,
        Quantity quantity,
        Money unitPrice)
    {
        return new ApuComponent(id, resource, quantity, unitPrice);
    }

    internal void ChangeQuantity(Quantity quantity) => Quantity = quantity;
    internal void RefreshPrice() => UnitPrice = Resource.Price;
}

public sealed class Apu
{
    private readonly List<ApuComponent> _components = [];

    public Identifier Id { get; }
    public Key Key { get; }
    public string Name { get; private set; }
    public Unit Unit { get; }
    public IReadOnlyCollection<ApuComponent> Components => _components.AsReadOnly();
    public Money DirectCost => Money.From(_components.Sum(component => component.Total.Amount));

    private Apu(
        Identifier id,
        Key key,
        string name,
        Unit unit)
    {
        Id = id ?? throw new DomainException("APU identifier is required.");
        Key = key ?? throw new DomainException("APU key is required.");
        Unit = unit ?? throw new DomainException("APU unit is required.");

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("APU name is required.");
        }

        Name = name.Trim();
    }

    public static Apu Create(
        Key key,
        string name,
        Unit unit)
    {
        return new Apu(
            Identifier.Create(),
            key,
            name,
            unit);
    }

    public static Apu Rehydrate(
        Identifier id,
        Key key,
        string name,
        Unit unit,
        IEnumerable<ApuComponentSnapshot> components)
    {
        var apu = new Apu(id, key, name, unit);

        foreach (var component in components)
        {
            apu._components.Add(
                ApuComponent.Rehydrate(
                    component.Id,
                    component.Resource,
                    component.Quantity,
                    component.UnitPrice));
        }

        return apu;
    }

    public void AddComponent(
        Resource resource,
        Quantity quantity)
    {
        if (_components.Any(component => component.Resource.Id == resource.Id))
        {
            throw new DomainException("Resource already exists in APU.");
        }

        _components.Add(ApuComponent.Create(resource, quantity));
    }

    public void RemoveComponent(Identifier id)
    {
        var item = Find(id);
        _components.Remove(item);
    }

    public void ChangeQuantity(
        Identifier id,
        Quantity quantity)
    {
        Find(id).ChangeQuantity(quantity);
    }

    public void RefreshPrices()
    {
        _components.ForEach(component => component.RefreshPrice());
    }

    private ApuComponent Find(Identifier id)
    {
        return _components.SingleOrDefault(component => component.Id == id)
            ?? throw new DomainException("APU component was not found.");
    }
}

public sealed record ApuComponentSnapshot(
    Identifier Id,
    Resource Resource,
    Quantity Quantity,
    Money UnitPrice);
