namespace Lama.Application.Services;

/// <summary>
/// Resultado de la generación de SAS URLs para upload de evidencias
/// </summary>
public class BlobSasUploadResult
{
    /// <summary>SAS URL para subir foto del piloto</summary>
    public string PilotPhotoSasUrl { get; set; } = string.Empty;

    /// <summary>SAS URL para subir foto del odómetro</summary>
    public string OdometerPhotoSasUrl { get; set; } = string.Empty;

    /// <summary>Path del blob de foto del piloto (para guardar en BD)</summary>
    public string PilotPhotoBlobPath { get; set; } = string.Empty;

    /// <summary>Path del blob de foto del odómetro (para guardar en BD)</summary>
    public string OdometerPhotoBlobPath { get; set; } = string.Empty;

    /// <summary>Fecha de expiración de las SAS URLs</summary>
    public DateTime ExpiresAt { get; set; }
}

/// <summary>
/// Servicio para generar SAS URLs de Azure Blob Storage
/// que permiten upload directo desde cliente sin pasar por backend
/// </summary>
public interface IBlobSasService
{
    /// <summary>
    /// Genera 2 SAS URLs para upload de evidencias (foto piloto + odómetro).
    /// Las SAS URLs tienen permisos mínimos (Write/Create) y TTL de 10 minutos.
    /// </summary>
    /// <param name="tenantId">ID del tenant</param>
    /// <param name="eventId">ID del evento (puede ser null para START_YEAR)</param>
    /// <param name="memberId">ID del miembro</param>
    /// <param name="vehicleId">ID del vehículo</param>
    /// <param name="evidenceType">Tipo de evidencia (START_YEAR o CUTOFF)</param>
    /// <param name="correlationId">ID de correlación único</param>
    /// <param name="pilotPhotoContentType">Content-Type de la foto del piloto (ej: "image/jpeg")</param>
    /// <param name="odometerPhotoContentType">Content-Type de la foto del odómetro</param>
    /// <returns>Resultado con SAS URLs y paths de los blobs</returns>
    Task<BlobSasUploadResult> GenerateEvidenceUploadSasAsync(
        Guid tenantId,
        int? eventId,
        int memberId,
        int vehicleId,
        string evidenceType,
        string correlationId,
        string pilotPhotoContentType,
        string odometerPhotoContentType);

    /// <summary>
    /// Verifica que un blob existe en Azure Storage.
    /// Útil para validar que el cliente realmente subió las fotos antes de crear el Evidence.
    /// </summary>
    /// <param name="blobPath">Path del blob a verificar</param>
    /// <returns>True si el blob existe, false en caso contrario</returns>
    Task<bool> BlobExistsAsync(string blobPath);

    /// <summary>
    /// Elimina un blob de Azure Storage.
    /// Útil para limpiar blobs si el proceso falla.
    /// </summary>
    /// <param name="blobPath">Path del blob a eliminar</param>
    /// <returns>True si se eliminó exitosamente</returns>
    Task<bool> DeleteBlobAsync(string blobPath);

    /// <summary>
    /// Genera una SAS URL con permisos de lectura para un blob existente.
    /// Útil para mostrar las fotos en el frontend.
    /// </summary>
    /// <param name="blobPath">Path del blob</param>
    /// <param name="expiresInMinutes">Minutos de validez del SAS (default 60)</param>
    /// <returns>SAS URL con permisos de lectura</returns>
    Task<string> GenerateReadSasUrlAsync(string blobPath, int expiresInMinutes = 60);
}
