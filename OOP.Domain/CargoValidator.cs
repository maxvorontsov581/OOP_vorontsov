namespace OOP.Domain;

public sealed class CargoValidator : IValidator<Cargo>
{
    public ValidationResult Validate(Cargo item)
    {
        if (item.WeightKg <= 0 || item.VolumeM3 <= 0)
            return ValidationResult.Error("Некорректный вес или объём.");

        if (item is PerishableCargo p && p.ExpirationDate <= DateTime.Now)
            return ValidationResult.Error("Срок годности истёк.");

        return ValidationResult.Ok();
    }
}
