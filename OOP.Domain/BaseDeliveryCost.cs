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
