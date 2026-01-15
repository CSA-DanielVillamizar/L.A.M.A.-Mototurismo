# ⚠️ CONFIGURACIÓN REQUERIDA EN BACKEND

## CORS Configuration para Next.js Frontend

El backend .NET 8 debe permitir requests desde el frontend Next.js.

### 📝 Ubicación
`src/Lama.API/Program.cs`

### 🔧 Código a Agregar

```csharp
// ============================================
// AGREGAR ANTES DE: var builder = WebApplication.CreateBuilder(args);
// ============================================

// Leer configuración de CORS desde appsettings.json o usar default
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() 
    ?? new[] { "http://localhost:3000", "https://localhost:3000" };

// ============================================
// AGREGAR DESPUÉS DE: builder.Services.AddControllers();
// ============================================

// Configurar CORS para permitir frontend Next.js
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Si usas cookies/auth
    });

    // Política específica para desarrollo
    options.AddPolicy("Development", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ============================================
// AGREGAR DESPUÉS DE: app.UseHttpsRedirection();
// Y ANTES DE: app.UseAuthorization();
// ============================================

// Habilitar CORS
if (app.Environment.IsDevelopment())
{
    app.UseCors("Development"); // Más permisivo en dev
}
else
{
    app.UseCors(); // Usa política default en producción
}
```

### 📄 Configuración en appsettings.json (Opcional)

```json
{
  "AllowedOrigins": [
    "http://localhost:3000",
    "https://localhost:3000",
    "https://your-production-domain.com"
  ]
}
```

### ✅ Verificación

Después de aplicar los cambios:

1. Reiniciar el backend .NET:
   ```bash
   cd src/Lama.API
   dotnet run
   ```

2. Probar desde el frontend:
   ```bash
   cd src/Lama.Web
   npm run dev
   ```

3. Abrir navegador: `http://localhost:3000/evidence/upload`

4. Verificar en consola del navegador (F12):
   - ✅ No debe haber errores de CORS
   - ✅ Request GET a `/api/MemberStatusTypes` debe funcionar
   - ✅ Debe mostrar "33 tipos de estado cargados desde el backend"

### 🐛 Troubleshooting

#### Error: "CORS policy: No 'Access-Control-Allow-Origin' header"

**Causa**: CORS no configurado o mal configurado

**Solución**:
1. Verificar que `app.UseCors()` esté ANTES de `app.UseAuthorization()`
2. Verificar que la URL del frontend esté en `AllowedOrigins`
3. Reiniciar el backend

#### Error: "Preflight request failed"

**Causa**: Falta `.AllowAnyMethod()` o `.AllowAnyHeader()`

**Solución**:
```csharp
policy.WithOrigins(allowedOrigins)
      .AllowAnyMethod()    // ← Asegurar que esté
      .AllowAnyHeader();   // ← Asegurar que esté
```

### 🔒 Seguridad en Producción

⚠️ **IMPORTANTE**: No usar `AllowAnyOrigin()` en producción

```csharp
// ❌ INCORRECTO en producción
policy.AllowAnyOrigin()
      .AllowAnyMethod()
      .AllowAnyHeader();

// ✅ CORRECTO en producción
policy.WithOrigins("https://your-domain.com")
      .AllowAnyMethod()
      .AllowAnyHeader()
      .AllowCredentials(); // Solo si usas auth
```

### 📚 Referencias

- [ASP.NET Core CORS Documentation](https://learn.microsoft.com/en-us/aspnet/core/security/cors)
- [MDN CORS Guide](https://developer.mozilla.org/en-US/docs/Web/HTTP/CORS)

---

**Última actualización**: 2026-01-15  
**Aplicable a**: .NET 8, ASP.NET Core Web API
