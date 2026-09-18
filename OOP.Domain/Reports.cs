namespace OOP.Domain;

public static class Reports
{
    public static IEnumerable<string> TopVehicles(IEnumerable<Order> orders)
    {
        return orders
            .Where(o => o.Status == OrderStatus.Delivered && o.AssignedVehicle != null)
            .GroupBy(o => o.AssignedVehicle!)
            .Select(g => new { Vehicle = g.Key, Sum = g.Sum(x => x.TotalCost) })
            .OrderByDescending(x => x.Sum)
            .Take(3)
            .Select(x => $"{x.Vehicle.RegistrationNumber}: {x.Sum:0.00}");
    }

    public static IEnumerable<string> OrdersByStatus(IEnumerable<Order> orders)
    {
        return orders
            .GroupBy(o => o.Status)
            .OrderBy(g => g.Key)
            .Select(g => $"{g.Key}: {g.Count()} заказов, сумма {g.Sum(x => x.TotalCost):0.00}");
    }

    public static IEnumerable<string> AverageLoad(IEnumerable<Order> orders)
    {
        return orders
            .Where(o => o.AssignedVehicle != null)
            .GroupBy(o => o.AssignedVehicle!.GetType().Name)
            .Select(g => new
            {
                Type = g.Key,
                Load = g.Average(o => o.Cargo.Sum(c => c.WeightKg) / o.AssignedVehicle!.MaxLoadKg * 100)
            })
            .OrderByDescending(x => x.Load)
            .Select(x => $"{x.Type}: {x.Load:0.0}%");
    }

    public static IEnumerable<string> CustomersAbove(IEnumerable<Customer> customers, decimal threshold)
    {
        return customers
            .Select(c => new { c.Name, Total = c.Orders.Sum(o => o.TotalCost) })
            .Where(x => x.Total > threshold)
            .OrderByDescending(x => x.Total)
            .Select(x => $"{x.Name}: {x.Total:0.00}");
    }

    public static IEnumerable<string> CargoWithCustomers(IEnumerable<Order> orders)
    {
        var orderCargo = orders.SelectMany(order => order.Cargo.Select(cargo => new
        {
            Cargo = cargo,
            OrderId = order.Id,
            CustomerId = order.Customer.Id
        }));

        var customers = orders.Select(o => o.Customer).Distinct();

        var result = from item in orderCargo
                     join customer in customers on item.CustomerId equals customer.Id
                     select $"{item.Cargo.Description} -> заказ {item.OrderId.ToString()[..8]} -> {customer.Name}";
        return result;
    }

    public static Dictionary<int, int> DangerousByClass(IEnumerable<Order> orders)
    {
        return orders
            .SelectMany(o => o.Cargo)
            .OfType<DangerousCargo>()
            .GroupBy(x => x.HazardClass)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    public static ILookup<string, Cargo> CargoByType(IEnumerable<Order> orders)
    {
        return orders
            .SelectMany(o => o.Cargo)
            .ToLookup(c => c.GetType().Name);
    }

    public static IEnumerable<string> VehicleOrders(IEnumerable<Vehicle> vehicles, IEnumerable<Order> orders)
    {
        return vehicles.Join(
            orders.Where(o => o.AssignedVehicle != null),
            v => v.Id,
            o => o.AssignedVehicle!.Id,
            (v, o) => $"{v.RegistrationNumber} -> {o.Customer.Name}");
    }

    public static IEnumerable<string> LongRoutes(IEnumerable<Order> orders)
    {
        return orders
            .OrderByDescending(o => o.Route.DistanceKm)
            .Take(5)
            .Select(o => $"{o.Id.ToString()[..8]}: {o.Route.DistanceKm:0.0} км");
    }

    public static IEnumerable<string> CargoStats(IEnumerable<Order> orders)
    {
        return orders
            .SelectMany(o => o.Cargo)
            .GroupBy(c => c.GetType().Name)
            .OrderByDescending(g => g.Count())
            .Select(g => $"{g.Key}: {g.Count()} шт., {g.Sum(x => x.WeightKg):0.0} кг");
    }

    public static string Summary(DeliveryService service)
    {
        int delivered = service.Orders.Count(o => o.Status == OrderStatus.Delivered);
        int active = service.Orders.Count(o => o.Status == OrderStatus.Assigned || o.Status == OrderStatus.InTransit);
        return $"Всего заказов: {service.Orders.Count}; доставлено: {delivered}; активных: {active}; выручка: {service.Revenue:0.00}";
    }
}
