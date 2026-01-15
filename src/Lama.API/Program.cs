using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using Lama.API.Extensions;
using Lama.API.Middleware;
using Lama.API.Authorization;
using Lama.Infrastructure.Options;
using Lama.Domain.Entities;

namespace Lama.API;

/// <summary>
/// Clase principal de la aplicación API
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Configurar opciones de Azure AD desde appsettings
        var azureAdSection = builder.Configuration.GetSection("AzureAd");
        builder.Services.Configure<AzureAdOptions>(azureAdSection);

        // Configurar autenticación JWT Bearer con Microsoft Entra External ID (B2C)
        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(options =>
            {
                // Configurar opciones de validación de tokens
                builder.Configuration.Bind("AzureAd", options);
                options.Authority = azureAdSection["Authority"];
                options.Audience = azureAdSection["Audience"];

                // En DEBUG: permitir tokens sin validar issuer para testing local
#if DEBUG
                options.TokenValidationParameters.ValidateIssuer = false;
                options.TokenValidationParameters.ValidateIssuerSigningKey = false;
                options.TokenValidationParameters.ValidateAudience = false;
#endif
            },
            options =>
            {
                builder.Configuration.Bind("AzureAd", options);
                options.Authority = azureAdSection["Authority"];
            });

        // Registrar servicios de LAMA
        builder.Services.AddLamaServices(builder.Configuration);

        // Registrar handler de autorización personalizado (SCOPED - no Singleton)
        builder.Services.AddScoped<IAuthorizationHandler, ScopeAuthorizationHandler>();

        // Configurar políticas de autorización basadas en roles y scopes
        builder.Services.AddAuthorization(options =>
        {
            // Política: Validar eventos - requiere ser MTO o superior del capítulo
            options.AddPolicy("CanValidateEvent", policy =>
                policy.Requirements.Add(new ResourceAuthorizationRequirement(
                    RoleType.MTO_CHAPTER, 
                    ScopeType.CHAPTER)));

            // Política: Gestionar capítulo - requiere ser Admin de capítulo o superior
            options.AddPolicy("CanManageChapter", policy =>
                policy.Requirements.Add(new ResourceAuthorizationRequirement(
                    RoleType.ADMIN_CHAPTER,
                    ScopeType.CHAPTER)));

            // Política: Gestionar país - requiere ser Admin nacional o superior
            options.AddPolicy("CanManageCountry", policy =>
                policy.Requirements.Add(new ResourceAuthorizationRequirement(
                    RoleType.ADMIN_NATIONAL,
                    ScopeType.COUNTRY)));

            // Política: Gestionar continente - requiere ser Admin continental o superior
            options.AddPolicy("CanManageContinent", policy =>
                policy.Requirements.Add(new ResourceAuthorizationRequirement(
                    RoleType.ADMIN_CONTINENT,
                    ScopeType.CONTINENT)));

            // Política: Acceso global - requiere ser Admin internacional o superior
            options.AddPolicy("CanManageGlobal", policy =>
                policy.Requirements.Add(new ResourceAuthorizationRequirement(
                    RoleType.ADMIN_INTERNATIONAL,
                    ScopeType.GLOBAL)));

            // Política: Super Admin - solo para SUPER_ADMIN
            options.AddPolicy("IsSuperAdmin", policy =>
                policy.Requirements.Add(new ResourceAuthorizationRequirement(
                    RoleType.SUPER_ADMIN,
                    ScopeType.GLOBAL)));
        });

        // Configurar CORS (opcional)
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        // Middleware de CorrelationId: DEBE estar al principio para capturar todas las solicitudes
        // Asigna ID único a cada solicitud para rastreo distribuido
        app.UseCorrelationIdMiddleware();

        // Middleware de Multi-Tenancy: Resuelve tenant temprano
        app.UseMiddleware<TenantResolutionMiddleware>();

        // Middleware de autenticación y autorización JWT
        app.UseAuthentication();
        app.UseAuthorization();

        // Middleware para sincronizar IdentityUser después de autenticación exitosa
        app.UseMiddleware<IdentityUserSyncMiddleware>();

        app.UseCors("AllowAll");
        app.MapControllers();

        app.Run();
    }
}
