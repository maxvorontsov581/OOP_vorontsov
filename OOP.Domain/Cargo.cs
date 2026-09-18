namespace OOP.Domain;

public abstract class Cargo : IEntity
{
    public Guid Id { get; }
    public string Description { get; private set; }
    public double WeightKg { get; private set; }
    public double VolumeM3 { get; private set; }
    public decimal DeclaredValue { get; private set; }

    protected Cargo(string description, double weightKg, double volumeM3, decimal declaredValue)
        : this(Guid.NewGuid(), description, weightKg, volumeM3, declaredValue)
    {
    }

    protected Cargo(Guid id, string description, double weightKg, double volumeM3, decimal declaredValue)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new CargoValidationException("Описание груза не задано.");
        if (weightKg <= 0)
            throw new CargoValidationException("Вес должен быть больше нуля.");
        if (volumeM3 <= 0)
            throw new CargoValidationException("Объём должен быть больше нуля.");
        if (declaredValue < 0)
            throw new CargoValidationException("Стоимость не может быть отрицательной.");

        Id = id;
        Description = description;
        WeightKg = weightKg;
        VolumeM3 = volumeM3;
        DeclaredValue = declaredValue;
    }

    public override string ToString() => $"{Description}, {WeightKg:0.##} кг";
}

public sealed class StandardCargo : Cargo, IStackable
{
    public bool CanStack => true;

    public StandardCargo(string description, double weightKg, double volumeM3, decimal declaredValue)
        : base(description, weightKg, volumeM3, declaredValue) { }

    internal StandardCargo(Guid id, string description, double weightKg, double volumeM3, decimal declaredValue)
        : base(id, description, weightKg, volumeM3, declaredValue) { }
}

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

public sealed class FragileCargo : Cargo, IInsurable
{
    public double RiskFactor { get; }

    public FragileCargo(string description, double weightKg, double volumeM3, decimal declaredValue, double riskFactor)
        : base(description, weightKg, volumeM3, declaredValue)
    {
        if (riskFactor <= 0)
            throw new CargoValidationException("Коэффициент риска должен быть больше нуля.");
        RiskFactor = riskFactor;
    }

    internal FragileCargo(Guid id, string description, double weightKg, double volumeM3, decimal declaredValue, double riskFactor)
        : base(id, description, weightKg, volumeM3, declaredValue)
    {
        RiskFactor = riskFactor;
    }

    decimal IInsurable.InsuranceValue => DeclaredValue * (decimal)RiskFactor;
}

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

public sealed class OversizedCargo : Cargo
{
    public OversizedCargo(string description, double weightKg, double volumeM3, decimal declaredValue)
        : base(description, weightKg, volumeM3, declaredValue) { }

    internal OversizedCargo(Guid id, string description, double weightKg, double volumeM3, decimal declaredValue)
        : base(id, description, weightKg, volumeM3, declaredValue) { }
}
