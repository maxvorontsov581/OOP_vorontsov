using OOP.Domain;

namespace OOP.App;

public static class SimpleMenu
{
    public static void Run(DeliveryService service)
    {
        DeliveryService current = service;

        while (true)
        {
            PrintMenu(current);
            string? command = Console.ReadLine();

            if (command == "0" || command is null)
                break;

            try
            {
                switch (command)
                {
                    case "1":
                        CreateOrder(current);
                        break;
                    case "2":
                        ShowOrders(current);
                        break;
                    case "3":
                        AssignOrder(current);
                        break;
                    case "4":
                        AddServicesToOrder(current);
                        break;
                    case "5":
                        StartOrder(current);
                        break;
                    case "6":
                        CompleteOrder(current);
                        break;
                    case "7":
                        CancelOrder(current);
                        break;
                    case "8":
                        PrintReports(current);
                        break;
                    case "9":
                        ShowFleet(current);
                        break;
                    case "10":
                        Maintenance(current);
                        break;
                    case "11":
                        ChangeTariff(current);
                        break;
                    case "12":
                        JsonStorage.Save("state.json", current);
                        Console.WriteLine("Состояние сохранено в state.json");
                        break;
                    case "13":
                        current = JsonStorage.Load("state.json");
                        Console.WriteLine($"Загружено: {current.Orders.Count} заказов, {current.Vehicles.Count} ТС");
                        break;
                    default:
                        Console.WriteLine("Неизвестная команда.");
                        break;
                }
            }
            catch (LogisticsException ex)
            {
                Console.WriteLine("Ошибка логистики: " + ex.Message);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Ошибка данных: " + ex.Message);
            }
        }
    }

    private static void PrintMenu(DeliveryService service)
    {
        Console.WriteLine();
        Console.WriteLine("================ MENU ================");
        Console.WriteLine($"Заказов: {service.Orders.Count}, транспорт: {service.Vehicles.Count}, выручка: {service.Revenue:0.00}");
        Console.WriteLine($"Текущий тариф: {service.TariffName}");
        Console.WriteLine("1  - создать простой заказ");
        Console.WriteLine("2  - показать заказы");
        Console.WriteLine("3  - подобрать транспорт первому новому заказу");
        Console.WriteLine("4  - добавить услуги первому назначенному заказу");
        Console.WriteLine("5  - запустить первую назначенную доставку");
        Console.WriteLine("6  - завершить первую доставку в пути");
        Console.WriteLine("7  - отменить первый доступный заказ");
        Console.WriteLine("8  - отчеты");
        Console.WriteLine("9  - показать парк");
        Console.WriteLine("10 - обслуживание транспорта");
        Console.WriteLine("11 - сменить тариф");
        Console.WriteLine("12 - сохранить JSON");
        Console.WriteLine("13 - загрузить JSON");
        Console.WriteLine("0  - выход");
        Console.Write("Команда: ");
    }

    private static void CreateOrder(DeliveryService service)
    {
        Console.Write("Имя клиента (Enter = Клиент): ");
        string name = Console.ReadLine() ?? "";
        if (string.IsNullOrWhiteSpace(name))
            name = "Клиент";

        Console.Write("Тип груза standard/fragile/dangerous/perishable/oversized (Enter = standard): ");
        string type = Console.ReadLine() ?? "";
        if (string.IsNullOrWhiteSpace(type))
            type = "standard";

        double weight = ReadDouble("Вес, кг (Enter = 10): ", 10);
        double volume = ReadDouble("Объем, м3 (Enter = 0.1): ", 0.1);
        double distance = ReadDouble("Примерная дистанция, км (Enter = 20): ", 20);

        Customer customer = new(name, "console");
        Cargo cargo = CargoFactory.Create(type, "Груз из меню", weight, volume, 10000);

        Route route = new(new[]
        {
            new RoutePoint(59.93, 30.31),
            new RoutePoint(59.93 + distance / 111.0, 30.31)
        });

        Order order = service.CreateOrder(customer, new[] { cargo }, route);
        Console.WriteLine("Создан заказ: " + order);
    }

    private static double ReadDouble(string text, double defaultValue)
    {
        Console.Write(text);
        string? raw = Console.ReadLine();

        if (double.TryParse(raw, out double n))
        {
            if (n > 0)
                return n;
        }

        return defaultValue;
    }

    private static void ShowOrders(DeliveryService service)
    {
        if (service.Orders.Count == 0)
        {
            Console.WriteLine("Заказов нет.");
            return;
        }

        foreach (Order order in service.Orders.OrderBy(x => x.CreatedAt))
        {
            string vehicle = order.AssignedVehicle?.RegistrationNumber ?? "-";
            Console.WriteLine($"{order} | ТС: {vehicle} | грузов: {order.Cargo.Count} | услуг: {order.Services}");
        }
    }

    private static void AssignOrder(DeliveryService service)
    {
        Order? order = service.Orders.FirstOrDefault(x => x.Status == OrderStatus.Created);
        if (order is null)
        {
            Console.WriteLine("Нет новых заказов.");
            return;
        }

        Vehicle vehicle = service.AssignBest(order);
        Console.WriteLine($"Назначен {vehicle}. Цена: {order.TotalCost:0.00}");
    }

    private static void AddServicesToOrder(DeliveryService service)
    {
        Order? order = service.Orders.FirstOrDefault(x => x.Status == OrderStatus.Assigned);
        if (order is null)
        {
            Console.WriteLine("Нет назначенного заказа.");
            return;
        }

        ExtraServices extras = ExtraServices.Insurance | ExtraServices.Urgent;
        if (order.Cargo.Any(x => x is FragileCargo))
            extras |= ExtraServices.FragilePacking;

        IDeliveryCost calculation = service.AddServices(order, extras);
        Console.WriteLine(calculation.Describe());
    }

    private static void StartOrder(DeliveryService service)
    {
        Order? order = service.Orders.FirstOrDefault(x => x.Status == OrderStatus.Assigned);
        if (order is null)
        {
            Console.WriteLine("Нет назначенного заказа.");
            return;
        }

        service.StartDelivery(order);
        Console.WriteLine("Доставка запущена: " + order.Id.ToString()[..8]);
    }

    private static void CompleteOrder(DeliveryService service)
    {
        Order? order = service.Orders.FirstOrDefault(x => x.Status == OrderStatus.InTransit);
        if (order is null)
        {
            Console.WriteLine("Нет доставки в пути.");
            return;
        }

        service.CompleteDelivery(order);
        Console.WriteLine("Доставка завершена: " + order.Id.ToString()[..8]);
    }

    private static void CancelOrder(DeliveryService service)
    {
        Order? order = service.Orders.FirstOrDefault(x =>
            x.Status == OrderStatus.Created || x.Status == OrderStatus.Assigned);

        if (order is null)
        {
            Console.WriteLine("Нет заказа, который можно отменить.");
            return;
        }

        service.CancelOrder(order);
        Console.WriteLine("Заказ отменен: " + order.Id.ToString()[..8]);
    }

    private static void ShowFleet(DeliveryService service)
    {
        foreach (Vehicle vehicle in service.Vehicles.OrderBy(x => x.RegistrationNumber))
        {
            Console.WriteLine($"{vehicle} | max {vehicle.MaxLoadKg:0} кг | {vehicle.MaxVolumeM3:0.##} м3 | {vehicle.Conditions}");
        }
    }

    private static void Maintenance(DeliveryService service)
    {
        Vehicle? vehicle = service.Vehicles.FirstOrDefault(x => x.State != VehicleState.InTransit);
        if (vehicle is null)
        {
            Console.WriteLine("Сейчас нет транспорта, состояние которого можно поменять.");
            return;
        }

        if (vehicle.State == VehicleState.UnderMaintenance)
        {
            service.EndMaintenance(vehicle);
            Console.WriteLine(vehicle.RegistrationNumber + " возвращен в парк.");
        }
        else
        {
            service.StartMaintenance(vehicle);
            Console.WriteLine(vehicle.RegistrationNumber + " отправлен на обслуживание.");
        }
    }

    private static void ChangeTariff(DeliveryService service)
    {
        Console.Write("1-Standard, 2-Express, 3-Heavy: ");
        string? value = Console.ReadLine();

        ITariffStrategy tariff;
        if (value == "2")
            tariff = new ExpressTariff();
        else if (value == "3")
            tariff = new HeavyCargoTariff();
        else
            tariff = new StandardTariff();

        service.SetTariff(tariff);
        Console.WriteLine("Установлен тариф: " + tariff.Name);
    }

    private static void PrintReports(DeliveryService service)
    {
        Print("Заказы по статусам", Reports.OrdersByStatus(service.Orders));
        Print("Топ транспорта", Reports.TopVehicles(service.Orders));
        Print("Средняя загрузка", Reports.AverageLoad(service.Orders));
        Print("Клиенты с суммой > 1000", Reports.CustomersAbove(service.Customers, 1000));
        Print("Груз -> заказ -> клиент", Reports.CargoWithCustomers(service.Orders).Take(10));
        Print("Самые длинные маршруты", Reports.LongRoutes(service.Orders));

        Console.WriteLine("Опасные грузы по классам:");
        foreach (KeyValuePair<int, int> pair in Reports.DangerousByClass(service.Orders))
            Console.WriteLine($"  {pair.Key} -> {pair.Value}");
    }

    private static void Print(string title, IEnumerable<string> lines)
    {
        Console.WriteLine(title + ":");
        foreach (string line in lines)
            Console.WriteLine("  " + line);
    }
}
