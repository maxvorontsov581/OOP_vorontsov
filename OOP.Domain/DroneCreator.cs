namespace OOP.Domain;

public sealed class DroneCreator : VehicleCreator
{
    public override Vehicle Create(string number) => new DroneCourier(number);
}
