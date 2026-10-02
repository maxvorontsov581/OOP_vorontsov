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
