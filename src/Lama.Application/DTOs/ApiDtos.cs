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

/// <summary>
/// DTO genérico para resultados paginados
/// </summary>
public class PagedResultDto<T>
{
    /// <summary>Items de la página actual</summary>
    public IReadOnlyList<T> Items { get; set; } = new List<T>();

    /// <summary>Número de página actual (1-indexed)</summary>
    public int Page { get; set; }

    /// <summary>Cantidad de items por página</summary>
    public int PageSize { get; set; }

    /// <summary>Total de items en todas las páginas</summary>
    public int Total { get; set; }

    /// <summary>Total de páginas disponibles</summary>
    public int TotalPages => (Total + PageSize - 1) / PageSize;
}

/// <summary>
/// DTO para admin queue - lista de asistencias pendientes de validación
/// </summary>
public class AdminQueueItemDto
{
    /// <summary>ID del registro de asistencia</summary>
    public int AttendanceId { get; set; }

    /// <summary>ID del tenant</summary>
    public Guid TenantId { get; set; }

    /// <summary>ID del evento</summary>
    public int EventId { get; set; }

    /// <summary>Nombre del evento</summary>
    public string EventName { get; set; } = string.Empty;

    /// <summary>Fecha del evento</summary>
    public DateOnly EventDate { get; set; }

    /// <summary>ID del miembro</summary>
    public int MemberId { get; set; }

    /// <summary>Nombre completo del miembro</summary>
    public string MemberName { get; set; } = string.Empty;

    /// <summary>Email del miembro</summary>
    public string MemberEmail { get; set; } = string.Empty;

    /// <summary>ID del capítulo del miembro</summary>
    public int ChapterId { get; set; }

    /// <summary>ID del vehículo</summary>
    public int VehicleId { get; set; }

    /// <summary>Placa del vehículo</summary>
    public string VehicleLicPlate { get; set; } = string.Empty;

    /// <summary>Datos de la motocicleta (Make Model Year Color)</summary>
    public string VehicleMotorcycleData { get; set; } = string.Empty;

    /// <summary>Estado de asistencia (PENDING, CONFIRMED, REJECTED)</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>Puntos otorgados por evento</summary>
    public int? PointsPerEvent { get; set; }

    /// <summary>Puntos otorgados por distancia</summary>
    public int? PointsPerDistance { get; set; }

    /// <summary>Total de puntos otorgados al miembro</summary>
    public int? PointsAwardedPerMember { get; set; }

    /// <summary>Clase de visitante (si aplica)</summary>
    public string? VisitorClass { get; set; }

    /// <summary>Fecha de confirmación</summary>
    public DateTime? ConfirmedAt { get; set; }

    /// <summary>ID del usuario que confirmó</summary>
    public int? ConfirmedBy { get; set; }

    /// <summary>Fecha de creación del registro</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Fecha de última actualización</summary>
    public DateTime UpdatedAt { get; set; }
}
