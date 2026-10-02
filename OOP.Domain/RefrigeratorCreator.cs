namespace OOP.Domain;

public sealed class RefrigeratorCreator : VehicleCreator
{
    public override Vehicle Create(string number) => new RefrigeratorTruck(number);
}
