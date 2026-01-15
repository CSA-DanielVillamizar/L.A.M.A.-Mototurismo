# Changelog - LAMA Web Frontend

## [1.1.0] - 2026-01-15

### ✨ Sprint UX COR - Evidence Upload Form Improvements

**Nuevos Features:**
- ✅ Select de eventos con carga automática desde `/api/events`
- ✅ Autocomplete de miembros con búsqueda en tiempo real (debounce 300ms)
- ✅ Select dinámico de vehículos que se carga al seleccionar un miembro
- ✅ Validación automática de existencia de entities
- ✅ Estados de loading independientes para cada dropdown
- ✅ Experiencia de usuario profesional sin necesidad de conocer IDs

**Backend - Nuevos Endpoints:**
- GET `/api/events` - Listar eventos disponibles
- GET `/api/members/search?q={query}` - Buscar miembros (min. 2 caracteres)
- GET `/api/members/{memberId}/vehicles` - Obtener vehículos de un miembro

**Backend - API Refactoring:**
- Rutas cambiadas a kebab-case para consistencia REST:
  - ❌ `/api/MemberStatusTypes` → ✅ `/api/member-status-types`
  - ❌ `/api/MemberStatusTypes/by-category/{category}` → ✅ `/api/member-status-types/by-category/{category}`
  - ❌ `/api/MemberStatusTypes/categories` → ✅ `/api/member-status-types/categories`

**Frontend - UX Improvements:**
- Reemplazados inputs manuales de IDs por controles inteligentes:
  - Event ID: `<input type="number">` → `<select>` con eventos reales
  - Member ID: `<input type="number">` → `<input>` con autocomplete
  - Vehicle ID: `<input type="number">` → `<select>` dinámico

**Frontend - Technical:**
- `api-client.ts`: 3 nuevos métodos agregados
- `types/api.ts`: 3 nuevas interfaces (Event, MemberSearchResult, Vehicle)
- `EvidenceUploadForm.tsx`: Reescrito con arquitectura de componentes mejorada
- Backup del formulario original guardado como `EvidenceUploadForm.tsx.backup`

**Documentation:**
- `SPRINT_UX_COR.md` creado con documentación completa del sprint
- Flujo de usuario detallado paso a paso
- Comparación antes/después con ejemplos visuales

**Testing:**
```bash
# Backend
cd src/Lama.API
dotnet run --configuration Debug

# Frontend
cd src/Lama.Web
npm install
npm run dev
```

Luego abrir http://localhost:3000/evidence/upload

---

## [1.0.2] - 2026-01-15

### ⚙️ Backend: Development Auth Bypass Configurado

**Cambio en Backend (Lama.API):**
- AdminController ahora usa `#if !DEBUG` para deshabilitar `[Authorize]` en Development
- Permite probar `POST /api/admin/evidence/upload` sin autenticación
- Production permanece protegido con `[Authorize(Roles = "MTO,Admin")]`

**Impacto en Frontend:**
- ✅ `/evidence/upload` funciona directamente en Development (localhost:5000)
- ✅ No requiere JWT/OAuth para testing local
- ✅ Formulario puede subir evidencias sin headers de Authorization
- ⚠️ Production requerirá configurar autenticación real

**Archivos Backend:**
- `src/Lama.API/Controllers/AdminController.cs` - Directiva `#if !DEBUG` agregada
- `src/Lama.API/DEVELOPMENT_AUTH_BYPASS.md` - Documentación completa

**Testing:**
```bash
# Verificar que funciona sin autenticación:
curl -X POST "http://localhost:5000/api/admin/evidence/upload?eventId=1" \
  -F "memberId=1" -F "vehicleId=1" \
  -F "evidenceType=START_YEAR" \
  -F "pilotWithBikePhoto=@test.jpg" \
  -F "odometerCloseupPhoto=@test.jpg" \
  -F "odometerReading=100" -F "unit=Miles"
```

Esperado: `200 OK` con respuesta JSON (puntos calculados)

---

## [1.0.1] - 2026-01-15

### 🐛 Bugfix: Endpoints API Corregidos

**Problema**: 
El frontend estaba usando rutas incorrectas para los endpoints de MemberStatusTypes:
- ❌ `/api/memberstatus` (incorrecto, minúscula)
- ❌ `/api/memberstatus/by-category/{category}`
- ❌ `/api/memberstatus/categories`

**Causa**: 
El backend .NET usa `[Route("api/[controller]")]` en `MemberStatusTypesController`, lo que resulta en rutas con PascalCase.

**Solución**:
Rutas corregidas a las reales del backend:
- ✅ `/api/MemberStatusTypes` (correcto, PascalCase)
- ✅ `/api/MemberStatusTypes/by-category/{category}`
- ✅ `/api/MemberStatusTypes/categories`
- ✅ `/api/MemberStatusTypes/by-name/{statusName}`

### Archivos Modificados:
1. `lib/api-client.ts` - Corregidas 3 rutas en métodos:
   - `getMemberStatusTypes()`
   - `getMemberStatusTypesByCategory()`
   - `getMemberStatusCategories()`

2. Documentación actualizada:
   - `README.md` - Endpoints consumidos
   - `QUICKSTART.md` - Comando curl
   - `TECHNICAL_SUMMARY.md` - Tabla de endpoints
   - `DELIVERY_REPORT.md` - Endpoints y reglas de negocio
   - `BACKEND_CORS_CONFIG.md` - Verificación

### Impacto:
- ✅ El frontend ahora puede cargar correctamente los 33 tipos de estado
- ✅ El formulario de evidencia funcionará completamente
- ✅ No se requieren cambios en el backend
- ✅ `uploadEvidence()` no cambió (ya estaba correcto)

### Testing:
Después de este fix, el frontend debe:
1. Cargar exitosamente los tipos de estado al abrir `/evidence/upload`
2. Mostrar mensaje "33 tipos de estado cargados desde el backend"
3. No mostrar errores 404 en consola del navegador

---

## [1.0.0] - 2026-01-15

### ✨ Initial Release

**Sprint 1**: Sistema de Subida de Evidencias

#### Features:
- ✅ Página `/evidence/upload` con formulario completo
- ✅ Integración con backend .NET 8
- ✅ Upload multipart/form-data con 2 fotos
- ✅ Manejo de errores 400/500
- ✅ Muestra puntos ganados inmediatamente
- ✅ TypeScript estricto + Tailwind CSS
- ✅ Documentación completa

#### Endpoints Implementados:
- GET `/api/MemberStatusTypes`
- GET `/api/MemberStatusTypes/by-category/{category}`
- GET `/api/MemberStatusTypes/categories`
- POST `/api/admin/evidence/upload`

#### Stack:
- Next.js 14.2.0
- React 18.3.0
- TypeScript 5.4.5
- Tailwind CSS 3.4.3

---

## Formato de Versiones

Este proyecto sigue [Semantic Versioning](https://semver.org/):
- **MAJOR**: Cambios incompatibles en API
- **MINOR**: Nueva funcionalidad (backward compatible)
- **PATCH**: Bugfixes (backward compatible)

## Categorías de Cambios

- ✨ **Features**: Nueva funcionalidad
- 🐛 **Bugfix**: Corrección de errores
- 📝 **Docs**: Cambios en documentación
- 🎨 **Style**: Cambios de formato/estilo
- ♻️ **Refactor**: Refactorización de código
- ⚡ **Perf**: Mejoras de performance
- 🔒 **Security**: Parches de seguridad
