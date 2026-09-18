namespace OOP.Domain;

public sealed class StandardTariff : ITariffStrategy
{
    public string Name => "Стандарт";
    public decimal Calculate(decimal baseCost, Route route, IReadOnlyCollection<Cargo> cargo) => baseCost;
}

public sealed class ExpressTariff : ITariffStrategy
{
    public string Name => "Экспресс";
    public decimal Calculate(decimal baseCost, Route route, IReadOnlyCollection<Cargo> cargo)
        => baseCost * TariffConfig.Instance.ExpressMultiplier;
}

public sealed class HeavyCargoTariff : ITariffStrategy
{
    public string Name => "Тяжёлый груз";
    public decimal Calculate(decimal baseCost, Route route, IReadOnlyCollection<Cargo> cargo)
    {
        double weight = cargo.Sum(x => x.WeightKg);
        return weight > 10000 ? baseCost * 1.25m : baseCost;
    }
}

public sealed class TariffConfig
{
    private static readonly Lazy<TariffConfig> _instance = new(() => new TariffConfig());
    public static TariffConfig Instance => _instance.Value;

    public decimal ExpressMultiplier { get; set; } = 1.5m;
    public decimal InsurancePercent { get; set; } = 0.02m;
    public decimal FragilePackingPrice { get; set; } = 700m;

    private TariffConfig() { }
}
