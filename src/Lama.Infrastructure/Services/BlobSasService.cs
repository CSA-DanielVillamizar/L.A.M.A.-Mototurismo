using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Lama.Application.Services;

namespace Lama.Infrastructure.Services;

/// <summary>
/// Implementación de servicio para generar SAS URLs de Azure Blob Storage
/// </summary>
public class BlobSasService : IBlobSasService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<BlobSasService> _logger;
    private readonly string _containerName;

    public BlobSasService(
        BlobServiceClient blobServiceClient,
        IConfiguration configuration,
        ILogger<BlobSasService> logger)
    {
        _blobServiceClient = blobServiceClient ?? throw new ArgumentNullException(nameof(blobServiceClient));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        // Obtener el nombre del container desde configuración (default: "evidences")
        _containerName = _configuration["AzureStorage:EvidenceContainerName"] ?? "evidences";
    }

    public async Task<BlobSasUploadResult> GenerateEvidenceUploadSasAsync(
        Guid tenantId,
        int? eventId,
        int memberId,
        int vehicleId,
        string evidenceType,
        string correlationId,
        string pilotPhotoContentType,
        string odometerPhotoContentType)
    {
        try
        {
            _logger.LogInformation("Generando SAS URLs para evidencia. CorrelationId: {CorrelationId}", correlationId);

            // Construir paths de blobs siguiendo estructura: TenantId/year/eventId/memberId/vehicleId/evidenceType/
            var year = DateTime.UtcNow.Year;
            var eventPart = eventId.HasValue ? eventId.Value.ToString() : "start-year";
            var basePath = $"{tenantId}/{year}/{eventPart}/{memberId}/{vehicleId}/{evidenceType.ToLowerInvariant()}";

            // Generar nombres de archivo únicos con correlationId y extensión basada en content-type
            var pilotPhotoExtension = GetFileExtensionFromContentType(pilotPhotoContentType);
            var odometerPhotoExtension = GetFileExtensionFromContentType(odometerPhotoContentType);

            var pilotPhotoBlobPath = $"{basePath}/pilot_{correlationId}{pilotPhotoExtension}";
            var odometerPhotoBlobPath = $"{basePath}/odometer_{correlationId}{odometerPhotoExtension}";

            // Obtener container client
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            
            // Crear container si no existe
            await containerClient.CreateIfNotExistsAsync();

            // Generar SAS para foto de piloto
            var pilotBlobClient = containerClient.GetBlobClient(pilotPhotoBlobPath);
            var pilotSasUrl = GenerateUploadSasUrl(pilotBlobClient, pilotPhotoContentType);

            // Generar SAS para foto de odómetro
            var odometerBlobClient = containerClient.GetBlobClient(odometerPhotoBlobPath);
            var odometerSasUrl = GenerateUploadSasUrl(odometerBlobClient, odometerPhotoContentType);

            var expiresAt = DateTime.UtcNow.AddMinutes(10);

            _logger.LogInformation("SAS URLs generadas exitosamente. CorrelationId: {CorrelationId}, ExpiresAt: {ExpiresAt}",
                correlationId, expiresAt);

            return new BlobSasUploadResult
            {
                PilotPhotoSasUrl = pilotSasUrl,
                OdometerPhotoSasUrl = odometerSasUrl,
                PilotPhotoBlobPath = pilotPhotoBlobPath,
                OdometerPhotoBlobPath = odometerPhotoBlobPath,
                ExpiresAt = expiresAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generando SAS URLs para evidencia. CorrelationId: {CorrelationId}", correlationId);
            throw;
        }
    }

    public async Task<bool> BlobExistsAsync(string blobPath)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobPath);
            return await blobClient.ExistsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verificando existencia de blob: {BlobPath}", blobPath);
            return false;
        }
    }

    public async Task<bool> DeleteBlobAsync(string blobPath)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobPath);
            var response = await blobClient.DeleteIfExistsAsync();
            
            _logger.LogInformation("Blob eliminado: {BlobPath}, Resultado: {Deleted}", blobPath, response.Value);
            return response.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error eliminando blob: {BlobPath}", blobPath);
            return false;
        }
    }

    public async Task<string> GenerateReadSasUrlAsync(string blobPath, int expiresInMinutes = 60)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobPath);

            // Verificar que el blob existe
            if (!await blobClient.ExistsAsync())
            {
                _logger.LogWarning("Blob no existe para generar SAS de lectura: {BlobPath}", blobPath);
                throw new FileNotFoundException($"Blob no encontrado: {blobPath}");
            }

            // Generar SAS con permisos de lectura
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = _containerName,
                BlobName = blobPath,
                Resource = "b", // "b" = blob
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(expiresInMinutes)
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            var sasUrl = blobClient.GenerateSasUri(sasBuilder).ToString();

            _logger.LogDebug("SAS URL de lectura generada para blob: {BlobPath}, Expira en: {ExpiresInMinutes} minutos",
                blobPath, expiresInMinutes);

            return sasUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generando SAS URL de lectura para blob: {BlobPath}", blobPath);
            throw;
        }
    }

    /// <summary>
    /// Genera una SAS URL con permisos de escritura/creación para un blob específico
    /// </summary>
    private string GenerateUploadSasUrl(BlobClient blobClient, string contentType)
    {
        // SAS con permisos mínimos: Write y Create solamente
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _containerName,
            BlobName = blobClient.Name,
            Resource = "b", // "b" = blob
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(10), // TTL de 10 minutos
            ContentType = contentType
        };

        // Permisos mínimos: Write y Create (NO Read, NO Delete)
        sasBuilder.SetPermissions(BlobSasPermissions.Write | BlobSasPermissions.Create);

        var sasUrl = blobClient.GenerateSasUri(sasBuilder).ToString();
        return sasUrl;
    }

    /// <summary>
    /// Obtiene la extensión de archivo apropiada basándose en el Content-Type
    /// </summary>
    private string GetFileExtensionFromContentType(string contentType)
    {
        return contentType.ToLowerInvariant() switch
        {
            "image/jpeg" or "image/jpg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",
            "image/heic" => ".heic",
            "image/heif" => ".heif",
            _ => ".jpg" // Default a .jpg si no reconocemos el tipo
        };
    }
}
