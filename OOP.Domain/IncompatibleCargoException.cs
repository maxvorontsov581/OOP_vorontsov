namespace OOP.Domain;

public sealed class IncompatibleCargoException : LogisticsException
{
    public IncompatibleCargoException(string message) : base(message) { }
}
