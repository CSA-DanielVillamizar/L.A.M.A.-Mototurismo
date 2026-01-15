using Microsoft.EntityFrameworkCore;
using Lama.Infrastructure.Data;
using Lama.Application.Services;
using Lama.Application.Abstractions;
using Lama.Application.Repositories;
using Lama.Infrastructure.Services;
using Lama.Infrastructure.Repositories;

namespace Lama.API.Extensions;

/// <summary>
/// Extensiones para registrar servicios en el contenedor DI
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra servicios de infraestructura y aplicación
    /// </summary>
    public static IServiceCollection AddLamaServices(this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<LamaDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("LamaDb")));
        
        // Registrar LamaDbContext como ILamaDbContext para MemberStatusService
        services.AddScoped<ILamaDbContext>(provider => provider.GetRequiredService<LamaDbContext>());

        // Repositories
        services.AddScoped<IMemberRepository, MemberRepository>();
        services.AddScoped<IVehicleRepository, VehicleRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IAttendanceRepository, AttendanceRepository>();

        // Services
        services.AddScoped<IMemberStatusService, MemberStatusService>();
        services.AddScoped<IAppConfigProvider, AppConfigProvider>();
        services.AddScoped<IPointsCalculatorService, PointsCalculatorService>();
        services.AddScoped<IBlobStorageService, FakeBlobStorageService>();
        services.AddScoped<IAttendanceConfirmationService, AttendanceConfirmationService>();

        return services;
    }
}
