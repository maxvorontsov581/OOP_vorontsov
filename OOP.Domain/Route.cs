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

public sealed class Route
{
    private readonly List<RoutePoint> _points = new();

    public IReadOnlyCollection<RoutePoint> Points => _points.AsReadOnly();

    public double DistanceKm
    {
        get
        {
            if (_points.Count < 2)
                return 0;

            double result = 0;
            for (int i = 1; i < _points.Count; i++)
                result += _points[i] - _points[i - 1];

            return result;
        }
    }

    public Route(IEnumerable<RoutePoint> points)
    {
        _points.AddRange(points);
        if (_points.Count < 2)
            throw new RouteNotFoundException("Маршрут должен содержать хотя бы две точки.");
    }

    public TimeSpan EstimateTime(Vehicle v)
    {
        if (v.AverageSpeedKmH <= 0)
            return TimeSpan.Zero;

        return TimeSpan.FromHours(DistanceKm / v.AverageSpeedKmH);
    }
}
