namespace OOP.Domain;

public sealed class PerishableCargo : Cargo, ITemperatureSensitive
{
    public DateTime ExpirationDate { get; }
    public double RequiredTemperatureC { get; }

    public PerishableCargo(string description, double weightKg, double volumeM3, decimal declaredValue,
        DateTime expirationDate, double requiredTemperatureC)
        : base(description, weightKg, volumeM3, declaredValue)
    {
        ExpirationDate = expirationDate;
        RequiredTemperatureC = requiredTemperatureC;
    }

    internal PerishableCargo(Guid id, string description, double weightKg, double volumeM3, decimal declaredValue,
        DateTime expirationDate, double requiredTemperatureC)
        : base(id, description, weightKg, volumeM3, declaredValue)
    {
        ExpirationDate = expirationDate;
        RequiredTemperatureC = requiredTemperatureC;
    }
}
