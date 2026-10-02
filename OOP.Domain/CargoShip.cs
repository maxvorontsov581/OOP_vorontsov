using System.Text.RegularExpressions;

namespace OOP.Domain;

public sealed class CargoShip : Vehicle
{
    public CargoShip(string number)
        : base(number, 500000, 3000, 35, 12)
    {
        Conditions = TransportConditions.Sealed | TransportConditions.LongRange;
    }

    internal CargoShip(Guid id, string number, VehicleState state)
        : base(id, number, 500000, 3000, 35, 12, state)
    {
        Conditions = TransportConditions.Sealed | TransportConditions.LongRange;
    }

    public override decimal CalculateDeliveryCost(Route route, IReadOnlyCollection<Cargo> cargo)
    {
        decimal oversizedExtra = cargo.Count(x => x is OversizedCargo) * 1000m;
        return (decimal)route.DistanceKm * BaseRatePerKm + oversizedExtra;
    }
}
