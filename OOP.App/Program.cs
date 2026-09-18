namespace OOP.App;

internal static class Program
{
    private static void Main()
    {
        Demo demo = new();
        OOP.Domain.DeliveryService service = demo.Run();
        SimpleMenu.Run(service);
    }
}
