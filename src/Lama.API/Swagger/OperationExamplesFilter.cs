using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lama.API.Swagger;

/// <summary>
/// Agrega ejemplos representativos para requests/responses en el documento OpenAPI.
/// </summary>
public class OperationExamplesFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var path = (context.ApiDescription.RelativePath ?? string.Empty).ToLowerInvariant();

        AddMemberSearchExamples(operation, path);
        AddEventsExamples(operation, path);
        AddEvidenceUploadRequestExamples(operation, path);
        AddEvidenceSubmitExamples(operation, path);
        AddAdminEvidenceUploadExamples(operation, path);
    }

    private static void AddMemberSearchExamples(OpenApiOperation operation, string path)
    {
        if (!path.Contains("members/search")) return;

        if (operation.Responses.TryGetValue("200", out var okResponse) && okResponse.Content.TryGetValue("application/json", out var json))
        {
            json.Example = new OpenApiArray
            {
                new OpenApiObject
                {
                    ["MemberId"] = new OpenApiInteger(101),
                    ["FirstName"] = new OpenApiString("Maria"),
                    ["LastName"] = new OpenApiString("Lopez"),
                    ["FullName"] = new OpenApiString("Maria Lopez"),
                    ["Status"] = new OpenApiString("ACTIVE"),
                    ["ChapterId"] = new OpenApiInteger(12)
                },
                new OpenApiObject
                {
                    ["MemberId"] = new OpenApiInteger(102),
                    ["FirstName"] = new OpenApiString("Jose"),
                    ["LastName"] = new OpenApiString("Perez"),
                    ["FullName"] = new OpenApiString("Jose Perez"),
                    ["Status"] = new OpenApiString("ACTIVE"),
                    ["ChapterId"] = new OpenApiInteger(12)
                }
            };
        }
    }

    private static void AddEventsExamples(OpenApiOperation operation, string path)
    {
        if (!path.Contains("events")) return;

        if (operation.Responses.TryGetValue("200", out var okResponse) && okResponse.Content.TryGetValue("application/json", out var json))
        {
            json.Example = new OpenApiArray
            {
                new OpenApiObject
                {
                    ["Id"] = new OpenApiInteger(501),
                    ["Name"] = new OpenApiString("Continental Ride"),
                    ["Location"] = new OpenApiString("Buenos Aires, AR"),
                    ["EventStartDate"] = new OpenApiString("2026-03-10T09:00:00Z"),
                    ["EventEndDate"] = new OpenApiString("2026-03-12T18:00:00Z"),
                    ["ChapterId"] = new OpenApiInteger(7)
                }
            };
        }
    }

    private static void AddEvidenceUploadRequestExamples(OpenApiOperation operation, string path)
    {
        if (!path.Contains("evidence/upload-request")) return;

        if (operation.RequestBody?.Content.TryGetValue("application/json", out var body) == true)
        {
            body.Example = new OpenApiObject
            {
                ["MemberId"] = new OpenApiInteger(101),
                ["VehicleId"] = new OpenApiInteger(3001),
                ["EventId"] = new OpenApiInteger(501),
                ["EvidenceType"] = new OpenApiString("START_YEAR"),
                ["PilotPhotoContentType"] = new OpenApiString("image/jpeg"),
                ["OdometerPhotoContentType"] = new OpenApiString("image/jpeg")
            };
        }

        if (operation.Responses.TryGetValue("200", out var okResponse) && okResponse.Content.TryGetValue("application/json", out var json))
        {
            json.Example = new OpenApiObject
            {
                ["CorrelationId"] = new OpenApiString("7f5caa6c2c324c1b8c4c9d1d0a123456"),
                ["PilotPhotoSasUrl"] = new OpenApiString("https://storage.blob.core.windows.net/evidence/pilot.jpg?..."),
                ["OdometerPhotoSasUrl"] = new OpenApiString("https://storage.blob.core.windows.net/evidence/odometer.jpg?..."),
                ["PilotPhotoBlobPath"] = new OpenApiString("tenants/1/evidence/pilot.jpg"),
                ["OdometerPhotoBlobPath"] = new OpenApiString("tenants/1/evidence/odometer.jpg"),
                ["ExpiresAt"] = new OpenApiString("2026-01-15T23:59:59Z")
            };
        }
    }

    private static void AddEvidenceSubmitExamples(OpenApiOperation operation, string path)
    {
        if (!path.Contains("evidence/submit")) return;

        if (operation.RequestBody?.Content.TryGetValue("application/json", out var body) == true)
        {
            body.Example = new OpenApiObject
            {
                ["CorrelationId"] = new OpenApiString("7f5caa6c2c324c1b8c4c9d1d0a123456"),
                ["MemberId"] = new OpenApiInteger(101),
                ["VehicleId"] = new OpenApiInteger(3001),
                ["EventId"] = new OpenApiInteger(501),
                ["EvidenceType"] = new OpenApiString("START_YEAR"),
                ["PilotPhotoBlobPath"] = new OpenApiString("tenants/1/evidence/pilot.jpg"),
                ["OdometerPhotoBlobPath"] = new OpenApiString("tenants/1/evidence/odometer.jpg"),
                ["OdometerReading"] = new OpenApiInteger(10500),
                ["OdometerUnit"] = new OpenApiString("Miles")
            };
        }
    }

    private static void AddAdminEvidenceUploadExamples(OpenApiOperation operation, string path)
    {
        if (!path.Contains("admin/evidence/upload")) return;

        if (operation.RequestBody?.Content.TryGetValue("multipart/form-data", out var body) == true)
        {
            body.Example = new OpenApiObject
            {
                ["eventId"] = new OpenApiInteger(501),
                ["memberId"] = new OpenApiInteger(101),
                ["vehicleId"] = new OpenApiInteger(3001),
                ["evidenceType"] = new OpenApiString("CUTOFF"),
                ["pilotWithBikePhoto"] = new OpenApiString("(binary JPEG)")
            };
        }
    }
}
