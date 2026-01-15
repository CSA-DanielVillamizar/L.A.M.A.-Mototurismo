using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Lama.API.Extensions;
using Lama.API.Middleware;
using Lama.Infrastructure.Options;

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

        // Middleware de Multi-Tenancy: DEBE estar antes de autenticación para resolver tenant temprano
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
