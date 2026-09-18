namespace OOP.Domain;

public sealed class Customer : IEntity
{
    private readonly List<Order> _orders = new();

    public Guid Id { get; }
    public string Name { get; }
    public string Contact { get; }
    public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();

    public Customer(string name, string contact)
        : this(Guid.NewGuid(), name, contact)
    {
    }

    internal Customer(Guid id, string name, string contact)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Имя клиента не задано.");

        Id = id;
        Name = name;
        Contact = contact;
    }

    internal void AddOrder(Order order)
    {
        if (!_orders.Contains(order))
            _orders.Add(order);
    }

    public override string ToString() => $"{Name} ({Contact})";
}
