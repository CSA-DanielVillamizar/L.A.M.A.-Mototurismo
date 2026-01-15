using Microsoft.EntityFrameworkCore;
using Lama.Domain.Entities;
using Lama.Domain.Enums;
using Lama.Application.Repositories;
using Lama.Application.Services;
using Lama.Infrastructure.Data;

namespace Lama.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de confirmación de asistencia
/// </summary>
public class AttendanceConfirmationService(
    IAttendanceRepository attendanceRepository,
    IVehicleRepository vehicleRepository,
    IEventRepository eventRepository,
    IBlobStorageService blobStorageService,
    IPointsCalculatorService pointsCalculatorService,
    LamaDbContext context) : IAttendanceConfirmationService
{
    private readonly IAttendanceRepository _attendanceRepository = attendanceRepository;
    private readonly IVehicleRepository _vehicleRepository = vehicleRepository;
    private readonly IEventRepository _eventRepository = eventRepository;
    private readonly IBlobStorageService _blobStorageService = blobStorageService;
    private readonly IPointsCalculatorService _pointsCalculatorService = pointsCalculatorService;
    private readonly LamaDbContext _context = context;

    public async Task<AttendanceConfirmationResult> ConfirmAttendanceAsync(
        int eventId,
        UploadEvidenceRequest request,
        int validatedByMemberId,
        CancellationToken cancellationToken = default)
    {
        var result = new AttendanceConfirmationResult();

        try
        {
            // Iniciar transacción
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                // 1. Validar datos
                var @event = await _eventRepository.GetByIdAsync(eventId, cancellationToken)
                    ?? throw new InvalidOperationException($"Evento {eventId} no encontrado");

                var vehicle = await _vehicleRepository.GetByIdAsync(request.VehicleId, cancellationToken)
                    ?? throw new InvalidOperationException($"Vehículo {request.VehicleId} no encontrado");

                var member = vehicle.Member
                    ?? throw new InvalidOperationException("Miembro del vehículo no encontrado");

                var attendance = await _attendanceRepository.GetByMemberEventAsync(request.MemberId, eventId, cancellationToken)
                    ?? throw new InvalidOperationException($"Asistencia no encontrada para miembro {request.MemberId} en evento {eventId}");

                // 2. Subir fotos a blob storage
                var pilotPhotoUrl = await _blobStorageService.UploadAsync(
                    request.PilotWithBikePhotoStream,
                    request.PilotWithBikePhotoFileName,
                    "image/jpeg",
                    cancellationToken);

                var odometerPhotoUrl = await _blobStorageService.UploadAsync(
                    request.OdometerCloseupPhotoStream,
                    request.OdometerCloseupPhotoFileName,
                    "image/jpeg",
                    cancellationToken);

                // 3. Actualizar vehículo con datos de odómetro y evidencia
                var evidenceType = request.EvidenceType.ToUpper();
                var readingDate = request.ReadingDate ?? DateOnly.FromDateTime(DateTime.UtcNow);

                if (evidenceType == "START_YEAR")
                {
                    vehicle.StartingOdometer = request.OdometerReading;
                    vehicle.StartingOdometerDate = readingDate;
                    vehicle.StartYearEvidenceUrl = pilotPhotoUrl;
                    vehicle.StartYearEvidenceValidatedAt = DateTime.UtcNow;
                }
                else if (evidenceType == "CUTOFF")
                {
                    vehicle.FinalOdometer = request.OdometerReading;
                    vehicle.FinalOdometerDate = readingDate;
                    vehicle.CutOffEvidenceUrl = pilotPhotoUrl;
                    vehicle.CutOffEvidenceValidatedAt = DateTime.UtcNow;
                }

                vehicle.OdometerUnit = request.Unit;
                vehicle.EvidenceValidatedBy = validatedByMemberId;
                vehicle.Photography = "VALIDATED";
                vehicle.UpdatedAt = DateTime.UtcNow;

                await _vehicleRepository.UpdateAsync(vehicle, cancellationToken);

                // 4. Calcular puntos
                // Convertir a millas si es necesario
                double mileageInMiles = @event.Mileage;
                if (@event.Mileage > 0)
                {
                    // Asumir que el evento ya está en millas
                    mileageInMiles = @event.Mileage;
                }

                var pointsCalculation = await _pointsCalculatorService.CalculateAsync(
                    mileageInMiles,
                    @event.Class,
                    member.CountryBirth,
                    member.Continent,
                    @event.StartLocationCountry,
                    @event.StartLocationContinent,
                    cancellationToken);

                // 5. Actualizar asistencia
                attendance.Status = "CONFIRMED";
                attendance.PointsPerEvent = pointsCalculation.PointsPerEvent;
                attendance.PointsPerDistance = pointsCalculation.PointsPerDistance;
                attendance.PointsAwardedPerMember = pointsCalculation.TotalPoints;
                attendance.VisitorClass = pointsCalculation.VisitorClassification.ToString();
                attendance.ConfirmedAt = DateTime.UtcNow;
                attendance.ConfirmedBy = validatedByMemberId;
                attendance.UpdatedAt = DateTime.UtcNow;

                await _attendanceRepository.UpdateAsync(attendance, cancellationToken);
                await _attendanceRepository.SaveChangesAsync(cancellationToken);

                // 6. Commit
                await transaction.CommitAsync(cancellationToken);

                // 7. Preparar resultado
                result.Success = true;
                result.Message = $"Asistencia confirmada exitosamente. Puntos: {pointsCalculation.TotalPoints}";
                result.AttendanceId = attendance.Id;
                result.PointsPerEvent = pointsCalculation.PointsPerEvent;
                result.PointsPerDistance = pointsCalculation.PointsPerDistance;
                result.PointsAwardedPerMember = pointsCalculation.TotalPoints;
                result.VisitorClass = pointsCalculation.VisitorClassification.ToString();
                result.MemberId = request.MemberId;
                result.VehicleId = request.VehicleId;

                return result;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = $"Error confirmando asistencia: {ex.Message}";
            return result;
        }
    }
}
