# COR API Reference
## Endpoints Administrativos para Sistema de Confirmación de Asistencia

Base URL: `http://localhost:5000` (Development)

---

## 📅 Eventos

### GET /api/events
**Descripción:** Obtiene todos los eventos, opcionalmente filtrado por año

**Query Parameters:**
- `year?` (int) - Año para filtrar eventos (opcional)

**Response:** `200 OK`
```json
[
  {
    "eventId": 1,
    "eventName": "Viaje Naturaleza - Región Cafetera",
    "eventDate": "2025-03-15",
    "chapterId": 5,
    "eventType": "3"
  }
]
```

**Ejemplos:**
```
GET /api/events
GET /api/events?year=2025
```

---

## 👥 Admin Miembros

### GET /api/admin/members/search
**Descripción:** Busca miembros por nombre, número de orden o placa de vehículo

**Query Parameters:**
- `q` (string, required) - Término de búsqueda (mínimo 1 carácter)

**Response:** `200 OK`
```json
[
  {
    "memberId": 42,
    "firstName": "Juan",
    "lastName": "Pérez",
    "fullName": "Juan Pérez",
    "status": "MEMBER",
    "chapterId": 5,
    "order": 42
  }
]
```

**Ejemplos:**
```
GET /api/admin/members/search?q=Juan
GET /api/admin/members/search?q=42        (búsqueda por orden)
GET /api/admin/members/search?q=ABC-1234  (búsqueda por placa)
```

**Error Responses:**
- `400 Bad Request` - Término de búsqueda vacío o muy corto

---

### GET /api/admin/members/{memberId}/vehicles
**Descripción:** Obtiene todos los vehículos de un miembro específico

**Path Parameters:**
- `memberId` (int) - ID del miembro

**Response:** `200 OK`
```json
[
  {
    "vehicleId": 7,
    "memberId": 42,
    "licPlate": "ABC-1234",
    "motorcycleData": "Harley-Davidson Sportster 1200 Negro",
    "trike": false,
    "displayName": "Harley-Davidson Sportster 1200 Negro - ABC-1234"
  },
  {
    "vehicleId": 8,
    "memberId": 42,
    "licPlate": "XYZ-5678",
    "motorcycleData": "Honda CB500F Gris",
    "trike": false,
    "displayName": "Honda CB500F Gris - XYZ-5678"
  }
]
```

**Error Responses:**
- `404 Not Found` - Miembro no existe

---

### GET /api/admin/members/{memberId}
**Descripción:** Obtiene detalles de un miembro específico

**Path Parameters:**
- `memberId` (int) - ID del miembro

**Response:** `200 OK`
```json
{
  "memberId": 42,
  "firstName": "Juan",
  "lastName": "Pérez",
  "fullName": "Juan Pérez",
  "status": "MEMBER",
  "chapterId": 5,
  "order": 42
}
```

**Error Responses:**
- `404 Not Found` - Miembro no existe

---

## 📋 Admin Eventos

### GET /api/admin/event/{eventId}/attendees
**Descripción:** Obtiene la lista de asistentes a un evento con estado específico

**Path Parameters:**
- `eventId` (int) - ID del evento

**Query Parameters:**
- `status?` (string) - Filtrar por estado: `PENDING`, `CONFIRMED`, `REJECTED` (opcional)

**Response:** `200 OK`
```json
[
  {
    "attendanceId": 101,
    "memberId": 42,
    "completeNames": "Juan Pérez",
    "order": 42,
    "vehicleId": 7,
    "licPlate": "ABC-1234",
    "motorcycleData": "Harley-Davidson Sportster 1200 Negro",
    "status": "PENDING",
    "confirmedAt": null
  },
  {
    "attendanceId": 102,
    "memberId": 51,
    "completeNames": "María García",
    "order": 51,
    "vehicleId": 10,
    "licPlate": "DEF-5678",
    "motorcycleData": "Yamaha YZF-R1 Roja",
    "status": "CONFIRMED",
    "confirmedAt": "2025-03-15T10:30:00"
  }
]
```

**Ejemplos:**
```
GET /api/admin/event/1/attendees              (todos los estados)
GET /api/admin/event/1/attendees?status=PENDING    (solo pendientes)
GET /api/admin/event/1/attendees?status=CONFIRMED  (solo confirmados)
```

**Error Responses:**
- `404 Not Found` - Evento no existe

---

## 📸 Admin Evidencia

### POST /api/admin/evidence/upload
**Descripción:** Sube evidencia fotográfica y confirma la asistencia de un miembro

**Query Parameters:**
- `eventId` (int) - ID del evento

**Form Data (multipart/form-data):**
- `memberId` (int) - ID del miembro
- `vehicleId` (int) - ID del vehículo
- `evidenceType` (string) - Tipo: `START_YEAR` o `CUTOFF`
- `pilotWithBikePhoto` (File) - Foto: Piloto con moto
- `odometerCloseupPhoto` (File) - Foto: Close-up odómetro
- `odometerReading` (double) - Lectura del odómetro
- `unit` (string) - Unidad: `Miles` o `Kilometers`
- `readingDate?` (DateOnly) - Fecha (YYYY-MM-DD, opcional)
- `notes?` (string) - Notas adicionales (opcional)

**Response:** `200 OK`
```json
{
  "message": "Asistencia confirmada exitosamente",
  "pointsAwarded": 45,
  "pointsPerEvent": 30,
  "pointsPerDistance": 15,
  "visitorClass": "VISITOR_CLASS_1",
  "memberId": 42,
  "vehicleId": 7,
  "attendanceId": 101,
  "evidenceType": "START_YEAR"
}
```

**cURL Ejemplo:**
```bash
curl -X POST "http://localhost:5000/api/admin/evidence/upload?eventId=1" \
  -F "memberId=42" \
  -F "vehicleId=7" \
  -F "evidenceType=START_YEAR" \
  -F "pilotWithBikePhoto=@photo1.jpg" \
  -F "odometerCloseupPhoto=@photo2.jpg" \
  -F "odometerReading=50250.5" \
  -F "unit=Miles" \
  -F "notes=Viaje exitoso"
```

**Error Responses:**
- `400 Bad Request` - Falta datos requeridos
- `404 Not Found` - Evento, miembro o vehículo no encontrado
- `500 Internal Server Error` - Error al procesar

---

## 🔐 Autenticación

### Development (DEBUG mode)
```
✅ Sin autenticación requerida para endpoints /api/admin/*
```

### Production
```
🔒 Requiere Header:
Authorization: Bearer <JWT_TOKEN>

Roles permitidos:
- MTO
- Admin
```

---

## 📊 Estados de Asistencia

| Estado | Descripción |
|--------|-------------|
| `PENDING` | Pendiente de validación |
| `CONFIRMED` | Validado y confirmado |
| `REJECTED` | Rechazado |

---

## 🎯 Tipos de Evidencia

| Tipo | Descripción |
|------|-------------|
| `START_YEAR` | Inicio de año (enero/febrero) |
| `CUTOFF` | Corte de año (diciembre/noviembre) |

---

## 📱 Integración Frontend

Ver [README.md](./README.md) para documentación completa de componentes y páginas.

**Métodos del cliente API:**
```typescript
// Cliente API (lib/api-client.ts)
apiClient.getEventsByYear(2025)
apiClient.adminSearchMembers('Juan')
apiClient.adminGetMemberVehicles(42)
apiClient.getEventAttendees(1, 'PENDING')
apiClient.uploadEvidence(request)
```

---

## 🧪 Testing

### Postman Collection Variables
```
{{BASE_URL}} = http://localhost:5000
{{EVENT_ID}} = 1
{{MEMBER_ID}} = 42
{{VEHICLE_ID}} = 7
```

---

*Última actualización: 15 Enero 2026*  
*Versión API: 1.2.0*
