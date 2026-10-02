namespace OOP.Domain;

public sealed class PlaneCreator : VehicleCreator
{
    public override Vehicle Create(string number) => new CargoPlane(number);
}
