namespace Lama.Application.Services;

/// <summary>
/// Interfaz para gestión de almacenamiento de archivos (fotos de evidencia)
/// </summary>
public interface IBlobStorageService
{
    /// <summary>
    /// Sube un archivo al almacenamiento y devuelve su URL
    /// </summary>
    /// <param name="fileStream">Stream del archivo</param>
    /// <param name="fileName">Nombre del archivo</param>
    /// <param name="contentType">Tipo MIME (ej. image/jpeg)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>URL del archivo almacenado</returns>
    Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina un archivo del almacenamiento
    /// </summary>
    Task<bool> DeleteAsync(string fileUrl, CancellationToken cancellationToken = default);
}
