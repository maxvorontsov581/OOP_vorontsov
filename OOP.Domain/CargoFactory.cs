namespace OOP.Domain;

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
