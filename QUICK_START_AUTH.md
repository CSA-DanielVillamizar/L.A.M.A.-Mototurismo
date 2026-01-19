# Quick Start: Auth Implementation

## Backend Setup (Completado ✅)

### 1. Archivos Creados

**Domain Layer:**
- `src/Lama.Domain/Entities/RefreshToken.cs` - Entidad de refresh token rotativo

**Application Layer:**
- `src/Lama.Application/Abstractions/IRefreshTokenRepository.cs` - Repositorio de tokens
- `src/Lama.Application/Services/IAuthSessionService.cs` - Servicio de sesiones
- `src/Lama.Application/DTOs/AuthDtos.cs` - DTOs para auth (ExchangeTokenRequest, AuthSessionResponse, UserInfo, LogoutResponse)

**Infrastructure Layer:**
- `src/Lama.Infrastructure/Data/Configurations/RefreshTokenConfiguration.cs` - EF Core config
- `src/Lama.Infrastructure/Repositories/RefreshTokenRepository.cs` - Implementación repositorio
- `src/Lama.Infrastructure/Services/AuthSessionService.cs` - Implementación servicio

**API Layer:**
- `src/Lama.API/Controllers/AuthController.cs` - Endpoints actualizados:
  - `POST /api/v1/auth/exchange` - Exchange Entra token
  - `POST /api/v1/auth/refresh-session` - Refresh rotativo
  - `POST /api/v1/auth/logout-session` - Logout
  - `GET /api/v1/auth/me` - Usuario actual

### 2. Configuración Actualizada

**Program.cs:**
- ✅ CORS con `AllowCredentials()` para cookies
- ✅ Política `AllowFrontend` para producción

**appsettings.json:**
- ✅ Sección `Jwt` con config de tokens
- ✅ Sección `Frontend` con URL permitida
- ✅ Flag `Development:AllowInsecureCookies` para dev

**ServiceCollectionExtensions.cs:**
- ✅ Registro de `IRefreshTokenRepository` y `IAuthSessionService`

### 3. Pendiente: Base de Datos

```powershell
# Crear migration
cd src\Lama.Infrastructure
dotnet ef migrations add AddRefreshTokens --startup-project ..\Lama.API

# Aplicar migration
dotnet ef database update --startup-project ..\Lama.API
```

### 4. Configurar User Secrets (Recomendado)

```powershell
cd src\Lama.API

# Inicializar user secrets
dotnet user-secrets init

# Configurar JWT secret
dotnet user-secrets set "Jwt:SecretKey" "YOUR_SECURE_32_CHAR_SECRET_KEY_HERE"

# Configurar Entra ID (opcional si difiere de appsettings)
dotnet user-secrets set "AzureAd:Authority" "https://YOUR_TENANT.b2clogin.com/..."
dotnet user-secrets set "AzureAd:ClientId" "YOUR_CLIENT_ID"
dotnet user-secrets set "AzureAd:ClientSecret" "YOUR_CLIENT_SECRET"
```

### 5. Build & Run

```powershell
cd src\Lama.API
dotnet build
dotnet run
```

Backend estará en: `http://localhost:5000` (o puerto configurado)

---

## Frontend Setup (Pendiente ⏳)

### 1. Archivos a Crear

**Context:**
- `src/Lama.Web/contexts/AuthContext.tsx` - React context para auth

**Hooks:**
- `src/Lama.Web/hooks/useAuth.ts` - Hook de autenticación

**API Client:**
- Actualizar `src/Lama.Web/lib/api-client.ts` con métodos:
  - `exchangeEntraToken(idToken)`
  - `refreshSession()`
  - `logout()`
  - `getCurrentUser()`

**Components:**
- `src/Lama.Web/components/auth/LoginButton.tsx` - Botón login con Entra
- `src/Lama.Web/components/auth/ProtectedRoute.tsx` - Proteger rutas
- `src/Lama.Web/components/auth/AuthGuard.tsx` - Guard con roles

**Pages:**
- `src/Lama.Web/app/login/page.tsx` - Página de login
- `src/Lama.Web/app/auth/callback/page.tsx` - Callback Entra ID

### 2. Flujo de Implementación

```typescript
// 1. AuthContext - Almacenar access token y user en memoria
const AuthContext = createContext<AuthContextType>({
  user: null,
  accessToken: null,
  isAuthenticated: false,
  isLoading: true,
  login: async (idToken) => {},
  logout: async () => {},
  refreshToken: async () => {}
});

// 2. useAuth hook - Consumir context
export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) throw new Error('useAuth must be used within AuthProvider');
  return context;
};

// 3. API Client - fetch con credentials
const apiClient = {
  exchangeToken: async (idToken: string) => {
    const response = await fetch('/api/v1/auth/exchange', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ idToken }),
      credentials: 'include' // IMPORTANTE: enviar cookies
    });
    return response.json();
  },
  
  refreshSession: async () => {
    const response = await fetch('/api/v1/auth/refresh-session', {
      method: 'POST',
      credentials: 'include' // Cookie auto-enviada
    });
    return response.json();
  }
};

// 4. Interceptor para 401 - Auto-refresh
apiClient.interceptors.response.use(
  response => response,
  async error => {
    if (error.response?.status === 401) {
      try {
        await apiClient.refreshSession();
        // Reintentar request original
        return apiClient(error.config);
      } catch (refreshError) {
        // Refresh falló - redirigir a login
        window.location.href = '/login';
      }
    }
    throw error;
  }
);
```

### 3. UI Premium (Stripe-like)

**Principios:**
- ❌ Sin emojis
- ✅ Iconos lucide-react
- ✅ Colores neutros (grays, blues)
- ✅ Bordes sutiles (border-gray-200)
- ✅ Sombras suaves (shadow-sm, shadow-md)
- ✅ Tipografía clara (font-sans, font-medium)
- ✅ Spacing consistente (Tailwind spacing scale)
- ✅ Estados loading skeleton
- ✅ Transiciones suaves (transition-all)

**Componentes Base:**
```tsx
// Button
<button className="px-4 py-2 bg-blue-600 text-white rounded-md 
  hover:bg-blue-700 transition-colors font-medium shadow-sm">
  Login
</button>

// Card
<div className="bg-white rounded-lg border border-gray-200 shadow-sm p-6">
  <h2 className="text-lg font-semibold text-gray-900">Title</h2>
  <p className="text-sm text-gray-600">Description</p>
</div>

// Input
<input className="px-3 py-2 border border-gray-300 rounded-md 
  focus:outline-none focus:ring-2 focus:ring-blue-500" />
```

---

## Testing Auth Flow

### Manual Test (Postman/cURL)

```bash
# 1. Simular Entra token (obtener real de Entra ID o mock)
ENTRA_TOKEN="eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIs..."

# 2. Exchange token
curl -X POST http://localhost:5000/api/v1/auth/exchange \
  -H "Content-Type: application/json" \
  -d "{\"idToken\": \"$ENTRA_TOKEN\"}" \
  -c cookies.txt

# 3. Llamar API protegida
ACCESS_TOKEN="eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
curl -X GET http://localhost:5000/api/v1/auth/me \
  -H "Authorization: Bearer $ACCESS_TOKEN"

# 4. Refresh (esperar 15 min o expirar token)
curl -X POST http://localhost:5000/api/v1/auth/refresh-session \
  -b cookies.txt \
  -c cookies.txt

# 5. Logout
curl -X POST http://localhost:5000/api/v1/auth/logout-session \
  -b cookies.txt
```

### Integration Test (xUnit)

```csharp
[Fact]
public async Task ExchangeEntraToken_ValidToken_ReturnsSessionWithCookie()
{
    // Arrange
    var idToken = "mock_entra_token";
    var request = new ExchangeTokenRequest { IdToken = idToken };
    
    // Act
    var response = await _client.PostAsJsonAsync("/api/v1/auth/exchange", request);
    
    // Assert
    response.EnsureSuccessStatusCode();
    var session = await response.Content.ReadFromJsonAsync<AuthSessionResponse>();
    Assert.NotNull(session.AccessToken);
    Assert.NotNull(session.User);
    
    // Verificar cookie
    var cookies = response.Headers.GetValues("Set-Cookie");
    Assert.Contains(cookies, c => c.Contains("refresh_token") && c.Contains("HttpOnly"));
}
```

---

## Deployment Checklist

### Backend

- [ ] Configurar Azure Key Vault para secrets
- [ ] Actualizar `Jwt:SecretKey` con secret seguro (32+ chars)
- [ ] Configurar `Frontend:Url` con URL de producción
- [ ] Cambiar `Development:AllowInsecureCookies` a `false`
- [ ] Habilitar HTTPS (certificado SSL)
- [ ] Configurar Application Insights para logging
- [ ] Ejecutar migration `AddRefreshTokens` en DB de producción
- [ ] Configurar Azure SQL Database con firewall rules
- [ ] Configurar Redis Cache (Azure Cache for Redis)
- [ ] Revisar CORS policy (solo frontend de producción)

### Frontend

- [ ] Configurar Entra ID redirect URI para producción
- [ ] Actualizar `NEXT_PUBLIC_API_BASE_URL` con API de producción
- [ ] Implementar Content Security Policy (CSP)
- [ ] Configurar SameSite=None si cross-site (requiere HTTPS)
- [ ] Habilitar HTTPS (certificado SSL)
- [ ] Configurar CDN (Azure Front Door o Cloudflare)
- [ ] Implementar error monitoring (Sentry)
- [ ] Test de accesibilidad (WCAG)

### Monitoring

- [ ] Configurar alertas en detección de reuso de tokens
- [ ] Dashboard de métricas de auth (login rate, errors, refresh rate)
- [ ] Logs estructurados en Azure Log Analytics
- [ ] Audit trail de acciones críticas

---

## Próximos Pasos

1. **Ejecutar migration** de RefreshTokens
2. **Crear AuthProvider** en frontend
3. **Implementar páginas** de login y callback
4. **Integrar Entra ID** con redirect flow
5. **Probar end-to-end** auth flow
6. **Implementar UI premium** para portales admin/member
7. **Escribir tests** de integración

---

## Referencias Rápidas

- [ARCHITECTURE_AUTH.md](../ARCHITECTURE_AUTH.md) - Arquitectura completa
- [appsettings.example.json](src/Lama.API/appsettings.example.json) - Config ejemplo
- [Program.cs](src/Lama.API/Program.cs) - Configuración ASP.NET
- [AuthController.cs](src/Lama.API/Controllers/AuthController.cs) - Endpoints
