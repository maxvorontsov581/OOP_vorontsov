using OOP.Domain;

namespace OOP.App;

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
