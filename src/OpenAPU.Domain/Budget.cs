namespace OpenAPU.Domain;

public sealed class BudgetItem
{
    public Identifier Id { get; }
    public Concept Concept { get; }
    public Quantity Quantity { get; private set; }
    public Money UnitPrice { get; private set; }
    public Money Total => UnitPrice.Multiply(Quantity);

    internal BudgetItem(
        Identifier id,
        Concept concept,
        Quantity quantity,
        Money unitPrice)
    {
        Id = id ?? throw new DomainException("Budget item identifier is required.");
        Concept = concept ?? throw new DomainException("Concept is required.");
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    internal static BudgetItem Create(
        Concept concept,
        Quantity quantity)
    {
        return new BudgetItem(
            Identifier.Create(),
            concept,
            quantity,
            concept.UnitPrice);
    }

    internal static BudgetItem Rehydrate(
        Identifier id,
        Concept concept,
        Quantity quantity,
        Money unitPrice)
    {
        return new BudgetItem(id, concept, quantity, unitPrice);
    }

    internal void ChangeQuantity(Quantity quantity) => Quantity = quantity;
    internal void RefreshPrice() => UnitPrice = Concept.UnitPrice;
}

public sealed class Budget
{
    private readonly List<BudgetItem> _items = [];

    public Identifier Id { get; }
    public Key Key { get; }
    public string Name { get; private set; }
    public IReadOnlyCollection<BudgetItem> Items => _items.AsReadOnly();
    public Money Total => Money.From(_items.Sum(item => item.Total.Amount));

    private Budget(
        Identifier id,
        Key key,
        string name)
    {
        Id = id ?? throw new DomainException("Budget identifier is required.");
        Key = key ?? throw new DomainException("Budget key is required.");

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Budget name is required.");
        }

        Name = name.Trim();
    }

    public static Budget Create(
        Key key,
        string name)
    {
        return new Budget(
            Identifier.Create(),
            key,
            name);
    }

    public static Budget Rehydrate(
        Identifier id,
        Key key,
        string name,
        IEnumerable<BudgetItemSnapshot> items)
    {
        var budget = new Budget(id, key, name);

        foreach (var item in items)
        {
            budget._items.Add(
                BudgetItem.Rehydrate(
                    item.Id,
                    item.Concept,
                    item.Quantity,
                    item.UnitPrice));
        }

        return budget;
    }

    public void AddItem(
        Concept concept,
        Quantity quantity)
    {
        if (_items.Any(item => item.Concept.Id == concept.Id))
        {
            throw new DomainException("Concept already exists in budget.");
        }

        _items.Add(BudgetItem.Create(concept, quantity));
    }

    public void RemoveItem(Identifier id)
    {
        var item = Find(id);
        _items.Remove(item);
    }

    public void ChangeQuantity(
        Identifier id,
        Quantity quantity)
    {
        Find(id).ChangeQuantity(quantity);
    }

    public void RefreshPrices()
    {
        _items.ForEach(item => item.RefreshPrice());
    }

    private BudgetItem Find(Identifier id)
    {
        return _items.SingleOrDefault(item => item.Id == id)
            ?? throw new DomainException("Budget item was not found.");
    }
}

public sealed record BudgetItemSnapshot(
    Identifier Id,
    Concept Concept,
    Quantity Quantity,
    Money UnitPrice);
