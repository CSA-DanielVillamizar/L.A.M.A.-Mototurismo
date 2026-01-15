using Lama.Application.Services;
using Lama.Domain.Enums;

namespace Lama.Infrastructure.Services;

/// <summary>
/// Implementación de IPointsCalculatorService
/// Calcula puntos según las reglas de negocio de LAMA
/// </summary>
public class PointsCalculatorService(IAppConfigProvider configProvider) : IPointsCalculatorService
{
    private readonly IAppConfigProvider _configProvider = configProvider;
    private const double KM_TO_MILES_FACTOR = 0.621371;

    public async Task<PointsCalculationResult> CalculateAsync(
        double eventMileageInMiles,
        int eventClass,
        string? memberCountry,
        string? memberContinent,
        string? eventStartCountry,
        string? eventStartContinent,
        CancellationToken cancellationToken = default)
    {
        var result = new PointsCalculationResult();

        // 1. Puntos por evento (basado en clase)
        result.PointsPerEvent = await GetPointsPerClassAsync(eventClass, cancellationToken);

        // 2. Puntos por distancia
        result.PointsPerDistance = await GetPointsPerDistanceAsync(eventMileageInMiles, cancellationToken);

        // 3. Bonus visitante
        (result.VisitorBonus, result.VisitorClassification) = await CalculateVisitorBonusAsync(
            memberCountry, memberContinent, eventStartCountry, eventStartContinent, cancellationToken);

        // 4. Total
        result.TotalPoints = result.PointsPerEvent + result.PointsPerDistance + result.VisitorBonus;

        // 5. Detalles
        result.CalculationDetails = $"Class:{eventClass} miles:{eventMileageInMiles:F2}mi " +
            $"PointsPerEvent:{result.PointsPerEvent} PointsPerDistance:{result.PointsPerDistance} " +
            $"VisitorBonus({result.VisitorClassification}):{result.VisitorBonus} Total:{result.TotalPoints}";

        return result;
    }

    /// <summary>
    /// Obtiene los puntos base según la clase del evento
    /// Configuración: PointsPerClassMultiplier_1 a _5
    /// </summary>
    private async Task<int> GetPointsPerClassAsync(int eventClass, CancellationToken cancellationToken = default)
    {
        string configKey = eventClass switch
        {
            1 => "PointsPerClassMultiplier_1",
            2 => "PointsPerClassMultiplier_2",
            3 => "PointsPerClassMultiplier_3",
            4 => "PointsPerClassMultiplier_4",
            5 => "PointsPerClassMultiplier_5",
            _ => "PointsPerClassMultiplier_1"
        };

        int defaultValue = eventClass switch
        {
            1 => 1,
            2 => 3,
            3 => 5,
            4 => 10,
            5 => 15,
            _ => 1
        };

        return await _configProvider.GetIntAsync(configKey, defaultValue, cancellationToken);
    }

    /// <summary>
    /// Calcula puntos por distancia recorrida
    /// Umbrales: >200 mi = 1 punto, >800 mi = 2 puntos
    /// </summary>
    private async Task<int> GetPointsPerDistanceAsync(double mileageInMiles, CancellationToken cancellationToken = default)
    {
        var threshold2Points = await _configProvider.GetIntAsync("DistanceThreshold_2Points_OneWayMiles", 800, cancellationToken);
        var threshold1Point = await _configProvider.GetIntAsync("DistanceThreshold_1Point_OneWayMiles", 200, cancellationToken);

        if (mileageInMiles > threshold2Points)
            return 2;

        if (mileageInMiles > threshold1Point)
            return 1;

        return 0;
    }

    /// <summary>
    /// Calcula bonus visitante y clasificación
    /// - LOCAL: mismo país => +0
    /// - VISITOR_A: otro país, mismo continente => +1
    /// - VISITOR_B: otro continente => +2
    /// - Fallback si no hay continente: país diferente => +1
    /// </summary>
    private async Task<(int bonus, VisitorClass classification)> CalculateVisitorBonusAsync(
        string? memberCountry,
        string? memberContinent,
        string? eventCountry,
        string? eventContinent,
        CancellationToken cancellationToken = default)
    {
        // Si no tenemos información de país, asumir local
        if (string.IsNullOrEmpty(memberCountry) || string.IsNullOrEmpty(eventCountry))
            return (0, VisitorClass.Local);

        // Si es el mismo país => LOCAL
        if (memberCountry.Equals(eventCountry, StringComparison.OrdinalIgnoreCase))
            return (0, VisitorClass.Local);

        // Países diferentes
        var bonusB = await _configProvider.GetIntAsync("VisitorBonus_DifferentContinent", 2, cancellationToken);
        var bonusA = await _configProvider.GetIntAsync("VisitorBonus_SameContinent", 1, cancellationToken);

        // Si tenemos continentes, usar esa lógica
        if (!string.IsNullOrEmpty(memberContinent) && !string.IsNullOrEmpty(eventContinent))
        {
            if (memberContinent.Equals(eventContinent, StringComparison.OrdinalIgnoreCase))
                return (bonusA, VisitorClass.VisitorA);
            else
                return (bonusB, VisitorClass.VisitorB);
        }

        // Fallback: sin continente, solo considerar país diferente
        return (bonusA, VisitorClass.VisitorA);
    }

    public double ConvertMilesToKilometers(double miles) => miles / KM_TO_MILES_FACTOR;
    public double ConvertKilometersToMiles(double kilometers) => kilometers * KM_TO_MILES_FACTOR;
}
