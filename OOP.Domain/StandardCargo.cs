namespace OOP.Domain;

public sealed class StandardCargo : Cargo, IStackable
{
    public bool CanStack => true;

    public StandardCargo(string description, double weightKg, double volumeM3, decimal declaredValue)
        : base(description, weightKg, volumeM3, declaredValue) { }

    internal StandardCargo(Guid id, string description, double weightKg, double volumeM3, decimal declaredValue)
        : base(id, description, weightKg, volumeM3, declaredValue) { }
}
