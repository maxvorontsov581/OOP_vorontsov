namespace OOP.Domain;

public sealed class InvalidOrderStateException : LogisticsException
{
    public InvalidOrderStateException(string message) : base(message) { }
}
