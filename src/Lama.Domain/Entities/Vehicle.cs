using Lama.Domain.Enums;

namespace Lama.Domain.Entities;

/// <summary>
/// Vehículo (moto o triciclo) de un miembro
/// </summary>
public class Vehicle
{
    /// <summary>Identificador único</summary>
    public int Id { get; set; }

    /// <summary>ID del tenant (multi-tenancy). Default: LAMA_DEFAULT (00000000-0000-0000-0000-000000000001)</summary>
    public Guid TenantId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

    /// <summary>ID del miembro propietario</summary>
    public int MemberId { get; set; }

    /// <summary>Descripción del vehículo (marca, modelo, año, etc.)</summary>
    public required string MotorcycleData { get; set; }

    /// <summary>Placa de licencia única</summary>
    public required string LicPlate { get; set; }

    /// <summary>Indica si es un triciclo</summary>
    public bool Trike { get; set; }

    /// <summary>Estado de validación de evidencia fotográfica</summary>
    public string Photography { get; set; } = "PENDING";

    /// <summary>URL de evidencia de inicio de año (collage: piloto+moto + odómetro)</summary>
    public string? StartYearEvidenceUrl { get; set; }

    /// <summary>URL de evidencia de corte (collage: piloto+moto + odómetro)</summary>
    public string? CutOffEvidenceUrl { get; set; }

    /// <summary>Fecha y hora de validación de evidencia de inicio</summary>
    public DateTime? StartYearEvidenceValidatedAt { get; set; }

    /// <summary>Fecha y hora de validación de evidencia de corte</summary>
    public DateTime? CutOffEvidenceValidatedAt { get; set; }

    /// <summary>ID del MTO/Admin que validó la evidencia</summary>
    public int? EvidenceValidatedBy { get; set; }

    /// <summary>Unidad de medida del odómetro (Miles, Kilometers)</summary>
    public string OdometerUnit { get; set; } = "Miles";

    /// <summary>Lectura de odómetro al inicio</summary>
    public double? StartingOdometer { get; set; }

    /// <summary>Lectura de odómetro al final</summary>
    public double? FinalOdometer { get; set; }

    /// <summary>Fecha de lectura inicial</summary>
    public DateOnly? StartingOdometerDate { get; set; }

    /// <summary>Fecha de lectura final</summary>
    public DateOnly? FinalOdometerDate { get; set; }

    /// <summary>Indica si la moto está activa en el campeonato actual</summary>
    public bool IsActiveForChampionship { get; set; }

    /// <summary>Fecha de creación del registro</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Última fecha de actualización</summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>Relación: Miembro propietario</summary>
    public Member? Member { get; set; }

    /// <summary>Relación: Validador de evidencia</summary>
    public Member? ValidatedByMember { get; set; }

    /// <summary>Relación: Asistencias con este vehículo</summary>
    public ICollection<Attendance> Attendances { get; set; } = [];
}
