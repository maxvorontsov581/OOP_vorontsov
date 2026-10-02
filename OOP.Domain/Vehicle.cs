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
