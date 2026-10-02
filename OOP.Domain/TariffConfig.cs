namespace OOP.Domain;

public sealed class TariffConfig
{
    private static readonly Lazy<TariffConfig> _instance = new(() => new TariffConfig());
    public static TariffConfig Instance => _instance.Value;

    public decimal ExpressMultiplier { get; set; } = 1.5m;
    public decimal InsurancePercent { get; set; } = 0.02m;
    public decimal FragilePackingPrice { get; set; } = 700m;

    private TariffConfig() { }
}
