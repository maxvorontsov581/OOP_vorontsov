namespace OOP.Domain;

public sealed class ExpressTariff : ITariffStrategy
{
    public string Name => "Экспресс";
    public decimal Calculate(decimal baseCost, Route route, IReadOnlyCollection<Cargo> cargo)
        => baseCost * TariffConfig.Instance.ExpressMultiplier;
}
