# OOP — логистическая система

**Воронцов Максим**  
**ИСУ:** 465450  
**Telegram:** [@dontcare2much](https://t.me/dontcare2much)

Учебный проект по объектно-ориентированному программированию на C#.

## Запуск

Нужен **.NET 9 SDK**.

Из корня проекта:

```bash
dotnet run --project OOP.App/OOP.App.csproj
```

При запуске сначала выполняется демонстрационный сценарий, после чего открывается консольное меню.

Демо создаёт примеры транспорта, клиентов, грузов и заказов, показывает работу событий, тарифов, дополнительных услуг, LINQ-отчётов и сохранения/загрузки JSON.

Сборка проекта:

```bash
dotnet build OOP.sln
```

Запуск тестов:

```bash
dotnet test OOP.sln
```

## Структура проекта

- `OOP.Domain` — основные модели, транспорт, грузы, заказы, репозиторий, валидация, расчёт стоимости, паттерны, события, исключения, отчёты и JSON.
- `OOP.App` — запуск демонстрационного сценария и консольное меню.
- `OOP.Tests` — тесты моделей, сервиса, репозитория, валидации, транспорта и JSON.

## Соответствие заданию

| Пункт | Где находится реализация |
| --- | --- |
| T1 — инкапсуляция | `OOP.Domain/Cargo.cs`, `OOP.Domain/Vehicle.cs`, `OOP.Domain/Order.cs` |
| T2 — наследование и полиморфизм | `OOP.Domain/Vehicle.cs`, `OOP.Domain/Truck.cs`, `OOP.Domain/RefrigeratorTruck.cs`, `OOP.Domain/CargoPlane.cs`, `OOP.Domain/CargoShip.cs`, `OOP.Domain/DroneCourier.cs`, а также наследники `Cargo` |
| T3 — интерфейсы и вариантность | `OOP.Domain/IReadOnlyRepository.cs`, `OOP.Domain/IValidator.cs`, `OOP.Domain/IInsurable.cs`, `OOP.Domain/ITemperatureSensitive.cs`; демонстрация — `OOP.App/Demo.cs` |
| T4 — обобщения и коллекции | `OOP.Domain/Repository.cs`, `OOP.Domain/ReportExtensions.cs` |
| T5 — делегаты и события | `OOP.Domain/LogisticsEvent.cs`, `OOP.Domain/DeliveryService.cs`, `OOP.App/ConsoleNotifier.cs`, `OOP.App/FileLogger.cs` |
| T6 — исключения | `OOP.Domain/LogisticsException.cs` и остальные пользовательские исключения; примеры `try/catch`, `when`, `throw;`, `finally` и `using` — в демонстрационном коде |
| T7 — пять паттернов | См. таблицу ниже |
| T8 — отчёты LINQ | `OOP.Domain/Reports.cs` |
| T9 — сохранение JSON | `OOP.Domain/JsonStorage.cs` |
| T10 — enum, структура и оператор | `OOP.Domain/TransportConditions.cs` содержит `[Flags]`, `OOP.Domain/RoutePoint.cs` — `readonly struct` и перегрузку оператора `-` |

## Паттерны

| Паттерн | Где применён | Зачем |
| --- | --- | --- |
| Strategy | `OOP.Domain/ITariffStrategy.cs`, `OOP.Domain/StandardTariff.cs`, `OOP.Domain/ExpressTariff.cs`, `OOP.Domain/HeavyCargoTariff.cs` | Позволяет менять алгоритм расчёта тарифа без изменения `DeliveryService` |
| Decorator | `OOP.Domain/DeliveryCostDecorator.cs`, `OOP.Domain/InsuranceDecorator.cs`, `OOP.Domain/UrgencyDecorator.cs`, `OOP.Domain/FragilePackingDecorator.cs` | Позволяет комбинировать дополнительные услуги поверх базовой стоимости |
| Factory Method | `OOP.Domain/VehicleCreator.cs` и классы конкретных creators | Создаёт разные виды транспорта через общий метод создания |
| Observer | события `DeliveryService`, подписчики `ConsoleNotifier` и `FileLogger` | Позволяет независимо реагировать на создание заказа, изменение статуса и завершение доставки |
| Singleton | `OOP.Domain/TariffConfig.cs` | Хранит единственный экземпляр конфигурации тарифов через `Lazy<TariffConfig>` |

## Основные возможности

Проект демонстрирует:

- наследование и полиморфизм для транспорта и грузов;
- `abstract`, `virtual`, `override`, `base` и `sealed`;
- интерфейсы и явную реализацию интерфейса;
- generics и `Repository<T>`;
- ковариантность `out` и контравариантность `in`;
- собственный делегат и события;
- пользовательские исключения;
- `yield return`;
- LINQ-запросы и отчёты;
- JSON-сохранение и восстановление состояния;
- паттерны Strategy, Decorator, Factory Method, Observer и Singleton;
- `Lazy<T>`;
- `[Flags] enum`;
- `readonly struct` и перегрузку оператора;
- модульные тесты на xUnit.
