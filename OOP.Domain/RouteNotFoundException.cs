namespace OOP.Domain;

public sealed class RouteNotFoundException : LogisticsException
{
    public RouteNotFoundException(string message) : base(message) { }
}
