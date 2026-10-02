using System.Text.RegularExpressions;

namespace OOP.Domain;

public sealed class RefrigeratorTruck : Truck
{
    public double MinTemperatureC { get; }
    public double MaxTemperatureC { get; }

    public RefrigeratorTruck(string number, double minTemperatureC = -20, double maxTemperatureC = 10)
        : base(number, 16000, 70, 65, 42, 1.08m)
    {
        MinTemperatureC = minTemperatureC;
        MaxTemperatureC = maxTemperatureC;
        Conditions = TransportConditions.Sealed | TransportConditions.Refrigerated;
    }

    internal RefrigeratorTruck(Guid id, string number, VehicleState state,
        double minTemperatureC, double maxTemperatureC)
        : base(id, number, 16000, 70, 65, 42, state, 1.08m)
    {
        MinTemperatureC = minTemperatureC;
        MaxTemperatureC = maxTemperatureC;
        Conditions = TransportConditions.Sealed | TransportConditions.Refrigerated;
    }

    public override bool CanCarry(Cargo cargo)
    {
        if (!base.CanCarry(cargo))
            return false;

        if (cargo is ITemperatureSensitive temp)
            return temp.RequiredTemperatureC >= MinTemperatureC && temp.RequiredTemperatureC <= MaxTemperatureC;

        return true;
    }

    public override decimal CalculateDeliveryCost(Route route, IReadOnlyCollection<Cargo> cargo)
        => base.CalculateDeliveryCost(route, cargo) + 500m;
}
