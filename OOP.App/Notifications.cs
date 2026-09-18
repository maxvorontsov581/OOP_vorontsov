using OOP.Domain;

namespace OOP.App;

public sealed class ConsoleNotifier
{
    public void OnOrderCreated(object sender, OrderCreatedEventArgs e)
        => Console.WriteLine($"[EVENT] Создан заказ {Short(e.Order.Id)}");

    public void OnStatusChanged(object sender, OrderStatusChangedEventArgs e)
        => Console.WriteLine($"[EVENT] {Short(e.Order.Id)}: {e.OldStatus} -> {e.NewStatus}");

    public void OnOverload(object sender, VehicleOverloadAttemptEventArgs e)
        => Console.WriteLine($"[EVENT] Перегруз {e.Vehicle.RegistrationNumber}: {e.TotalWeightKg:0.##} кг");

    public void OnCompleted(object sender, DeliveryCompletedEventArgs e)
        => Console.WriteLine($"[EVENT] Доставка завершена. Выручка +{e.Revenue:0.00}");

    private static string Short(Guid id) => id.ToString()[..8];
}

public sealed class FileLogger
{
    private readonly string _path;

    public FileLogger(string path)
    {
        _path = path;
    }

    public void OnOrderCreated(object sender, OrderCreatedEventArgs e)
        => Write($"Created {e.Order.Id}");

    public void OnStatusChanged(object sender, OrderStatusChangedEventArgs e)
        => Write($"Status {e.Order.Id}: {e.OldStatus} -> {e.NewStatus}");

    public void OnOverload(object sender, VehicleOverloadAttemptEventArgs e)
        => Write($"Overload {e.Vehicle.RegistrationNumber}: {e.TotalWeightKg:0.##} kg");

    public void OnCompleted(object sender, DeliveryCompletedEventArgs e)
        => Write($"Completed {e.Order.Id}: {e.Revenue:0.00}");

    private void Write(string text)
    {
        using StreamWriter writer = new(_path, append: true);
        writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {text}");
    }
}
