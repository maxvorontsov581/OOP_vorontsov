namespace OOP.Domain;

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
