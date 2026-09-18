using OOP.Domain;
using Xunit;

namespace OOP.Tests;

public class BasicTests
{
    [Fact]
    public void OrderStates()
    {
        Order order = new(TestData.Customer(), new[] { TestData.SmallCargo() }, TestData.Route100Km());
        Truck truck = new("TRK-020");
        order.Assign(truck, 100);
        order.StartDelivery();
        order.Complete();
        Assert.Equal(OrderStatus.Delivered, order.Status);
    }

    [Fact]
    public void BadOrderStart()
    {
        Order order = new(TestData.Customer(), new[] { TestData.SmallCargo() }, TestData.Route100Km());
        Assert.Throws<InvalidOrderStateException>(() => order.StartDelivery());
    }

    [Fact]
    public void CancelOrder()
    {
        Order order = new(TestData.Customer(), new[] { TestData.SmallCargo() }, TestData.Route100Km());
        order.Cancel();
        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    [Fact]
    public void BadCancel()
    {
        Order order = new(TestData.Customer(), new[] { TestData.SmallCargo() }, TestData.Route100Km());
        order.Assign(new Truck("TRK-021"), 100);
        order.StartDelivery();
        order.Complete();
        Assert.Throws<InvalidOrderStateException>(() => order.Cancel());
    }

    [Fact]
    public void RepoAdd()
    {
        Repository<Customer> repo = new();
        Customer customer = TestData.Customer("A");
        repo.Add(customer);
        Assert.Same(customer, repo[customer.Id]);
    }

    [Fact]
    public void RepoSearch()
    {
        Repository<Customer> repo = new();
        Customer a = TestData.Customer("A");
        Customer b = TestData.Customer("B");
        repo.Add(a);
        repo.Add(b);

        Assert.Single(repo.FindAll(x => x.Name == "A"));
        Assert.Equal(2, repo.GetAll().Count());
        Assert.True(repo.Remove(a));
        Assert.Single(repo);
    }

    [Fact]
    public void RepoDuplicate()
    {
        Repository<Customer> repo = new();
        Customer a = TestData.Customer("A");
        repo.Add(a);
        repo.Add(a);
        Assert.Single(repo);
    }
}
