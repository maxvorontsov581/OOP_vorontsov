using OOP.Domain;
using Xunit;

namespace OOP.Tests;

public class ValidatorTests
{
    [Fact]
    public void BadMix()
    {
        CargoCompatibilityValidator validator = new();
        Cargo[] cargo =
        {
            new DangerousCargo("Danger", 1, 0.1, 1, 5),
            new PerishableCargo("Food", 1, 0.1, 1, DateTime.Now.AddDays(1), 4)
        };
        Assert.Throws<IncompatibleCargoException>(() => validator.ValidateCargoSet(cargo));
    }

    [Fact]
    public void Expired()
    {
        CargoCompatibilityValidator validator = new();
        Cargo[] cargo = { new PerishableCargo("Old", 1, 0.1, 1, DateTime.Now.AddDays(-1), 4) };
        Assert.Throws<CargoValidationException>(() => validator.ValidateCargoSet(cargo));
    }

    [Fact]
    public void NeedFridge()
    {
        CargoCompatibilityValidator validator = new();
        Cargo[] cargo = { new PerishableCargo("Food", 1, 0.1, 1, DateTime.Now.AddDays(1), 4) };
        Assert.Throws<IncompatibleCargoException>(() => validator.ValidateForVehicle(cargo, new Truck("TRK-010")));
    }

    [Fact]
    public void Overload()
    {
        CargoCompatibilityValidator validator = new();
        Truck smallTruck = new("TRK-011", maxLoadKg: 100, maxVolumeM3: 10);
        Cargo[] cargo =
        {
            new StandardCargo("A", 60, 1, 1),
            new StandardCargo("B", 60, 1, 1)
        };
        Assert.Throws<VehicleOverloadException>(() => validator.ValidateForVehicle(cargo, smallTruck));
    }

    [Fact]
    public void EmptyCargo()
    {
        CargoCompatibilityValidator validator = new();
        Assert.Throws<CargoValidationException>(() => validator.ValidateCargoSet(Array.Empty<Cargo>()));
    }

    [Fact]
    public void ZeroWeight()
    {
        Assert.Throws<CargoValidationException>(() => new StandardCargo("Bad", 0, 1, 1));
    }
}
