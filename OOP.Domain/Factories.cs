namespace OOP.Domain;

public abstract class VehicleCreator
{
    public abstract Vehicle Create(string number);
}

public sealed class TruckCreator : VehicleCreator
{
    public override Vehicle Create(string number) => new Truck(number);
}

public sealed class RefrigeratorCreator : VehicleCreator
{
    public override Vehicle Create(string number) => new RefrigeratorTruck(number);
}

public sealed class PlaneCreator : VehicleCreator
{
    public override Vehicle Create(string number) => new CargoPlane(number);
}

public sealed class ShipCreator : VehicleCreator
{
    public override Vehicle Create(string number) => new CargoShip(number);
}

public sealed class DroneCreator : VehicleCreator
{
    public override Vehicle Create(string number) => new DroneCourier(number);
}

public sealed class VehicleFactory
{
    private readonly Dictionary<string, VehicleCreator> _creators = new(StringComparer.OrdinalIgnoreCase);

    public VehicleFactory()
    {
        Register("truck", new TruckCreator());
        Register("refrigerator", new RefrigeratorCreator());
        Register("plane", new PlaneCreator());
        Register("ship", new ShipCreator());
        Register("drone", new DroneCreator());
    }

    public void Register(string type, VehicleCreator creator)
    {
        _creators[type] = creator;
    }

    public Vehicle Create(string type, string number)
    {
        if (!_creators.TryGetValue(type, out VehicleCreator? creator))
            throw new ArgumentException("Неизвестный тип транспорта.");

        return creator.Create(number);
    }
}

public static class CargoFactory
{
    public static Cargo Create(string type, string description, double weight, double volume, decimal value)
    {
        return type.ToLowerInvariant() switch
        {
            "standard" => new StandardCargo(description, weight, volume, value),
            "fragile" => new FragileCargo(description, weight, volume, value, 1.2),
            "dangerous" => new DangerousCargo(description, weight, volume, value, 5),
            "oversized" => new OversizedCargo(description, weight, volume, value),
            "perishable" => new PerishableCargo(description, weight, volume, value, DateTime.Now.AddDays(3), 4),
            _ => throw new ArgumentException("Неизвестный тип груза.")
        };
    }
}
