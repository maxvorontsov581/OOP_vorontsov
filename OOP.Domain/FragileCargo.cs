namespace OOP.Domain;

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
