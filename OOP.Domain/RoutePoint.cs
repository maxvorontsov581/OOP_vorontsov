namespace OOP.Domain;

public readonly struct RoutePoint
{
    public double Latitude { get; }
    public double Longitude { get; }

    public RoutePoint(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public static double operator -(RoutePoint a, RoutePoint b)
    {
        double lat = (a.Latitude - b.Latitude) * 111.0;
        double lon = (a.Longitude - b.Longitude) * 111.0;
        return Math.Sqrt(lat * lat + lon * lon);
    }

    public static explicit operator string(RoutePoint point)
        => $"{point.Latitude:0.000};{point.Longitude:0.000}";

    public override string ToString() => (string)this;
}
