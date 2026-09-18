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

public sealed class CargoCompatibilityValidator
{
    private readonly IValidator<Cargo> _cargoValidator;

    public CargoCompatibilityValidator(IValidator<Cargo>? cargoValidator = null)
    {
        _cargoValidator = cargoValidator ?? new CargoValidator();
    }

    public void ValidateCargoSet(IReadOnlyCollection<Cargo> cargo)
    {
        if (cargo.Count == 0)
            throw new CargoValidationException("Список грузов пуст.");

        foreach (Cargo item in cargo)
        {
            ValidationResult result = _cargoValidator.Validate(item);
            if (!result.IsValid)
                throw new CargoValidationException(result.Message);
        }

        bool hasDangerous = cargo.Any(x => x is DangerousCargo);
        bool hasPerishable = cargo.Any(x => x is PerishableCargo);

        if (hasDangerous && hasPerishable)
            throw new IncompatibleCargoException("Опасный и скоропортящийся груз нельзя везти вместе.");
    }

    public void ValidateForVehicle(IReadOnlyCollection<Cargo> cargo, Vehicle vehicle)
    {
        ValidateCargoSet(cargo);

        double totalWeight = cargo.Sum(x => x.WeightKg);
        double totalVolume = cargo.Sum(x => x.VolumeM3);

        if (totalWeight > vehicle.MaxLoadKg || totalVolume > vehicle.MaxVolumeM3)
            throw new VehicleOverloadException("Превышена грузоподъёмность или объём транспорта.");

        foreach (Cargo item in cargo)
        {
            if (!vehicle.CanCarry(item))
                throw new IncompatibleCargoException($"{vehicle.GetType().Name} не подходит для груза {item.Description}.");

            if (item is PerishableCargo && vehicle is not RefrigeratorTruck)
                throw new IncompatibleCargoException("Скоропортящийся груз требует рефрижератор.");
        }
    }
}
