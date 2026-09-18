using System.Text.Json;

namespace OOP.Domain;

public static class JsonStorage
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };

    public static void Save(string path, DeliveryService service)
    {
        SaveData snapshot = new()
        {
            Revenue = service.Revenue,
            Vehicles = service.Vehicles.Select(ToData).ToList(),
            Customers = service.Customers.Select(c => new CustomerData
            {
                Id = c.Id,
                Name = c.Name,
                Contact = c.Contact
            }).ToList(),
            Orders = service.Orders.Select(ToData).ToList()
        };

        using FileStream stream = File.Create(path);
        JsonSerializer.Serialize(stream, snapshot, Options);
    }

    public static DeliveryService Load(string path)
    {
        if (!File.Exists(path))
            return new DeliveryService();

        try
        {
            using FileStream stream = File.OpenRead(path);
            SaveData? snapshot = JsonSerializer.Deserialize<SaveData>(stream, Options);
            if (snapshot is null)
                return new DeliveryService();

            return Restore(snapshot);
        }
        catch (JsonException)
        {
            return new DeliveryService();
        }
        catch (IOException)
        {
            return new DeliveryService();
        }
    }

    private static DeliveryService Restore(SaveData snapshot)
    {
        DeliveryService service = new();
        Dictionary<Guid, Vehicle> vehicles = new();
        Dictionary<Guid, Customer> customers = new();

        foreach (VehicleData v in snapshot.Vehicles)
        {
            Vehicle vehicle = FromData(v);
            service.AddVehicle(vehicle);
            vehicles[vehicle.Id] = vehicle;
        }

        foreach (CustomerData c in snapshot.Customers)
        {
            Customer customer = new(c.Id, c.Name, c.Contact);
            service.AddCustomer(customer);
            customers[customer.Id] = customer;
        }

        foreach (OrderData o in snapshot.Orders)
        {
            if (!customers.TryGetValue(o.CustomerId, out Customer? customer))
                continue;

            if (o.Points.Count < 2)
                continue;

            Route route = new(o.Points.Select(p => new RoutePoint(p.Latitude, p.Longitude)));
            List<Cargo> cargo = o.Cargo.Select(FromData).ToList();
            Vehicle? assigned = null;
            if (o.AssignedVehicleId.HasValue)
                vehicles.TryGetValue(o.AssignedVehicleId.Value, out assigned);

            Order order = new(o.Id, customer, cargo, route, o.Status, assigned, o.TotalCost,
                o.CreatedAt == default ? DateTime.Now : o.CreatedAt, o.Services);
            service.Orders.Add(order);
        }

        service.RestoreRevenue(snapshot.Revenue);
        return service;
    }

    private static VehicleData ToData(Vehicle v)
    {
        VehicleData s = new()
        {
            Id = v.Id,
            Type = v.GetType().Name,
            RegistrationNumber = v.RegistrationNumber,
            State = v.State,
            MaxLoadKg = v.MaxLoadKg,
            MaxVolumeM3 = v.MaxVolumeM3,
            Speed = v.AverageSpeedKmH,
            Rate = v.BaseRatePerKm
        };

        if (v is Truck t)
            s.TollCoefficient = t.TollRoadCoefficient;
        if (v is RefrigeratorTruck r)
        {
            s.MinTemperatureC = r.MinTemperatureC;
            s.MaxTemperatureC = r.MaxTemperatureC;
        }
        if (v is CargoPlane p)
            s.PricePerKg = p.PricePerKg;
        if (v is DroneCourier d)
            s.MaxDistanceKm = d.MaxDistanceKm;

        return s;
    }

    private static Vehicle FromData(VehicleData s)
    {
        return s.Type switch
        {
            nameof(RefrigeratorTruck) => new RefrigeratorTruck(s.Id, s.RegistrationNumber, s.State,
                s.MinTemperatureC ?? -20, s.MaxTemperatureC ?? 10),
            nameof(CargoPlane) => new CargoPlane(s.Id, s.RegistrationNumber, s.State, s.PricePerKg ?? 2.5m),
            nameof(CargoShip) => new CargoShip(s.Id, s.RegistrationNumber, s.State),
            nameof(DroneCourier) => new DroneCourier(s.Id, s.RegistrationNumber, s.State, s.MaxDistanceKm ?? 50),
            _ => new Truck(s.Id, s.RegistrationNumber, s.MaxLoadKg, s.MaxVolumeM3, s.Speed, s.Rate,
                s.State, s.TollCoefficient ?? 1.10m)
        };
    }

    private static OrderData ToData(Order o)
    {
        return new OrderData
        {
            Id = o.Id,
            CustomerId = o.Customer.Id,
            AssignedVehicleId = o.AssignedVehicle?.Id,
            Status = o.Status,
            TotalCost = o.TotalCost,
            CreatedAt = o.CreatedAt,
            Services = o.Services,
            Points = o.Route.Points.Select(p => new PointData
            {
                Latitude = p.Latitude,
                Longitude = p.Longitude
            }).ToList(),
            Cargo = o.Cargo.Select(ToData).ToList()
        };
    }

    private static CargoData ToData(Cargo c)
    {
        CargoData s = new()
        {
            Id = c.Id,
            Type = c.GetType().Name,
            Description = c.Description,
            WeightKg = c.WeightKg,
            VolumeM3 = c.VolumeM3,
            DeclaredValue = c.DeclaredValue
        };

        if (c is PerishableCargo p)
        {
            s.ExpirationDate = p.ExpirationDate;
            s.RequiredTemperatureC = p.RequiredTemperatureC;
        }
        if (c is FragileCargo f)
            s.RiskFactor = f.RiskFactor;
        if (c is DangerousCargo d)
            s.HazardClass = d.HazardClass;

        return s;
    }

    private static Cargo FromData(CargoData s)
    {
        return s.Type switch
        {
            nameof(PerishableCargo) => new PerishableCargo(s.Id, s.Description, s.WeightKg, s.VolumeM3,
                s.DeclaredValue, s.ExpirationDate ?? DateTime.Now.AddDays(1), s.RequiredTemperatureC ?? 4),
            nameof(FragileCargo) => new FragileCargo(s.Id, s.Description, s.WeightKg, s.VolumeM3,
                s.DeclaredValue, s.RiskFactor ?? 1.1),
            nameof(DangerousCargo) => new DangerousCargo(s.Id, s.Description, s.WeightKg, s.VolumeM3,
                s.DeclaredValue, s.HazardClass ?? 5),
            nameof(OversizedCargo) => new OversizedCargo(s.Id, s.Description, s.WeightKg, s.VolumeM3, s.DeclaredValue),
            _ => new StandardCargo(s.Id, s.Description, s.WeightKg, s.VolumeM3, s.DeclaredValue)
        };
    }

    private sealed class SaveData
    {
        public SaveData() { }
        public decimal Revenue { get; set; }
        public List<VehicleData> Vehicles { get; set; } = new();
        public List<CustomerData> Customers { get; set; } = new();
        public List<OrderData> Orders { get; set; } = new();
    }

    private sealed class VehicleData
    {
        public VehicleData() { }
        public Guid Id { get; set; }
        public string Type { get; set; } = "";
        public string RegistrationNumber { get; set; } = "";
        public VehicleState State { get; set; }
        public double MaxLoadKg { get; set; }
        public double MaxVolumeM3 { get; set; }
        public double Speed { get; set; }
        public decimal Rate { get; set; }
        public decimal? TollCoefficient { get; set; }
        public double? MinTemperatureC { get; set; }
        public double? MaxTemperatureC { get; set; }
        public decimal? PricePerKg { get; set; }
        public double? MaxDistanceKm { get; set; }
    }

    private sealed class CustomerData
    {
        public CustomerData() { }
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string Contact { get; set; } = "";
    }

    private sealed class OrderData
    {
        public OrderData() { }
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? AssignedVehicleId { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalCost { get; set; }
        public DateTime CreatedAt { get; set; }
        public ExtraServices Services { get; set; }
        public List<PointData> Points { get; set; } = new();
        public List<CargoData> Cargo { get; set; } = new();
    }

    private sealed class PointData
    {
        public PointData() { }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    private sealed class CargoData
    {
        public CargoData() { }
        public Guid Id { get; set; }
        public string Type { get; set; } = "";
        public string Description { get; set; } = "";
        public double WeightKg { get; set; }
        public double VolumeM3 { get; set; }
        public decimal DeclaredValue { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public double? RequiredTemperatureC { get; set; }
        public double? RiskFactor { get; set; }
        public int? HazardClass { get; set; }
    }
}
