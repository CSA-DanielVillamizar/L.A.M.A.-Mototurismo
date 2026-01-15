namespace Lama.Domain.Enums;

/// <summary>
/// Tipos de acciones auditables en el sistema.
/// </summary>
public enum AuditActionType
{
    /// <summary>Creación de entidad</summary>
    Create = 1,

    /// <summary>Actualización de entidad</summary>
    Update = 2,

    /// <summary>Eliminación de entidad (soft delete o eliminación lógica)</summary>
    Delete = 3,

    /// <summary>Aprobación de evidencia</summary>
    EvidenceApproved = 4,

    /// <summary>Rechazo de evidencia</summary>
    EvidenceRejected = 5,

    /// <summary>Confirmación de asistencia</summary>
    AttendanceConfirmed = 6,

    /// <summary>Actualización de odómetro de vehículo</summary>
    VehicleOdometerUpdated = 7,

    /// <summary>Cambio de miembro a rol (role assignment)</summary>
    MemberRoleAssigned = 8,

    /// <summary>Remoción de miembro de rol</summary>
    MemberRoleRemoved = 9,

    /// <summary>Cambio de scope de miembro</summary>
    MemberScopeChanged = 10,

    /// <summary>Cambio de configuración</summary>
    ConfigurationChanged = 11,

    /// <summary>Inicio de sesión</summary>
    Login = 12,

    /// <summary>Logout</summary>
    Logout = 13,

    /// <summary>Acceso denegado (unauthorized)</summary>
    UnauthorizedAccess = 14,

    /// <summary>Operación administrativa genérica</summary>
    AdminOperation = 15
}
