namespace OOP.Domain;

public sealed class DeliveryCompletedEventArgs : EventArgs
{
    public Order Order { get; }
    public decimal Revenue { get; }

    public DeliveryCompletedEventArgs(Order order, decimal revenue)
    {
        Order = order;
        Revenue = revenue;
    }
}
