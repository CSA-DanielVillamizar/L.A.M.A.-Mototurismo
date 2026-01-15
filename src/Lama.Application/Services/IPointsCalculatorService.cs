using Lama.Domain.Entities;
using Lama.Domain.Enums;
using Lama.Application.Services;

namespace Lama.Application.Services;

/// <summary>
/// Servicio para calcular puntos según reglas de LAMA
/// </summary>
public interface IPointsCalculatorService
{
    /// <summary>
    /// Calcula los puntos otorgados a un miembro en un evento
    /// </summary>
    /// <param name="eventMileageInMiles">Distancia del evento en millas (ida)</param>
    /// <param name="eventClass">Clase del evento (1-5)</param>
    /// <param name="memberCountry">País del miembro</param>
    /// <param name="memberContinent">Continente del miembro</param>
    /// <param name="eventStartCountry">País de localidad de inicio del evento</param>
    /// <param name="eventStartContinent">Continente del evento (inicio)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Objeto con desglose de puntos calculados</returns>
    Task<PointsCalculationResult> CalculateAsync(
        double eventMileageInMiles,
        int eventClass,
        string? memberCountry,
        string? memberContinent,
        string? eventStartCountry,
        string? eventStartContinent,
        CancellationToken cancellationToken = default);

    /// <summary>Convierte millas a kilómetros</summary>
    double ConvertMilesToKilometers(double miles);

    /// <summary>Convierte kilómetros a millas</summary>
    double ConvertKilometersToMiles(double kilometers);
}

/// <summary>
/// Resultado del cálculo de puntos
/// </summary>
public class PointsCalculationResult
{
    /// <summary>Puntos por evento (basado en clase)</summary>
    public int PointsPerEvent { get; set; }

    /// <summary>Puntos por distancia recorrida</summary>
    public int PointsPerDistance { get; set; }

    /// <summary>Bonus por visitante</summary>
    public int VisitorBonus { get; set; }

    /// <summary>Total de puntos otorgados</summary>
    public int TotalPoints { get; set; }

    /// <summary>Clasificación de visitante</summary>
    public VisitorClass VisitorClassification { get; set; }

    /// <summary>Detalles de cálculo para auditoría</summary>
    public string CalculationDetails { get; set; } = string.Empty;
}
