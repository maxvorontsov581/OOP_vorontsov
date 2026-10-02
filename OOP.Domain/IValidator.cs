namespace OOP.Domain;

public interface IValidator<in T>
{
    ValidationResult Validate(T item);
}
