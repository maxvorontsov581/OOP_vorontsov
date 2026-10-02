namespace OOP.Domain;

public sealed class TruckCreator : VehicleCreator
{
    public override Vehicle Create(string number) => new Truck(number);
}
