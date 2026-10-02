namespace OOP.Domain;

public sealed class FragilePackingDecorator : DeliveryCostDecorator
{
    private readonly IReadOnlyCollection<Cargo> _cargo;

    public FragilePackingDecorator(IDeliveryCost inner, IReadOnlyCollection<Cargo> cargo) : base(inner)
    {
        _cargo = cargo;
    }

    public override decimal Total
        => Inner.Total + (_cargo.Any(x => x is FragileCargo) ? TariffConfig.Instance.FragilePackingPrice : 0m);

    public override string Describe() => Inner.Describe() + $" -> упаковка = {Total:0.00}";
}
