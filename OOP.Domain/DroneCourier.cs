using System.Text.RegularExpressions;

namespace OOP.Domain;

public sealed class DroneCourier : Vehicle
{
    public double MaxDistanceKm { get; }

    public DroneCourier(string number, double maxDistanceKm = 50)
        : base(number, 10, 0.2, 55, 15)
    {
        MaxDistanceKm = maxDistanceKm;
    }

    internal DroneCourier(Guid id, string number, VehicleState state, double maxDistanceKm)
        : base(id, number, 10, 0.2, 55, 15, state)
    {
        MaxDistanceKm = maxDistanceKm;
    }

    public override bool CanCarry(Cargo cargo)
        => base.CanCarry(cargo) && cargo is not DangerousCargo && cargo is not OversizedCargo;

    public override decimal CalculateDeliveryCost(Route route, IReadOnlyCollection<Cargo> cargo)
    {
        if (route.DistanceKm > MaxDistanceKm)
            return decimal.MaxValue;

        return (decimal)route.DistanceKm * BaseRatePerKm + (decimal)cargo.Sum(x => x.WeightKg) * 5m;
    }
}
