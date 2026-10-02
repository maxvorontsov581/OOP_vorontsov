using System.Text.RegularExpressions;

namespace OOP.Domain;

public sealed class CargoPlane : Vehicle
{
    public decimal PricePerKg { get; }

    public CargoPlane(string number, decimal pricePerKg = 2.5m)
        : base(number, 40000, 150, 750, 120)
    {
        PricePerKg = pricePerKg;
        Conditions = TransportConditions.Pressurized | TransportConditions.LongRange;
    }

    internal CargoPlane(Guid id, string number, VehicleState state, decimal pricePerKg)
        : base(id, number, 40000, 150, 750, 120, state)
    {
        PricePerKg = pricePerKg;
        Conditions = TransportConditions.Pressurized | TransportConditions.LongRange;
    }

    public override bool CanCarry(Cargo cargo)
    {
        if (!base.CanCarry(cargo))
            return false;

        return cargo is not DangerousCargo dangerous || dangerous.HazardClass > 3;
    }

    public override decimal CalculateDeliveryCost(Route route, IReadOnlyCollection<Cargo> cargo)
    {
        decimal weight = (decimal)cargo.Sum(x => x.WeightKg);
        return (decimal)route.DistanceKm * BaseRatePerKm + weight * PricePerKg;
    }
}
