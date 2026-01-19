using Microsoft.EntityFrameworkCore;
using Lama.Infrastructure.Data;
using Lama.Infrastructure.Data.Interceptors;
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
        // Registrar Interceptor de Auditoría ANTES de registrar DbContext
        services.AddScoped<AuditLoggingInterceptor>();

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
            
            // Registrar interceptor de auditoría en SaveChanges
            var auditingInterceptor = serviceProvider.GetRequiredService<AuditLoggingInterceptor>();
            options.AddInterceptors(auditingInterceptor);
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
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        // Services
        services.AddScoped<IMemberStatusService, MemberStatusService>();
        services.AddScoped<IAppConfigProvider, AppConfigProvider>();
        services.AddScoped<IPointsCalculatorService, PointsCalculatorService>();
        services.AddScoped<IBlobStorageService, FakeBlobStorageService>();
        services.AddScoped<IAttendanceConfirmationService, AttendanceConfirmationService>();
        
        // Authentication Service: Genera JWT tokens y gestiona refresh tokens con Redis
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        
        // Auth Session Service: Gestiona sesiones de aplicación con refresh tokens rotativos
        services.AddScoped<IAuthSessionService, AuthSessionService>();
        
        // Identity User Service: Sincroniza usuarios de Entra ID con base de datos local
        services.AddScoped<IIdentityUserService, IdentityUserService>();

        // Authorization Service: Gestiona roles y scopes de usuarios
        services.AddScoped<IUserAuthorizationService, UserAuthorizationService>();

        // Blob SAS Service: Genera SAS URLs para upload directo a Azure Blob
        // Comentado temporalmente para desarrollo sin Azure
        // services.AddScoped<IBlobSasService, BlobSasService>();

        // Ranking Service: Gestiona snapshots de ranking con actualizaciones incrementales y rebuilds
        services.AddScoped<IRankingService, RankingService>();

        // Audit Service: Registra y consulta auditoría de acciones en el sistema
        services.AddScoped<IAuditService, AuditService>();

        // Cache Service: Servicio de caché distribuido (Redis) para optimización
        services.AddSingleton<ICacheService, CacheService>();

        // Azure Blob Service Client: Cliente para Azure Blob Storage
        // Comentado temporalmente para desarrollo sin Azure
        // services.AddSingleton(sp =>
        // {
        //     var connectionString = configuration.GetConnectionString("AzureStorage");
        //     if (string.IsNullOrEmpty(connectionString))
        //     {
        //         throw new InvalidOperationException("AzureStorage connection string no configurada");
        //     }
        //     return new Azure.Storage.Blobs.BlobServiceClient(connectionString);
        // });

        return services;
    }
}

