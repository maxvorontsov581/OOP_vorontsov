namespace OOP.Domain;

public interface IEntity
{
    Guid Id { get; }
}

public interface IReadOnlyRepository<out T> where T : class, IEntity
{
    T? GetById(Guid id);
    IEnumerable<T> GetAll();
}

public interface IValidator<in T>
{
    ValidationResult Validate(T item);
}

public sealed class ValidationResult
{
    public bool IsValid { get; }
    public string Message { get; }

    public ValidationResult(bool isValid, string message = "")
    {
        IsValid = isValid;
        Message = message;
    }

    public static ValidationResult Ok() => new(true);
    public static ValidationResult Error(string message) => new(false, message);
}

public interface ITemperatureSensitive
{
    double RequiredTemperatureC { get; }
}

public interface IInsurable
{
    decimal InsuranceValue { get; }
}

public interface IStackable
{
    bool CanStack { get; }
}

public interface ITariffStrategy
{
    string Name { get; }
    decimal Calculate(decimal baseCost, Route route, IReadOnlyCollection<Cargo> cargo);
}

public interface IDeliveryCost
{
    decimal Total { get; }
    string Describe();
}

public enum VehicleState
{
    Free,
    InTransit,
    UnderMaintenance
}

public enum OrderStatus
{
    Created,
    Assigned,
    InTransit,
    Delivered,
    Cancelled
}

[Flags]
public enum TransportConditions
{
    None = 0,
    Refrigerated = 1,
    Sealed = 2,
    Pressurized = 4,
    LongRange = 8
}

[Flags]
public enum ExtraServices
{
    None = 0,
    Insurance = 1,
    Urgent = 2,
    FragilePacking = 4
}
