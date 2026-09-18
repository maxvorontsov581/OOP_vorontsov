using OOP.Domain;

namespace OOP.App;

public sealed class Demo
{
    public DeliveryService Run()
    {
        Console.WriteLine("=== Демо ===\n");

        DeliveryService service = new(new StandardTariff());
        ConsoleNotifier console = new();
        FileLogger file = new("deliveries.log");
        Subscribe(service, console, file);

        CreateFleet(service);
        List<Cargo> cargo = CreateCargo();

        Customer ivan = new("Иван", "+7-900-000-00-01");
        Customer anna = new("Анна", "anna@example.test");
        Customer oleg = new("Олег", "+7-900-000-00-03");

        Route longRoute = new(new[]
        {
            new RoutePoint(59.93, 30.31),
            new RoutePoint(55.75, 37.61)
        });
        Route shortRoute = new(new[]
        {
            new RoutePoint(59.93, 30.31),
            new RoutePoint(59.95, 30.35)
        });

        Console.WriteLine($"Маршрут СПб -> Москва, примерно {longRoute.DistanceKm:0} км.");
        Console.WriteLine($"Время на обычной фуре примерно {longRoute.EstimateTime(service.Vehicles.OfType<Truck>().First()):g}.");

        ShowInterfaces(service, cargo);
        ShowBadOrder(service, ivan, cargo, longRoute);

        List<Order> orders = CreateOrders(service, ivan, anna, oleg, cargo, longRoute, shortRoute);
        ShowOverload(service, ivan, longRoute);
        ShowMaintenance(service);

        RunDelivery(service, orders[0]);
        RunOtherOrders(service, orders);

        PrintReports(service);
        DeliveryService loaded = SaveLoad(service);

        service.OrderCreated -= console.OnOrderCreated;
        Console.WriteLine("\nConsoleNotifier отписан от OrderCreated.");

        ShowExceptions();
        Console.WriteLine("\n=== Конец демо ===");
        return loaded;
    }

    private static void Subscribe(DeliveryService service, ConsoleNotifier console, FileLogger file)
    {
        service.OrderCreated += console.OnOrderCreated;
        service.OrderStatusChanged += console.OnStatusChanged;
        service.VehicleOverloadAttempt += console.OnOverload;
        service.DeliveryCompleted += console.OnCompleted;

        service.OrderCreated += file.OnOrderCreated;
        service.OrderStatusChanged += file.OnStatusChanged;
        service.VehicleOverloadAttempt += file.OnOverload;
        service.DeliveryCompleted += file.OnCompleted;
    }

    private static void CreateFleet(DeliveryService service)
    {
        string[] types = { "truck", "truck", "refrigerator", "plane", "ship", "drone" };
        VehicleFactory factory = new();

        for (int i = 0; i < types.Length; i++)
            service.AddVehicle(factory.Create(types[i], $"CAR-{i + 1:000}"));

        Console.WriteLine("Парк создан: " + service.Vehicles.Count + " ТС");
        foreach (Vehicle vehicle in service.Vehicles)
            Console.WriteLine("  " + vehicle);
    }

    private static List<Cargo> CreateCargo()
    {
        List<Cargo> result = new()
        {
            CargoFactory.Create("standard", "Книги", 50, 0.5, 20000),
            CargoFactory.Create("fragile", "Монитор", 12, 0.1, 60000),
            CargoFactory.Create("dangerous", "Химреактив", 30, 0.2, 40000),
            CargoFactory.Create("oversized", "Станок", 3000, 8, 900000),
            CargoFactory.Create("perishable", "Молочная продукция", 400, 3, 150000),
            CargoFactory.Create("standard", "Одежда", 70, 1, 100000),
            CargoFactory.Create("fragile", "Стекло", 200, 2, 180000),
            CargoFactory.Create("dangerous", "Краска", 80, 0.8, 50000),
            CargoFactory.Create("oversized", "Трубы", 4500, 12, 300000),
            CargoFactory.Create("standard", "Документы", 2, 0.02, 5000)
        };

        Console.WriteLine("Грузов для демо создано: " + result.Count);
        return result;
    }

    private static void ShowInterfaces(DeliveryService service, List<Cargo> cargo)
    {
        Console.WriteLine("\n--- Интерфейсы, вариантность и Flags ---");

        RefrigeratorTruck fridge = service.Vehicles.OfType<RefrigeratorTruck>().First();
        bool refrigerated = (fridge.Conditions & TransportConditions.Refrigerated) != 0;
        Console.WriteLine($"Flags: у рефрижератора Refrigerated = {refrigerated}");

        IValidator<Cargo> cargoValidator = new CargoValidator();
        IValidator<PerishableCargo> perishableValidator = cargoValidator;
        ValidationResult check = perishableValidator.Validate((PerishableCargo)cargo[4]);
        Console.WriteLine("Contravariance: скоропортящийся груз валиден = " + check.IsValid);

        Repository<Truck> truckRepo = new();
        truckRepo.Add(new Truck("TMP-001"));
        IReadOnlyRepository<Truck> trucks = truckRepo;
        IReadOnlyRepository<Vehicle> vehicles = trucks;
        Console.WriteLine("Covariance: транспорта = " + vehicles.GetAll().Count());

        IInsurable insurable = (IInsurable)cargo[1];
        Console.WriteLine("Explicit interface IInsurable: страховая величина = " + insurable.InsuranceValue.ToString("0.00"));
    }

    private static void ShowBadOrder(DeliveryService service, Customer customer, List<Cargo> cargo, Route route)
    {
        Console.WriteLine("\n--- Несовместимые грузы ---");
        try
        {
            service.CreateOrder(customer, new[] { cargo[2], cargo[4] }, route);
        }
        catch (IncompatibleCargoException ex) when (ex.Message.Contains("нельзя"))
        {
            Console.WriteLine("Ожидаемая ошибка: " + ex.Message);
        }
    }

    private static List<Order> CreateOrders(DeliveryService service, Customer ivan, Customer anna,
        Customer oleg, List<Cargo> cargo, Route longRoute, Route shortRoute)
    {
        Console.WriteLine("\n--- Создание заказов ---");

        return new List<Order>
        {
            service.CreateOrder(ivan, new[] { cargo[0], cargo[1] }, longRoute),
            service.CreateOrder(anna, new[] { cargo[4] }, longRoute),
            service.CreateOrder(oleg, new[] { cargo[2] }, longRoute),
            service.CreateOrder(ivan, new[] { cargo[3] }, longRoute),
            service.CreateOrder(anna, new[] { cargo[9] }, shortRoute)
        };
    }

    private static void ShowOverload(DeliveryService service, Customer customer, Route route)
    {
        Console.WriteLine("\n--- Попытка перегруза дрона ---");
        Order heavy = service.CreateOrder(customer,
            new[] { new StandardCargo("Тяжелая коробка", 100, 0.1, 1000) }, route);
        DroneCourier drone = service.Vehicles.OfType<DroneCourier>().First();

        try
        {
            service.AssignVehicle(heavy, drone);
        }
        catch (VehicleOverloadException ex)
        {
            Console.WriteLine("Ожидаемая ошибка: " + ex.Message);
        }
    }

    private static void ShowMaintenance(DeliveryService service)
    {
        Console.WriteLine("\n--- Состояние транспорта ---");
        Truck truck = service.Vehicles.OfType<Truck>().First(x => x is not RefrigeratorTruck);
        service.StartMaintenance(truck);
        Console.WriteLine($"{truck.RegistrationNumber}: {truck.State}");
        service.EndMaintenance(truck);
        Console.WriteLine($"{truck.RegistrationNumber}: {truck.State}");
    }

    private static void RunDelivery(DeliveryService service, Order order)
    {
        Console.WriteLine("\n--- Доставка ---");
        Vehicle selected = service.AssignBest(order);
        Console.WriteLine($"Выбран транспорт: {selected}");
        Console.WriteLine($"Стоимость доставки: {order.TotalCost:0.00}");

        ExtraServices services = ExtraServices.Insurance |
                                         ExtraServices.Urgent |
                                         ExtraServices.FragilePacking;

        IDeliveryCost finalCost = service.AddServices(order, services);
        Console.WriteLine("Дополнительные услуги:");
        Console.WriteLine(finalCost.Describe());
        Console.WriteLine("Итого: " + order.TotalCost.ToString("0.00"));

        service.StartDelivery(order);
        service.CompleteDelivery(order);
    }

    private static void RunOtherOrders(DeliveryService service, List<Order> orders)
    {
        for (int i = 1; i <= 2; i++)
        {
            try
            {
                service.AssignBest(orders[i]);
                service.StartDelivery(orders[i]);
                service.CompleteDelivery(orders[i]);
            }
            catch (LogisticsException ex)
            {
                Console.WriteLine("Заказ не удалось провести полностью: " + ex.Message);
            }
        }

        try
        {
            service.AssignBest(orders[3]);
        }
        catch (LogisticsException ex)
        {
            Console.WriteLine("Четвертый заказ остался новым: " + ex.Message);
        }
    }

    private static void PrintReports(DeliveryService service)
    {
        Console.WriteLine("\n=== Отчеты ===");
        Console.WriteLine(Reports.Summary(service));
        Print("1. Топ транспорта", Reports.TopVehicles(service.Orders));
        Print("2. Заказы по статусам", Reports.OrdersByStatus(service.Orders));
        Print("3. Средняя загрузка", Reports.AverageLoad(service.Orders));
        Print("4. Клиенты > 1000", Reports.CustomersAbove(service.Customers, 1000));
        Print("5. Груз -> заказ -> клиент", Reports.CargoWithCustomers(service.Orders).Take(10));

        Console.WriteLine("6. Опасные грузы по классу:");
        foreach (KeyValuePair<int, int> pair in Reports.DangerousByClass(service.Orders))
            Console.WriteLine($"  класс {pair.Key}: {pair.Value}");

        Print("7. Самые длинные маршруты", Reports.LongRoutes(service.Orders));
        Print("8. Статистика типов груза", Reports.CargoStats(service.Orders));

        ILookup<string, Cargo> lookup = Reports.CargoByType(service.Orders);
        Console.WriteLine("Типов грузов: " + lookup.Count);
        Console.WriteLine("Заказов с транспортом: " + Reports.VehicleOrders(service.Vehicles, service.Orders).Count());

        Console.WriteLine("Первые два ТС:");
        Console.WriteLine(service.Vehicles.Take(2).ToReportTable());
    }

    private static void Print(string title, IEnumerable<string> lines)
    {
        Console.WriteLine(title + ":");
        foreach (string line in lines)
            Console.WriteLine("  " + line);
    }

    private static DeliveryService SaveLoad(DeliveryService service)
    {
        const string path = "state.json";
        Console.WriteLine("\n=== Сериализация ===");

        try
        {
            int beforeOrders = service.Orders.Count;
            int beforeVehicles = service.Vehicles.Count;
            decimal beforeRevenue = service.Revenue;

            JsonStorage.Save(path, service);
            Console.WriteLine("Состояние сохранено.");

            service.Orders.Clear();
            service.Customers.Clear();
            service.Vehicles.Clear();
            Console.WriteLine($"После очистки: заказов {service.Orders.Count}, ТС {service.Vehicles.Count}");

            DeliveryService loaded = JsonStorage.Load(path);
            Console.WriteLine($"После загрузки: заказов {loaded.Orders.Count}/{beforeOrders}, ТС {loaded.Vehicles.Count}/{beforeVehicles}");
            Console.WriteLine($"Выручка: {loaded.Revenue:0.00}/{beforeRevenue:0.00}");
            Console.WriteLine("Повторный отчет: " + Reports.Summary(loaded));
            return loaded;
        }
        finally
        {
            Console.WriteLine("Работа с файлом завершена.");
        }
    }

    private static void ShowExceptions()
    {
        Console.WriteLine("\n--- throw; и finally ---");
        try
        {
            Rethrow();
        }
        catch (CargoValidationException)
        {
            Console.WriteLine("Показан корректный повторный проброс через throw;.");
        }
        finally
        {
            Console.WriteLine("finally отработал.");
        }
    }

    private static void Rethrow()
    {
        try
        {
            _ = new StandardCargo("Ошибка", 0, 1, 1);
        }
        catch (CargoValidationException)
        {
            throw;
        }
    }
}
