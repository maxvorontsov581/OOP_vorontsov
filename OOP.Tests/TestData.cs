using OOP.Domain;

namespace OOP.Tests;

internal static class TestData
{
    public static Route Route100Km()
        => new(new[] { new RoutePoint(0, 0), new RoutePoint(0.9, 0) });

    public static Route ShortRoute()
        => new(new[] { new RoutePoint(59.93, 30.31), new RoutePoint(59.94, 30.32) });

    public static Route ZeroRoute()
        => new(new[] { new RoutePoint(10, 10), new RoutePoint(10, 10) });

    public static StandardCargo SmallCargo()
        => new("Box", 10, 0.1, 1000);

    public static Customer Customer(string name = "Test") => new(name, "test");
}
