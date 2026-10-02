namespace OOP.Domain;

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
