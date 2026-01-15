using Lama.Domain.Enums;

namespace Lama.Application.Services;

/// <summary>
/// Interfaz del servicio de confirmación de asistencia
/// Orquesta la lógica de:
/// 1. Subir evidencia fotográfica
/// 2. Actualizar datos de odómetro del vehículo
/// 3. Calcular puntos
/// 4. Confirmar asistencia del miembro
/// </summary>
public interface IAttendanceConfirmationService
{
    /// <summary>
    /// Confirma la asistencia de un miembro a un evento
    /// con subida de evidencia fotográfica
    /// </summary>
    /// <param name="eventId">ID del evento</param>
    /// <param name="request">Solicitud con archivos y datos</param>
    /// <param name="validatedByMemberId">ID del MTO/Admin que confirma</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Resultado con desglose de puntos y URLs de evidencia</returns>
    Task<AttendanceConfirmationResult> ConfirmAttendanceAsync(
        int eventId,
        UploadEvidenceRequest request,
        int validatedByMemberId,
        CancellationToken cancellationToken = default);
}
