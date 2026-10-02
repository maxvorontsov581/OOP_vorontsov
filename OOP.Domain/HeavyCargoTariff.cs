namespace OOP.Domain;

public sealed class HeavyCargoTariff : ITariffStrategy
{
    public string Name => "Тяжёлый груз";
    public decimal Calculate(decimal baseCost, Route route, IReadOnlyCollection<Cargo> cargo)
    {
        double weight = cargo.Sum(x => x.WeightKg);
        return weight > 10000 ? baseCost * 1.25m : baseCost;
    }
}
