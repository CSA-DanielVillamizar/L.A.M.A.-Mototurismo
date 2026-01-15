namespace Lama.Application.DTOs;

/// <summary>
/// DTO para representar un evento en las respuestas de la API
/// </summary>
public class EventDto
{
    /// <summary>ID del evento</summary>
    public int EventId { get; set; }

    /// <summary>Nombre del evento</summary>
    public string EventName { get; set; } = string.Empty;

    /// <summary>Fecha del evento</summary>
    public DateOnly EventDate { get; set; }

    /// <summary>ID del capítulo organizador</summary>
    public int ChapterId { get; set; }

    /// <summary>Tipo de evento</summary>
    public string EventType { get; set; } = string.Empty;
}

/// <summary>
/// DTO para búsqueda de miembros (autocomplete)
/// </summary>
public class MemberSearchDto
{
    /// <summary>ID del miembro</summary>
    public int MemberId { get; set; }

    /// <summary>Nombre del miembro</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Apellido del miembro</summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>Nombre completo (FirstName + LastName)</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>Estado/Rol del miembro</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>ID del capítulo</summary>
    public int ChapterId { get; set; }

    /// <summary>Número de orden del miembro</summary>
    public int Order { get; set; }
}

/// <summary>
/// DTO para detalles completos de un miembro
/// </summary>
public class MemberDto
{
    /// <summary>ID del miembro</summary>
    public int MemberId { get; set; }

    /// <summary>Nombre del miembro</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Apellido del miembro</summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>Nombre completo</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>Estado/Rol del miembro</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>ID del capítulo</summary>
    public int ChapterId { get; set; }
}

/// <summary>
/// DTO para vehículos de un miembro
/// </summary>
public class VehicleDto
{
    /// <summary>ID del vehículo</summary>
    public int VehicleId { get; set; }

    /// <summary>ID del miembro propietario</summary>
    public int MemberId { get; set; }

    /// <summary>Placa del vehículo</summary>
    public string LicPlate { get; set; } = string.Empty;

    /// <summary>Datos de la motocicleta (Marca Model Año Color)</summary>
    public string MotorcycleData { get; set; } = string.Empty;

    /// <summary>Es un triciclo</summary>
    public bool Trike { get; set; }

    /// <summary>Nombre descriptivo para dropdown (MotorcycleData - LicPlate)</summary>
    public string DisplayName => $"{MotorcycleData} - {LicPlate}";
}

/// <summary>
/// DTO para asistentes a un evento (admin queue)
/// </summary>
public class AttendeeDto
{
    /// <summary>ID del registro de asistencia</summary>
    public int AttendanceId { get; set; }

    /// <summary>ID del miembro</summary>
    public int MemberId { get; set; }

    /// <summary>Nombre completo del miembro</summary>
    public string CompleteNames { get; set; } = string.Empty;

    /// <summary>Número de orden del miembro</summary>
    public int Order { get; set; }

    /// <summary>ID del vehículo</summary>
    public int VehicleId { get; set; }

    /// <summary>Placa del vehículo</summary>
    public string LicPlate { get; set; } = string.Empty;

    /// <summary>Datos de la motocicleta</summary>
    public string MotorcycleData { get; set; } = string.Empty;

    /// <summary>Estado de asistencia (PENDING, CONFIRMED, REJECTED)</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>Fecha de confirmación (si aplica)</summary>
    public DateTime? ConfirmedAt { get; set; }
}
