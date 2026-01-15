using Lama.Application.Services;

namespace Lama.Infrastructure.Services;

/// <summary>
/// Implementación simulada de IBlobStorageService para desarrollo/testing
/// En producción, reemplazar con Azure Blob Storage
/// </summary>
public class FakeBlobStorageService : IBlobStorageService
{
    private readonly string _basePath;
    private readonly string _baseUrl;

    public FakeBlobStorageService(string basePath = "./uploads", string baseUrl = "https://localhost:7001/files")
    {
        _basePath = basePath;
        _baseUrl = baseUrl;

        // Crear directorio si no existe
        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
        }
    }

    public async Task<string> UploadAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Generar nombre único
            string uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
            string filePath = Path.Combine(_basePath, uniqueFileName);

            // Guardar archivo
            using (var file = File.Create(filePath))
            {
                await fileStream.CopyToAsync(file, cancellationToken);
            }

            // Devolver URL simulada
            return $"{_baseUrl}/{uniqueFileName}";
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error subiendo archivo: {fileName}", ex);
        }
    }

    public async Task<bool> DeleteAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            var fileName = fileUrl.Split('/').Last();
            var filePath = Path.Combine(_basePath, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return await Task.FromResult(true);
            }

            return await Task.FromResult(false);
        }
        catch
        {
            return await Task.FromResult(false);
        }
    }
}
