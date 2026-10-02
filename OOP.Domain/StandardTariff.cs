namespace OOP.Domain;

public sealed class StandardTariff : ITariffStrategy
{
    public string Name => "Стандарт";
    public decimal Calculate(decimal baseCost, Route route, IReadOnlyCollection<Cargo> cargo) => baseCost;
}
