using OOP.Domain;
using Xunit;

namespace OOP.Tests;

public class VehicleTests
{
    [Fact]
    public void TruckCost()
    {
        Truck truck = new("TRK-001");
        Assert.True(truck.CalculateDeliveryCost(TestData.Route100Km(), new[] { TestData.SmallCargo() }) > 0);
    }

    [Fact]
    public void FridgeCost()
    {
        Truck truck = new("TRK-002", rate: 42, tollRoadCoefficient: 1.08m);
        RefrigeratorTruck fridge = new("REF-001");
        decimal truckCost = truck.CalculateDeliveryCost(TestData.Route100Km(), new[] { TestData.SmallCargo() });
        decimal fridgeCost = fridge.CalculateDeliveryCost(TestData.Route100Km(), new[] { TestData.SmallCargo() });
        Assert.True(fridgeCost > truckCost);
    }

    [Fact]
    public void PlaneCost()
    {
        CargoPlane plane = new("PLN-001");
        decimal light = plane.CalculateDeliveryCost(TestData.Route100Km(), new[] { new StandardCargo("A", 1, 0.1, 1) });
        decimal heavy = plane.CalculateDeliveryCost(TestData.Route100Km(), new[] { new StandardCargo("B", 100, 0.1, 1) });
        Assert.True(heavy > light);
    }

    [Fact]
    public void ShipCost()
    {
        CargoShip ship = new("SHP-001");
        decimal normal = ship.CalculateDeliveryCost(TestData.Route100Km(), new[] { TestData.SmallCargo() });
        decimal oversized = ship.CalculateDeliveryCost(TestData.Route100Km(), new[] { new OversizedCargo("Big", 10, 0.1, 1) });
        Assert.True(oversized > normal);
    }

    [Fact]
    public void DroneDistance()
    {
        DroneCourier drone = new("DRN-001", 10);
        Assert.Equal(decimal.MaxValue,
            drone.CalculateDeliveryCost(TestData.Route100Km(), new[] { TestData.SmallCargo() }));
    }

    [Fact]
    public void ZeroDistance()
    {
        Truck truck = new("TRK-003");
        Assert.Equal(0m, truck.CalculateDeliveryCost(TestData.ZeroRoute(), new[] { TestData.SmallCargo() }));
    }

    [Fact]
    public void WeightLimit()
    {
        Truck truck = new("TRK-004", maxLoadKg: 100, maxVolumeM3: 10);
        Assert.True(truck.CanCarry(new StandardCargo("Limit", 100, 1, 1)));
    }

    [Fact]
    public void PlaneDanger()
    {
        CargoPlane plane = new("PLN-002");
        Assert.False(plane.CanCarry(new DangerousCargo("Danger", 1, 0.1, 1, 2)));
    }

    [Fact]
    public void FridgeTemperature()
    {
        RefrigeratorTruck fridge = new("REF-002", -10, 5);
        PerishableCargo good = new("Food", 1, 0.1, 1, DateTime.Now.AddDays(1), 4);
        PerishableCargo bad = new("Frozen", 1, 0.1, 1, DateTime.Now.AddDays(1), -20);
        Assert.True(fridge.CanCarry(good));
        Assert.False(fridge.CanCarry(bad));
    }

    [Fact]
    public void VehicleEquals()
    {
        Truck truck = new("TRK-005");
        Vehicle same = truck;
        Assert.Equal(truck, same);
        Assert.Equal(truck.GetHashCode(), same.GetHashCode());
    }
}
