using Lama.API.Extensions;
using Lama.API.Middleware;

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

        // Middleware de Multi-Tenancy: DEBE estar antes de UseCors para resolver tenant temprano
        app.UseMiddleware<TenantResolutionMiddleware>();

        app.UseCors("AllowAll");
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
