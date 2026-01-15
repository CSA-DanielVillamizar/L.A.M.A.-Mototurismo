namespace Lama.Domain.Enums;

/// <summary>
/// Estados posibles de una asistencia a evento
/// </summary>
public enum AttendanceStatus
{
    /// <summary>Asistencia pendiente de confirmación</summary>
    Pending,
    
    /// <summary>Asistencia confirmada</summary>
    Confirmed,
    
    /// <summary>Asistencia rechazada</summary>
    Rejected
}
