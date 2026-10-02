namespace OOP.Domain;

public sealed class CargoValidationException : LogisticsException
{
    public CargoValidationException(string message) : base(message) { }
}
