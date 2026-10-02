namespace OOP.Domain;

public sealed class VehicleOverloadException : LogisticsException
{
    public VehicleOverloadException(string message) : base(message) { }
}
