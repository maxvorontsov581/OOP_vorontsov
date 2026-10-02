namespace OOP.Domain;

public interface IDeliveryCost
{
    decimal Total { get; }
    string Describe();
}
