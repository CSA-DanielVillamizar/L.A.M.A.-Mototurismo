# COR API Reference (v1)

**Base URL:** `https://localhost:7001/api/v1`

- **Versionado:** Todos los endpoints comienzan con `/api/v1/...`.
- **Convención JSON:** PascalCase en todas las propiedades (backend y frontend).
- **Errores:** Respuestas estándar RFC 7807 (`application/problem+json`).
- **OpenAPI:** `/swagger/v1/swagger.json` (UI en `/swagger`).

## Errores (ProblemDetails)
```json
{
  "Type": "about:blank",
  "Title": "Invalid search term",
  "Status": 400,
  "Detail": "El término de búsqueda debe tener al menos 2 caracteres",
  "Instance": "/api/v1/members/search"
}
```

---
## 1) Eventos
**GET /api/v1/events?year=2026**
- `year` (int?, query) opcional

Respuesta 200:
```json
[
  {
    "Id": 501,
    "Name": "Continental Ride",
    "Location": "Buenos Aires, AR",
    "EventStartDate": "2026-03-10T09:00:00Z",
    "EventEndDate": "2026-03-12T18:00:00Z",
    "ChapterId": 7
  }
]
```

cURL:
```bash
curl -X GET "https://localhost:7001/api/v1/events?year=2026"
```

---
## 2) Búsqueda de Miembros (Autocomplete)
**GET /api/v1/members/search?q=jose**
- `q` (string, query) mínimo 2 caracteres

Respuesta 200:
```json
[
  {
    "MemberId": 101,
    "FirstName": "Maria",
    "LastName": "Lopez",
    "FullName": "Maria Lopez",
    "Status": "ACTIVE",
    "ChapterId": 12
  }
]
```

cURL:
```bash
curl -X GET "https://localhost:7001/api/v1/members/search?q=jose"
```

---
## 3) Solicitar SAS para subir evidencias
**POST /api/v1/evidence/upload-request**

Body `application/json`:
```json
{
  "MemberId": 101,
  "VehicleId": 3001,
  "EventId": 501,
  "EvidenceType": "START_YEAR",
  "PilotPhotoContentType": "image/jpeg",
  "OdometerPhotoContentType": "image/jpeg"
}
```

Respuesta 200:
```json
{
  "CorrelationId": "7f5caa6c2c324c1b8c4c9d1d0a123456",
  "PilotPhotoSasUrl": "https://storage.blob.core.windows.net/evidence/pilot.jpg?...",
  "OdometerPhotoSasUrl": "https://storage.blob.core.windows.net/evidence/odometer.jpg?...",
  "PilotPhotoBlobPath": "tenants/1/evidence/pilot.jpg",
  "OdometerPhotoBlobPath": "tenants/1/evidence/odometer.jpg",
  "ExpiresAt": "2026-01-15T23:59:59Z"
}
```

cURL:
```bash
curl -X POST "https://localhost:7001/api/v1/evidence/upload-request" \
  -H "Content-Type: application/json" \
  -d '{
    "MemberId": 101,
    "VehicleId": 3001,
    "EventId": 501,
    "EvidenceType": "START_YEAR",
    "PilotPhotoContentType": "image/jpeg",
    "OdometerPhotoContentType": "image/jpeg"
  }'
```

---
## 4) Registrar metadata de evidencia
**POST /api/v1/evidence/submit**

Body `application/json`:
```json
{
  "CorrelationId": "7f5caa6c2c324c1b8c4c9d1d0a123456",
  "MemberId": 101,
  "VehicleId": 3001,
  "EventId": 501,
  "EvidenceType": "START_YEAR",
  "PilotPhotoBlobPath": "tenants/1/evidence/pilot.jpg",
  "OdometerPhotoBlobPath": "tenants/1/evidence/odometer.jpg",
  "OdometerReading": 10500,
  "OdometerUnit": "Miles"
}
```

Respuesta 201:
```json
{
  "EvidenceId": 9001,
  "AttendanceId": null,
  "Status": "PENDING_REVIEW"
}
```

cURL:
```bash
curl -X POST "https://localhost:7001/api/v1/evidence/submit" \
  -H "Content-Type: application/json" \
  -d '{
    "CorrelationId": "7f5caa6c2c324c1b8c4c9d1d0a123456",
    "MemberId": 101,
    "VehicleId": 3001,
    "EventId": 501,
    "EvidenceType": "START_YEAR",
    "PilotPhotoBlobPath": "tenants/1/evidence/pilot.jpg",
    "OdometerPhotoBlobPath": "tenants/1/evidence/odometer.jpg",
    "OdometerReading": 10500,
    "OdometerUnit": "Miles"
  }'
```

---
## 5) Admin: upload multipart + confirm asistencia
**POST /api/v1/admin/evidence/upload?eventId=501**

`multipart/form-data`:
- memberId (int)
- vehicleId (int)
- evidenceType (START_YEAR | CUTOFF)
- pilotWithBikePhoto (file)
- odometerCloseupPhoto (file)
- odometerReading (number)
- unit (Miles | Kilometers)
- readingDate (yyyy-MM-dd, opcional)
- notes (string, opcional)

cURL:
```bash
curl -X POST "https://localhost:7001/api/v1/admin/evidence/upload?eventId=501" \
  -F "memberId=101" \
  -F "vehicleId=3001" \
  -F "evidenceType=START_YEAR" \
  -F "pilotWithBikePhoto=@pilot.jpg" \
  -F "odometerCloseupPhoto=@odo.jpg" \
  -F "odometerReading=10500" \
  -F "unit=Miles"
```

---
## 6) Rankings (snapshot)
**GET /api/v1/rankings?year=2026&scopeType=GLOBAL&skip=0&take=50**

Respuesta 200 (extracto):
```json
{
  "Total": 2,
  "Items": [
    { "MemberId": 101, "CompleteNames": "Maria Lopez", "Points": 120 },
    { "MemberId": 102, "CompleteNames": "Jose Perez", "Points": 115 }
  ]
}
```

---
## Autenticación
- Producción: JWT Bearer (Entra ID B2C) requerido en endpoints protegidos.
- Desarrollo: algunos endpoints administrativos pueden permitir bypass (DEBUG).

---
## Notas APIM-ready
- Versionado obligatorio `/api/v1`.
- Respuestas de error usan ProblemDetails (RFC 7807).
- Swagger con ejemplos enriquecidos (multipart, SAS, búsqueda).
- Contrato JSON en PascalCase para alinear backend y frontend.

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
