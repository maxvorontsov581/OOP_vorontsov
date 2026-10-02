namespace OOP.Domain;

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
