using Microsoft.AspNetCore.Mvc;

namespace Lama.API.Models;

/// <summary>
/// DTO para upload de evidencia multipart/form-data
/// Evita conflictos de Swagger con múltiples IFormFile
/// </summary>
public class UploadEvidenceFormDto
{
    [FromForm(Name = "memberId")]
    public int MemberId { get; set; }

    [FromForm(Name = "vehicleId")]
    public int VehicleId { get; set; }

    [FromForm(Name = "evidenceType")]
    public string EvidenceType { get; set; } = string.Empty;

    [FromForm(Name = "pilotWithBikePhoto")]
    public IFormFile PilotWithBikePhoto { get; set; } = null!;

    [FromForm(Name = "odometerCloseupPhoto")]
    public IFormFile OdometerCloseupPhoto { get; set; } = null!;

    [FromForm(Name = "odometerReading")]
    public double OdometerReading { get; set; }

    [FromForm(Name = "unit")]
    public string Unit { get; set; } = string.Empty;

    [FromForm(Name = "readingDate")]
    public DateOnly? ReadingDate { get; set; }

    [FromForm(Name = "notes")]
    public string? Notes { get; set; }
}
