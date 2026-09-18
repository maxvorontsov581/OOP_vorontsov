namespace OOP.Domain;

public sealed class Order : IEntity
{
    private readonly List<Cargo> _cargo = new();

    public Guid Id { get; }
    public Customer Customer { get; }
    public IReadOnlyCollection<Cargo> Cargo => _cargo.AsReadOnly();
    public Route Route { get; }
    public Vehicle? AssignedVehicle { get; private set; }
    public decimal TotalCost { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; }
    public ExtraServices Services { get; private set; }

    public Order(Customer customer, IEnumerable<Cargo> cargo, Route route)
        : this(Guid.NewGuid(), customer, cargo, route, OrderStatus.Created, null, 0,
            DateTime.Now, ExtraServices.None)
    {
    }

    internal Order(Guid id, Customer customer, IEnumerable<Cargo> cargo, Route route,
        OrderStatus status, Vehicle? assignedVehicle, decimal totalCost,
        DateTime createdAt, ExtraServices extraServices)
    {
        Id = id;
        Customer = customer ?? throw new ArgumentNullException(nameof(customer));
        Route = route ?? throw new ArgumentNullException(nameof(route));
        _cargo.AddRange(cargo);
        if (_cargo.Count == 0)
            throw new CargoValidationException("В заказе должен быть хотя бы один груз.");

        Status = status;
        AssignedVehicle = assignedVehicle;
        TotalCost = totalCost;
        CreatedAt = createdAt;
        Services = extraServices;
        customer.AddOrder(this);
    }

    public void Assign(Vehicle vehicle, decimal cost)
    {
        if (Status != OrderStatus.Created)
            throw new InvalidOrderStateException("Назначить транспорт можно только новому заказу.");
        if (cost < 0)
            throw new ArgumentOutOfRangeException(nameof(cost));

        AssignedVehicle = vehicle;
        TotalCost = cost;
        Status = OrderStatus.Assigned;
    }

    internal void ApplyFinalCost(decimal cost, ExtraServices services)
    {
        if (Status != OrderStatus.Assigned)
            throw new InvalidOrderStateException("Дополнительные услуги задаются после назначения транспорта.");

        TotalCost = cost;
        Services = services;
    }

    public void StartDelivery()
    {
        if (Status != OrderStatus.Assigned)
            throw new InvalidOrderStateException("Запустить можно только назначенный заказ.");

        Status = OrderStatus.InTransit;
    }

    public void Complete()
    {
        if (Status != OrderStatus.InTransit)
            throw new InvalidOrderStateException("Завершить можно только заказ в пути.");

        Status = OrderStatus.Delivered;
    }

    public void Cancel()
    {
        if (Status != OrderStatus.Created && Status != OrderStatus.Assigned)
            throw new InvalidOrderStateException("Этот заказ уже нельзя отменить.");

        Status = OrderStatus.Cancelled;
    }

    public override string ToString()
        => $"{Id.ToString()[..8]} | {Customer.Name} | {Status} | {TotalCost:0.00}";
}
