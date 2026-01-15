# L.A.M.A. Mototurismo - Frontend Modernizado v2.0

Renovación completa del frontend de L.A.M.A. Mototurismo con arquitectura limpia, design system profesional y experiencia de usuario mejorada.

## 📋 Contenido

- [Características](#características)
- [Arquitectura](#arquitectura)
- [Stack Tecnológico](#stack-tecnológico)
- [Instalación](#instalación)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [Guía de Componentes](#guía-de-componentes)
- [Desarrollo](#desarrollo)
- [Deployment](#deployment)

- **Framework**: Next.js 14 (App Router)
- **Lenguaje**: TypeScript 5.4+
- **Estilos**: Tailwind CSS 3.4+
- **HTTP Client**: Fetch API nativo
- **Backend**: .NET 8 API (puerto 5000)

## 📋 Prerequisitos

- Node.js 20.x o superior
- npm 9.x o superior
- Backend .NET 8 corriendo en `http://localhost:5000`

## 🛠️ Instalación

1. **Navegar al directorio del proyecto**
   ```bash
   cd src/Lama.Web
   ```

2. **Instalar dependencias**
   ```bash
   npm install
   ```

3. **Configurar variables de entorno**
   ```bash
   # Copiar archivo de ejemplo
   copy .env.local.example .env.local
   
   # Editar .env.local y configurar:
   NEXT_PUBLIC_API_BASE_URL=http://localhost:5000
   ```

4. **Iniciar servidor de desarrollo**
   ```bash
   npm run dev
   ```

5. **Abrir en navegador**
   ```
   http://localhost:3000
   ```

## 📁 Estructura del Proyecto

```
src/Lama.Web/
├── app/                          # App Router (Next.js 14)
│   ├── layout.tsx                # Layout principal con navegación
│   ├── page.tsx                  # Página de inicio
│   ├── globals.css               # Estilos globales + Tailwind
│   └── evidence/
│       └── upload/
│           └── page.tsx          # Página de subida de evidencia
├── components/
│   └── api-client.ts             # Cliente API centralizado
├── types/
│   └── api.ts                    # Tipos TypeScript del backend
├── package.json
├── tailwind.config.ts
├── next.config.js
## 🎯 Funcionalidades Implementadas (Sprint 1)


Formulario completo para subir evidencias con:
#### Campos del Formulario:
- **Event ID** (number, required): ID del evento
- **Member ID** (number, required): ID del miembro
- **Vehicle ID** (number, required): ID del vehículo
- **Tipo de Evidencia** (dropdown):
  - `START_YEAR` - Inicio de año
  - `CUTOFF` - Corte
- **Foto 1: Piloto con Moto** (file, required): Foto obligatoria
- **Foto 2: Odómetro Close-up** (file, required): Foto obligatoria
- **Lectura del Odómetro** (number, required): Valor > 0
- **Unidad** (dropdown):
  - `Kilometers` - Kilómetros
  - `Miles` - Millas
- **Fecha de Lectura** (date, optional): Fecha opcional
- **Notas Adicionales** (textarea, optional): Texto libre

- ✅ Muestra puntos ganados inmediatamente
- ✅ Limpieza automática del formulario tras éxito
- ✅ Loading states durante submission
- ✅ Tipos TypeScript estrictos

### ✅ Integración con Backend

#### GET `/api/MemberStatusTypes`
- Carga 33 tipos de estado desde el backend
- Muestra información al usuario
- Manejo de errores con retry

#### POST `/api/admin/evidence/upload`

## 🔌 Endpoints del Backend Consumidos

### 1. Member Status Types

```typescript
GET /api/MemberStatusTypes
Response: MemberStatusType[]

GET /api/MemberStatusTypes/by-category/{category}
Response: MemberStatusType[]

GET /api/MemberStatusTypes/categories
Response: MemberStatusType
```

### 2. Evidence Upload

```typescript
POST /api/admin/evidence/upload?eventId={id}
Content-Type: multipart/form-data

FormData:
  - memberId: number
  - vehicleId: number
  - evidenceType: 'START_YEAR' | 'CUTOFF'
  - pilotWithBikePhoto: File
  - odometerCloseupPhoto: File
  - message: string
  - pointsAwarded: number
  - pointsPerEvent: number
  - pointsPerDistance: number
  - visitorClass: string
  - memberId: number
  - vehicleId: number
  - attendanceId: number
  - evidenceType: string
```

```bash
# Desarrollo
# Producción
npm run build        # Construye para producción
npm run start        # Inicia servidor de producción

- **Accesibilidad**: Labels asociados, atributos ARIA
- **Feedback Visual**:
  - Validación en tiempo real
- **Colores**: Paleta primary basada en azul (personalizable en `tailwind.config.ts`)

## 🔒 Reglas de Negocio Respetadas

1. ✅ **STATUS es dropdown**: 33 valores desde API (no hardcoded)
2. ✅ **2 fotos obligatorias**: pilotWithBikePhoto + odometerCloseupPhoto
3. ✅ **Validación estricta**: EventId, MemberId, VehicleId > 0
4. ✅ **Tipos correctos**: evidenceType (START_YEAR/CUTOFF), unit (Miles/Kilometers)
5. ✅ **No invención de endpoints**: Solo consume los existentes del backend

## 🐛 Manejo de Errores

### Client-Side
- Validación de campos requeridos
- Validación de tipos numéricos > 0
- Validación de archivos seleccionados

### Server-Side
- **400 Bad Request**: Muestra mensaje específico del backend
- **404 Not Found**: Miembro, vehículo o evento no encontrado
- **500 Internal Server Error**: Error genérico con detalles
- **Network Errors**: Timeout o backend offline

## 🚀 Próximos Sprints (Roadmap)

### Sprint 2: Gestión de Miembros
- [ ] Página `/members` con listado
- [ ] Filtrado por STATUS (dropdown desde API)
- [ ] Búsqueda por nombre
- [ ] Integración con endpoint GET `/api/members`

### Sprint 3: Gestión de Eventos
- [ ] Página `/events` con calendario
- [ ] Crear nuevo evento
- [ ] Ver asistencias por evento
- [ ] Integración con endpoints de eventos

### Sprint 4: Dashboard
- [ ] Página `/dashboard` con estadísticas
- [ ] Gráficos de puntos por miembro
- [ ] Ranking de campeonato
- [ ] Métricas de asistencia

## 📝 Notas de Desarrollo

### API Client (`lib/api-client.ts`)
- Singleton instance exportado
- Manejo centralizado de errores
- URLs construidas dinámicamente desde env vars
- Tipado estricto con TypeScript

### Componentes Reutilizables
- `EvidenceUploadForm`: Componente standalone, puede ser reutilizado
- Separación de concerns: lógica en client component, layout en server component

### TypeScript
- `strict: true` en tsconfig.json
- Interfaces para todas las respuestas del backend
- No uso de `any` en el código

## 🤝 Contribución

1. Respetar Clean Architecture
2. Usar TypeScript estricto (no `any`)
3. No hardcodear valores (usar API)
4. Seguir convenciones de Next.js 14 App Router
5. Tailwind CSS para estilos (no CSS-in-JS)

## 📞 Soporte

Para problemas con:
- **Frontend**: Revisar console del navegador (F12)
- **Backend**: Verificar que .NET API esté corriendo en puerto 5000
- **CORS**: Configurar CORS en backend para permitir `http://localhost:3000`

## 📄 Licencia

Propiedad de LAMA Mototurismo. Todos los derechos reservados.
