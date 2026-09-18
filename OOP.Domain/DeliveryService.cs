namespace OOP.Domain;

public sealed class DeliveryService
{
    private readonly CargoCompatibilityValidator _validator;
    private ITariffStrategy _tariff;

    public Repository<Vehicle> Vehicles { get; } = new();
    public Repository<Customer> Customers { get; } = new();
    public Repository<Order> Orders { get; } = new();
    public decimal Revenue { get; private set; }
    public string TariffName => _tariff.Name;

    public event LogisticsEvent<OrderCreatedEventArgs>? OrderCreated;
    public event LogisticsEvent<OrderStatusChangedEventArgs>? OrderStatusChanged;
    public event LogisticsEvent<VehicleOverloadAttemptEventArgs>? VehicleOverloadAttempt;
    public event LogisticsEvent<DeliveryCompletedEventArgs>? DeliveryCompleted;

    public DeliveryService(ITariffStrategy? tariff = null)
    {
        _validator = new CargoCompatibilityValidator();
        _tariff = tariff ?? new StandardTariff();
    }

    public void SetTariff(ITariffStrategy tariff)
    {
        _tariff = tariff ?? throw new ArgumentNullException(nameof(tariff));
    }

    public void AddVehicle(Vehicle vehicle) => Vehicles.Add(vehicle);
    public void AddCustomer(Customer customer) => Customers.Add(customer);

    public Order CreateOrder(Customer customer, IEnumerable<Cargo> cargo, Route route)
    {
        List<Cargo> list = cargo.ToList();
        _validator.ValidateCargoSet(list);

        Customers.Add(customer);
        Order order = new(customer, list, route);
        Orders.Add(order);
        OrderCreated?.Invoke(this, new OrderCreatedEventArgs(order));
        return order;
    }

    public IEnumerable<Vehicle> FindVehicles(Order order)
    {
        List<Vehicle> result = new();
        double weight = order.Cargo.Sum(x => x.WeightKg);
        double volume = order.Cargo.Sum(x => x.VolumeM3);
        bool hasCold = order.Cargo.Any(x => x is PerishableCargo);

        foreach (Vehicle v in Vehicles)
        {
            if (v.State != VehicleState.Free)
                continue;

            if (weight > v.MaxLoadKg || volume > v.MaxVolumeM3)
                continue;

            bool ok = true;
            foreach (Cargo c in order.Cargo)
            {
                if (!v.CanCarry(c))
                {
                    ok = false;
                    break;
                }
            }

            if (!ok)
                continue;

            if (v is DroneCourier drone && order.Route.DistanceKm > drone.MaxDistanceKm)
                continue;

            if (hasCold && v is not RefrigeratorTruck)
                continue;

            result.Add(v);
        }

        return result;
    }

    public Vehicle AssignBest(Order order)
    {
        if (order.Status != OrderStatus.Created)
            throw new InvalidOrderStateException("Транспорт можно подбирать только для нового заказа.");

        List<Vehicle> candidates = FindVehicles(order).ToList();
        if (candidates.Count == 0)
            throw new IncompatibleCargoException("Не найден подходящий свободный транспорт.");

        Vehicle best = candidates
            .OrderBy(v => CalculateCost(v, order))
            .First();

        decimal cost = CalculateCost(best, order);
        OrderStatus old = order.Status;
        order.Assign(best, cost);
        OrderStatusChanged?.Invoke(this, new OrderStatusChangedEventArgs(order, old, order.Status));
        return best;
    }

    public void AssignVehicle(Order order, Vehicle vehicle)
    {
        if (vehicle.State != VehicleState.Free)
            throw new IncompatibleCargoException("Транспорт сейчас занят или находится на обслуживании.");

        double totalWeight = order.Cargo.Sum(x => x.WeightKg);
        double totalVolume = order.Cargo.Sum(x => x.VolumeM3);

        if (totalWeight > vehicle.MaxLoadKg || totalVolume > vehicle.MaxVolumeM3)
        {
            VehicleOverloadAttempt?.Invoke(this,
                new VehicleOverloadAttemptEventArgs(vehicle, order, totalWeight));
            throw new VehicleOverloadException("Попытка перегрузить транспорт.");
        }

        if (vehicle is DroneCourier drone && order.Route.DistanceKm > drone.MaxDistanceKm)
            throw new IncompatibleCargoException("Маршрут слишком длинный для дрона.");

        _validator.ValidateForVehicle(order.Cargo, vehicle);
        decimal cost = CalculateCost(vehicle, order);
        OrderStatus old = order.Status;
        order.Assign(vehicle, cost);
        OrderStatusChanged?.Invoke(this, new OrderStatusChangedEventArgs(order, old, order.Status));
    }

    public IDeliveryCost AddServices(Order order, ExtraServices services)
    {
        if (order.Status != OrderStatus.Assigned)
            throw new InvalidOrderStateException("Сначала заказу нужно назначить транспорт.");

        IDeliveryCost result = new BaseDeliveryCost(order.TotalCost, "Тариф и транспорт");

        if ((services & ExtraServices.Insurance) != 0)
            result = new InsuranceDecorator(result, order.Cargo);

        if ((services & ExtraServices.Urgent) != 0)
            result = new UrgencyDecorator(result);

        if ((services & ExtraServices.FragilePacking) != 0)
            result = new FragilePackingDecorator(result, order.Cargo);

        order.ApplyFinalCost(result.Total, services);
        return result;
    }

    public void StartDelivery(Order order)
    {
        if (order.AssignedVehicle is null)
            throw new InvalidOrderStateException("У заказа нет транспорта.");

        OrderStatus old = order.Status;
        order.StartDelivery();
        order.AssignedVehicle.SetState(VehicleState.InTransit);
        OrderStatusChanged?.Invoke(this, new OrderStatusChangedEventArgs(order, old, order.Status));
    }

    public void CompleteDelivery(Order order)
    {
        if (order.AssignedVehicle is null)
            throw new InvalidOrderStateException("У заказа нет транспорта.");

        OrderStatus old = order.Status;
        order.Complete();
        order.AssignedVehicle.SetState(VehicleState.Free);
        Revenue += order.TotalCost;
        OrderStatusChanged?.Invoke(this, new OrderStatusChangedEventArgs(order, old, order.Status));
        DeliveryCompleted?.Invoke(this, new DeliveryCompletedEventArgs(order, order.TotalCost));
    }

    public void CancelOrder(Order order)
    {
        OrderStatus old = order.Status;
        order.Cancel();
        if (order.AssignedVehicle is not null)
            order.AssignedVehicle.SetState(VehicleState.Free);
        OrderStatusChanged?.Invoke(this, new OrderStatusChangedEventArgs(order, old, order.Status));
    }

    public void StartMaintenance(Vehicle vehicle)
    {
        if (vehicle.State == VehicleState.InTransit)
            throw new LogisticsException("Нельзя отправить в обслуживание транспорт, который сейчас в пути.");

        vehicle.SetState(VehicleState.UnderMaintenance);
    }

    public void EndMaintenance(Vehicle vehicle)
    {
        if (vehicle.State == VehicleState.InTransit)
            throw new LogisticsException("Некорректное состояние транспорта.");

        vehicle.SetState(VehicleState.Free);
    }

    public decimal CalculateCost(Vehicle vehicle, Order order)
    {
        decimal baseCost = vehicle.CalculateDeliveryCost(order.Route, order.Cargo);
        if (baseCost == decimal.MaxValue)
            return baseCost;
        return _tariff.Calculate(baseCost, order.Route, order.Cargo);
    }

    internal void RestoreRevenue(decimal revenue) => Revenue = revenue;
}
