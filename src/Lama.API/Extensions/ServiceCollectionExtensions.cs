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
        // Multi-Tenancy: Registrar TenantContext como Scoped (una instancia por request)
        services.AddScoped<TenantContext>();
        services.AddScoped<ITenantProvider>(provider => provider.GetRequiredService<TenantContext>());

        // DbContext con inyección de TenantProvider para query filters
        services.AddDbContext<LamaDbContext>((serviceProvider, options) =>
        {
            options.UseSqlServer(configuration.GetConnectionString("LamaDb"));
            // Inyectar TenantProvider al crear LamaDbContext
            var tenantProvider = serviceProvider.GetService<ITenantProvider>();
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });
        
        // Alternativa: Registrar LamaDbContext como ILamaDbContext (sin TenantProvider en constructor principal)
        // services.AddDbContext<LamaDbContext>(options =>
        //     options.UseSqlServer(configuration.GetConnectionString("LamaDb")));
        
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
        
        // Identity User Service: Sincroniza usuarios de Entra ID con base de datos local
        services.AddScoped<IIdentityUserService, IdentityUserService>();

        // Authorization Service: Gestiona roles y scopes de usuarios
        services.AddScoped<IUserAuthorizationService, UserAuthorizationService>();

        return services;
    }
}

