using OOP.Domain;
using Xunit;

namespace OOP.Tests;

public class ServiceTests
{
    [Fact]
    public void Decorators()
    {
        Cargo[] cargo = { new FragileCargo("Glass", 1, 0.1, 10000, 1.2) };
        IDeliveryCost cost = new BaseDeliveryCost(1000, "base");
        cost = new InsuranceDecorator(cost, cargo);
        cost = new UrgencyDecorator(cost, 1.2m);
        cost = new FragilePackingDecorator(cost, cargo);
        Assert.True(cost.Total > 1000);
    }

    [Fact]
    public void DecoratorOrder()
    {
        Cargo[] cargo = { new FragileCargo("Glass", 1, 0.1, 10000, 1.2) };
        IDeliveryCost a = new UrgencyDecorator(new InsuranceDecorator(new BaseDeliveryCost(1000, "a"), cargo), 2m);
        IDeliveryCost b = new InsuranceDecorator(new UrgencyDecorator(new BaseDeliveryCost(1000, "b"), 2m), cargo);
        Assert.NotEqual(a.Total, b.Total);
    }

    [Fact]
    public void ExpressCost()
    {
        Truck truck = new("TRK-030");
        Order order = new(TestData.Customer(), new[] { TestData.SmallCargo() }, TestData.Route100Km());
        DeliveryService standard = new(new StandardTariff());
        DeliveryService express = new(new ExpressTariff());
        Assert.True(express.CalculateCost(truck, order) > standard.CalculateCost(truck, order));
    }

    [Fact]
    public void Factory()
    {
        VehicleFactory factory = new();
        Assert.IsType<CargoShip>(factory.Create("ship", "SHP-030"));
    }

    [Fact]
    public void AutoAssign()
    {
        DeliveryService service = new();
        service.AddVehicle(new Truck("TRK-031"));
        service.AddVehicle(new CargoPlane("PLN-031"));
        Order order = service.CreateOrder(TestData.Customer(), new[] { TestData.SmallCargo() }, TestData.Route100Km());

        Vehicle selected = service.AssignBest(order);

        Assert.NotNull(selected);
        Assert.Equal(OrderStatus.Assigned, order.Status);
        Assert.NotNull(order.AssignedVehicle);
    }

    [Fact]
    public void ServicesCost()
    {
        DeliveryService service = new();
        service.AddVehicle(new Truck("TRK-032"));
        Order order = service.CreateOrder(TestData.Customer(),
            new Cargo[] { new FragileCargo("Glass", 2, 0.1, 10000, 1.2) }, TestData.Route100Km());
        service.AssignBest(order);
        decimal before = order.TotalCost;

        service.AddServices(order,
            ExtraServices.Insurance | ExtraServices.Urgent | ExtraServices.FragilePacking);

        Assert.True(order.TotalCost > before);
        Assert.NotEqual(ExtraServices.None, order.Services);
    }

    [Fact]
    public void CompleteDelivery()
    {
        DeliveryService service = new();
        Truck truck = new("TRK-033");
        service.AddVehicle(truck);
        Order order = service.CreateOrder(TestData.Customer(), new[] { TestData.SmallCargo() }, TestData.Route100Km());
        service.AssignBest(order);
        service.StartDelivery(order);
        service.CompleteDelivery(order);

        Assert.Equal(OrderStatus.Delivered, order.Status);
        Assert.Equal(VehicleState.Free, truck.State);
        Assert.Equal(order.TotalCost, service.Revenue);
    }

    [Fact]
    public void Maintenance()
    {
        DeliveryService service = new();
        Truck unavailable = new("TRK-034");
        Truck available = new("TRK-035");
        service.AddVehicle(unavailable);
        service.AddVehicle(available);
        service.StartMaintenance(unavailable);
        Order order = service.CreateOrder(TestData.Customer(), new[] { TestData.SmallCargo() }, TestData.Route100Km());

        Vehicle selected = service.AssignBest(order);

        Assert.Same(available, selected);
    }

    [Fact]
    public void OverloadEvent()
    {
        DeliveryService service = new();
        DroneCourier drone = new("DRN-032");
        service.AddVehicle(drone);
        Order order = service.CreateOrder(TestData.Customer(),
            new[] { new StandardCargo("Heavy", 50, 0.1, 100) }, TestData.ShortRoute());
        bool eventRaised = false;
        service.VehicleOverloadAttempt += (_, _) => eventRaised = true;

        Assert.Throws<VehicleOverloadException>(() => service.AssignVehicle(order, drone));
        Assert.True(eventRaised);
    }
}
