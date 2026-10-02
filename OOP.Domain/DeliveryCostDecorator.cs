namespace OOP.Domain;

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
