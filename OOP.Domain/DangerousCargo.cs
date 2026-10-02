namespace OOP.Domain;

public sealed class DangerousCargo : Cargo
{
    public int HazardClass { get; }

    public DangerousCargo(string description, double weightKg, double volumeM3, decimal declaredValue, int hazardClass)
        : base(description, weightKg, volumeM3, declaredValue)
    {
        if (hazardClass < 1 || hazardClass > 9)
            throw new CargoValidationException("Класс опасности должен быть от 1 до 9.");
        HazardClass = hazardClass;
    }

    internal DangerousCargo(Guid id, string description, double weightKg, double volumeM3, decimal declaredValue, int hazardClass)
        : base(id, description, weightKg, volumeM3, declaredValue)
    {
        HazardClass = hazardClass;
    }
}
