using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Routing;
using System.Threading.RateLimiting;
using Microsoft.Identity.Web;
using Lama.API.Extensions;
using Lama.API.Middleware;
using Lama.API.Authorization;
using Lama.API.Routing;
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
        builder.Services.AddRouting(options =>
        {
            options.LowercaseUrls = true;
            options.LowercaseQueryStrings = false;
        });

        builder.Services
            .AddControllers(options =>
            {
                options.Conventions.Add(new RouteTokenTransformerConvention(new KebabCaseParameterTransformer()));
            })
            .AddJsonOptions(options =>
            {
                // Respetar PascalCase en el contrato JSON (API + ProblemDetails)
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

        builder.Services.AddProblemDetails();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "COR L.A.MA API",
                Version = "v1",
                Description = "API oficial para COR L.A.MA con versionado /api/v1, ProblemDetails y ejemplos de uso"
            });
            options.OperationFilter<Lama.API.Swagger.OperationExamplesFilter>();
        });

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

        // Configurar Redis para caché distribuido (IDistributedCache)
        var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
        if (!string.IsNullOrEmpty(redisConnectionString))
        {
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "Lama:"; // Prefijo para todas las claves
            });
        }
        else
        {
            // Fallback a MemoryCache si Redis no está configurado
            builder.Services.AddDistributedMemoryCache();
        }

        // Configurar Rate Limiting (ASP.NET Core 7+)
        builder.Services.AddRateLimiter(options =>
        {
            // Política para login: 10 req/min por IP
            options.AddFixedWindowLimiter("login", limiterOptions =>
            {
                limiterOptions.PermitLimit = 10;
                limiterOptions.Window = TimeSpan.FromMinutes(1);
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 3;
            });

            // Política para signup: 5 req/min por IP
            options.AddFixedWindowLimiter("signup", limiterOptions =>
            {
                limiterOptions.PermitLimit = 5;
                limiterOptions.Window = TimeSpan.FromMinutes(1);
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 1;
            });

            // Política para forgot password: 3 req/min por IP
            options.AddFixedWindowLimiter("forgotPassword", limiterOptions =>
            {
                limiterOptions.PermitLimit = 3;
                limiterOptions.Window = TimeSpan.FromMinutes(1);
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 1;
            });

            // Política para endpoints de búsqueda: 30 req/min por IP
            options.AddFixedWindowLimiter("search", limiterOptions =>
            {
                limiterOptions.PermitLimit = 30;
                limiterOptions.Window = TimeSpan.FromMinutes(1);
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 5;
            });

            // Política para uploads: 10 req/min por IP
            options.AddFixedWindowLimiter("upload", limiterOptions =>
            {
                limiterOptions.PermitLimit = 10;
                limiterOptions.Window = TimeSpan.FromMinutes(1);
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 2;
            });

            // Respuesta cuando se excede el límite
            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.HttpContext.Response.WriteAsync(
                    "Rate limit exceeded. Please try again later.", cancellationToken);
            };
        });

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

            // Política: Admin general - para cualquier rol de admin (ADMIN_CHAPTER o superior)
            // Usado para endpoints /admin/* que requieren permisos administrativos
            options.AddPolicy("AdminOnly", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                {
                    var userRoles = context.User.FindAll("role").Select(c => c.Value).ToList();
                    return userRoles.Any(role => role.StartsWith("ADMIN_") || role == "SUPER_ADMIN");
                });
            });
        });

        // Configurar CORS para soporte de credentials (cookies httpOnly)
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                var frontendUrl = builder.Configuration["Frontend:Url"] ?? "http://localhost:3002";
                policy.WithOrigins(frontendUrl)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials(); // Permitir envío de cookies
            });

            // Política permisiva para DEV (usar solo en desarrollo)
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
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/error");
        }

        app.UseSwagger();
        app.UseSwaggerUI();

        // HTTPS Redirection: Solo en producción
        if (!app.Environment.IsDevelopment())
        {
            app.UseHttpsRedirection();
        }

        // Middleware de CorrelationId: DEBE estar al principio para capturar todas las solicitudes
        // Asigna ID único a cada solicitud para rastreo distribuido
        app.UseCorrelationIdMiddleware();

            // CORS: DEBE estar ANTES de autenticación para que preflight OPTIONS funcione
            app.UseCors("AllowFrontend");

        // Rate Limiting: aplicar ANTES de autenticación
        app.UseRateLimiter();

        // Middleware de Multi-Tenancy: Resuelve tenant temprano
        app.UseMiddleware<TenantResolutionMiddleware>();

        // Middleware de autenticación y autorización JWT
        app.UseAuthentication();
        app.UseAuthorization();

        // Middleware para sincronizar IdentityUser después de autenticación exitosa
        app.UseMiddleware<IdentityUserSyncMiddleware>();

        app.MapControllers();

        // Endpoint de health check para testing
        app.MapGet("/health", () => "OK");

        app.Run();
    }
}
