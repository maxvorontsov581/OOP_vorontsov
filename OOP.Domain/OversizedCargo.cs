namespace OOP.Domain;

public sealed class OversizedCargo : Cargo
{
    public OversizedCargo(string description, double weightKg, double volumeM3, decimal declaredValue)
        : base(description, weightKg, volumeM3, declaredValue) { }

    internal OversizedCargo(Guid id, string description, double weightKg, double volumeM3, decimal declaredValue)
        : base(id, description, weightKg, volumeM3, declaredValue) { }
}
