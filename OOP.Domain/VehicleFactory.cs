namespace OOP.Domain;

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
