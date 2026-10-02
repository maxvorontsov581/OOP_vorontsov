using System.Text.RegularExpressions;

namespace OOP.Domain;

public class Truck : Vehicle
{
    public decimal TollRoadCoefficient { get; }

    public Truck(string number, double maxLoadKg = 20000, double maxVolumeM3 = 80,
        double speed = 70, decimal rate = 35, decimal tollRoadCoefficient = 1.10m)
        : base(number, maxLoadKg, maxVolumeM3, speed, rate)
    {
        TollRoadCoefficient = tollRoadCoefficient;
        Conditions = TransportConditions.Sealed;
    }

    internal Truck(Guid id, string number, double maxLoadKg, double maxVolumeM3,
        double speed, decimal rate, VehicleState state, decimal tollRoadCoefficient)
        : base(id, number, maxLoadKg, maxVolumeM3, speed, rate, state)
    {
        TollRoadCoefficient = tollRoadCoefficient;
        Conditions = TransportConditions.Sealed;
    }

    public override decimal CalculateDeliveryCost(Route route, IReadOnlyCollection<Cargo> cargo)
        => (decimal)route.DistanceKm * BaseRatePerKm * TollRoadCoefficient;
}
