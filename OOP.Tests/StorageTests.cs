using OOP.Domain;
using Xunit;

namespace OOP.Tests;

public class StorageTests
{
    [Fact]
    public void SaveLoad()
    {
        string path = Path.Combine(Path.GetTempPath(), $"oop-{Guid.NewGuid()}.json");
        try
        {
            DeliveryService service = MakeService();
            JsonStorage.Save(path, service);
            DeliveryService loaded = JsonStorage.Load(path);

            Assert.Equal(1, loaded.Vehicles.Count);
            Assert.Equal(1, loaded.Customers.Count);
            Assert.Equal(1, loaded.Orders.Count);
            Assert.Equal(OrderStatus.Delivered, loaded.Orders.First().Status);
            Assert.Equal(service.Revenue, loaded.Revenue);
            Assert.Equal(service.Orders.First().Services, loaded.Orders.First().Services);
        }
        finally
        {
            if (File.Exists(path))
                File.Delete(path);
        }
    }

    [Fact]
    public void MissingFile()
    {
        DeliveryService loaded = JsonStorage.Load(Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json"));
        Assert.Equal(0, loaded.Orders.Count);
    }

    [Fact]
    public void BrokenJson()
    {
        string path = Path.Combine(Path.GetTempPath(), $"oop-bad-{Guid.NewGuid()}.json");
        try
        {
            File.WriteAllText(path, "{ broken json");
            DeliveryService loaded = JsonStorage.Load(path);
            Assert.Equal(0, loaded.Orders.Count);
        }
        finally
        {
            if (File.Exists(path))
                File.Delete(path);
        }
    }

    [Fact]
    public void StatusReport()
    {
        DeliveryService service = MakeService();
        Assert.Contains(Reports.OrdersByStatus(service.Orders), x => x.Contains("Delivered"));
    }

    [Fact]
    public void DangerousReport()
    {
        DeliveryService service = new();
        Customer customer = TestData.Customer();
        service.CreateOrder(customer,
            new Cargo[] { new DangerousCargo("A", 1, 0.1, 1, 5), new DangerousCargo("B", 1, 0.1, 1, 5) },
            TestData.ShortRoute());

        Dictionary<int, int> result = Reports.DangerousByClass(service.Orders);
        Assert.Equal(2, result[5]);
    }

    [Fact]
    public void SummaryReport()
    {
        DeliveryService service = MakeService();
        string text = Reports.Summary(service);
        Assert.Contains("выручка", text.ToLowerInvariant());
    }

    private static DeliveryService MakeService()
    {
        DeliveryService service = new();
        Truck truck = new("TRK-777");
        service.AddVehicle(truck);
        Order order = service.CreateOrder(TestData.Customer("Round"),
            new Cargo[] { new FragileCargo("Glass", 5, 0.1, 10000, 1.1) }, TestData.Route100Km());
        service.AssignBest(order);
        service.AddServices(order, ExtraServices.Insurance);
        service.StartDelivery(order);
        service.CompleteDelivery(order);
        return service;
    }
}
