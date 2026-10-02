namespace OOP.Domain;

public class LogisticsException : Exception
{
    public LogisticsException(string message) : base(message) { }
    public LogisticsException(string message, Exception inner) : base(message, inner) { }
}
