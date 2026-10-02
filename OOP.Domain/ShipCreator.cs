namespace OOP.Domain;

public sealed class ShipCreator : VehicleCreator
{
    public override Vehicle Create(string number) => new CargoShip(number);
}
