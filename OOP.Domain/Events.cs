namespace OOP.Domain;

public delegate void LogisticsEvent<in T>(object sender, T e)
    where T : EventArgs;

public sealed class OrderCreatedEventArgs : EventArgs
{
    public Order Order { get; }
    public OrderCreatedEventArgs(Order order) => Order = order;
}

public sealed class OrderStatusChangedEventArgs : EventArgs
{
    public Order Order { get; }
    public OrderStatus OldStatus { get; }
    public OrderStatus NewStatus { get; }

    public OrderStatusChangedEventArgs(Order order, OrderStatus oldStatus, OrderStatus newStatus)
    {
        Order = order;
        OldStatus = oldStatus;
        NewStatus = newStatus;
    }
}

public sealed class VehicleOverloadAttemptEventArgs : EventArgs
{
    public Vehicle Vehicle { get; }
    public Order Order { get; }
    public double TotalWeightKg { get; }

    public VehicleOverloadAttemptEventArgs(Vehicle vehicle, Order order, double totalWeightKg)
    {
        Vehicle = vehicle;
        Order = order;
        TotalWeightKg = totalWeightKg;
    }
}

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
