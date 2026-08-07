namespace OpenAPU.Domain;

public enum ResourceType { Material, Labor, Equipment, Tool, Auxiliary }
public enum ResourceStatus { Active, Inactive }

public sealed class Resource
{
    public Identifier Id { get; }
    public Key Key { get; }
    public string Name { get; private set; }
    public ResourceType Type { get; }
    public Unit Unit { get; }
    public Money Price { get; private set; }
    public ResourceStatus Status { get; private set; }

    private Resource(
        Identifier id,
        Key key,
        string name,
        ResourceType type,
        Unit unit,
        Money price,
        ResourceStatus status)
    {
        Id = id ?? throw new DomainException("Resource identifier is required.");
        Key = key ?? throw new DomainException("Resource key is required.");
        Unit = unit ?? throw new DomainException("Resource unit is required.");

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Resource name is required.");
        }

        Name = name.Trim();
        Type = type;
        Price = price;
        Status = status;
    }

    public static Resource Create(
        Key key,
        string name,
        ResourceType type,
        Unit unit,
        Money price)
    {
        return new Resource(
            Identifier.Create(),
            key,
            name,
            type,
            unit,
            price,
            ResourceStatus.Active);
    }

    public static Resource Rehydrate(
        Identifier id,
        Key key,
        string name,
        ResourceType type,
        Unit unit,
        Money price,
        ResourceStatus status)
    {
        return new Resource(id, key, name, type, unit, price, status);
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Resource name is required.");
        }

        Name = name.Trim();
    }

    public void ChangePrice(Money price) => Price = price;
    public void Activate() => Status = ResourceStatus.Active;
    public void Deactivate() => Status = ResourceStatus.Inactive;
}
