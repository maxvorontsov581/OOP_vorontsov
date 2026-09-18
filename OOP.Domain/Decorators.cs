namespace OOP.Domain;

public sealed class BaseDeliveryCost : IDeliveryCost
{
    public decimal Total { get; }
    private readonly string _description;

    public BaseDeliveryCost(decimal total, string description)
    {
        Total = total;
        _description = description;
    }

    public string Describe() => $"{_description}: {Total:0.00}";
}

public abstract class DeliveryCostDecorator : IDeliveryCost
{
    protected readonly IDeliveryCost Inner;

    protected DeliveryCostDecorator(IDeliveryCost inner)
    {
        Inner = inner;
    }

    public abstract decimal Total { get; }
    public abstract string Describe();
}

public sealed class InsuranceDecorator : DeliveryCostDecorator
{
    private readonly IReadOnlyCollection<Cargo> _cargo;

    public InsuranceDecorator(IDeliveryCost inner, IReadOnlyCollection<Cargo> cargo) : base(inner)
    {
        _cargo = cargo;
    }

    public override decimal Total
        => Inner.Total + _cargo.Sum(x => x.DeclaredValue) * TariffConfig.Instance.InsurancePercent;

    public override string Describe() => Inner.Describe() + $" -> страховка = {Total:0.00}";
}

public sealed class UrgencyDecorator : DeliveryCostDecorator
{
    private readonly decimal _multiplier;

    public UrgencyDecorator(IDeliveryCost inner, decimal multiplier = 1.2m) : base(inner)
    {
        _multiplier = multiplier;
    }

    public override decimal Total => Inner.Total * _multiplier;
    public override string Describe() => Inner.Describe() + $" -> срочность = {Total:0.00}";
}

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
