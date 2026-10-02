namespace OOP.Domain;

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
