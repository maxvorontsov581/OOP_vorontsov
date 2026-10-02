namespace OOP.Domain;

public sealed class OrderCreatedEventArgs : EventArgs
{
    public Order Order { get; }
    public OrderCreatedEventArgs(Order order) => Order = order;
}
