namespace OpenAPU.Domain;

public sealed class BudgetItem
{
    public Identifier Id { get; } = Identifier.Create();
    public Concept Concept { get; }
    public Quantity Quantity { get; private set; }
    public Money UnitPrice { get; private set; }
    public Money Total => UnitPrice.Multiply(Quantity);

    internal BudgetItem(Concept concept, Quantity quantity)
    {
        Concept = concept ?? throw new DomainException("Concept is required.");
        Quantity = quantity;
        UnitPrice = concept.UnitPrice;
    }

    internal void ChangeQuantity(Quantity quantity) => Quantity = quantity;
    internal void RefreshPrice() => UnitPrice = Concept.UnitPrice;
}

public sealed class Budget
{
    private readonly List<BudgetItem> _items = [];
    public Identifier Id { get; } = Identifier.Create();
    public Key Key { get; }
    public string Name { get; private set; }
    public IReadOnlyCollection<BudgetItem> Items => _items.AsReadOnly();
    public Money Total => Money.From(_items.Sum(x => x.Total.Amount));

    private Budget(Key key, string name)
    {
        Key = key ?? throw new DomainException("Budget key is required.");
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Budget name is required.");
        Name = name.Trim();
    }

    public static Budget Create(Key key, string name) => new(key, name);
    public void AddItem(Concept concept, Quantity quantity)
    {
        if (_items.Any(x => x.Concept.Id == concept.Id)) throw new DomainException("Concept already exists in budget.");
        _items.Add(new BudgetItem(concept, quantity));
    }
    public void RemoveItem(Identifier id)
    {
        var item = Find(id);
        _items.Remove(item);
    }
    public void ChangeQuantity(Identifier id, Quantity quantity) => Find(id).ChangeQuantity(quantity);
    public void RefreshPrices() => _items.ForEach(x => x.RefreshPrice());
    private BudgetItem Find(Identifier id) => _items.SingleOrDefault(x => x.Id == id) ?? throw new DomainException("Budget item was not found.");
}
