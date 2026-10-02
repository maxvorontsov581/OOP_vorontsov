namespace OOP.Domain;

public interface ITariffStrategy
{
    string Name { get; }
    decimal Calculate(decimal baseCost, Route route, IReadOnlyCollection<Cargo> cargo);
}
