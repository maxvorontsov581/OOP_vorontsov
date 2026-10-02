namespace OOP.Domain;

public enum VehicleState
{
    Free,
    InTransit,
    UnderMaintenance
}

public enum OrderStatus
{
    Created,
    Assigned,
    InTransit,
    Delivered,
    Cancelled
}

[Flags]
public enum TransportConditions
{
    None = 0,
    Refrigerated = 1,
    Sealed = 2,
    Pressurized = 4,
    LongRange = 8
}

[Flags]
public enum ExtraServices
{
    None = 0,
    Insurance = 1,
    Urgent = 2,
    FragilePacking = 4
}
