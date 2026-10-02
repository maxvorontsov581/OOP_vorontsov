namespace OOP.Domain;

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
