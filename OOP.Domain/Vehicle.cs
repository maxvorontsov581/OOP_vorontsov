using System.Text.RegularExpressions;

namespace OOP.Domain;

public abstract class Vehicle : IEntity
{
    private string _registrationNumber;

    public Guid Id { get; }
    public string RegistrationNumber
    {
        get => _registrationNumber;
        private set
        {
            if (!Regex.IsMatch(value ?? "", "^[A-Za-z0-9-]{3,15}$"))
                throw new ArgumentException("Некорректный регистрационный номер.");
            _registrationNumber = value;
        }
    }

    public double MaxLoadKg { get; }
    public double MaxVolumeM3 { get; }
    public double AverageSpeedKmH { get; }
    public decimal BaseRatePerKm { get; }
    public VehicleState State { get; private set; }
    public TransportConditions Conditions { get; protected set; }

    protected Vehicle(string registrationNumber, double maxLoadKg, double maxVolumeM3,
        double averageSpeedKmH, decimal baseRatePerKm)
        : this(Guid.NewGuid(), registrationNumber, maxLoadKg, maxVolumeM3, averageSpeedKmH,
            baseRatePerKm, VehicleState.Free)
    {
    }

    protected Vehicle(Guid id, string registrationNumber, double maxLoadKg, double maxVolumeM3,
        double averageSpeedKmH, decimal baseRatePerKm, VehicleState state)
    {
        if (maxLoadKg <= 0 || maxVolumeM3 <= 0 || averageSpeedKmH <= 0 || baseRatePerKm < 0)
            throw new ArgumentException("Параметры транспорта заданы неверно.");

        Id = id;
        _registrationNumber = "TMP";
        RegistrationNumber = registrationNumber;
        MaxLoadKg = maxLoadKg;
        MaxVolumeM3 = maxVolumeM3;
        AverageSpeedKmH = averageSpeedKmH;
        BaseRatePerKm = baseRatePerKm;
        State = state;
    }

    public abstract decimal CalculateDeliveryCost(Route route, IReadOnlyCollection<Cargo> cargo);

    public virtual bool CanCarry(Cargo cargo)
    {
        return cargo.WeightKg <= MaxLoadKg && cargo.VolumeM3 <= MaxVolumeM3;
    }

    public void SetState(VehicleState state) => State = state;

    public override string ToString()
        => $"{GetType().Name} {RegistrationNumber} ({State})";

    public override bool Equals(object? obj)
        => obj is Vehicle other && other.Id == Id;

    public override int GetHashCode() => Id.GetHashCode();
}

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
