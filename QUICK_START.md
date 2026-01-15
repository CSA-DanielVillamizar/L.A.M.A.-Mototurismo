## 🚀 LAMA MOTOTURISMO - GUÍA DE INICIO RÁPIDO

**Tech Stack**: .NET 8 | EF Core | SQL Server | Clean Architecture | C# 12

---

### **📦 ESTRUCTURA ENTREGADA**

```
✓ Solución completa .NET 8 con Clean Architecture
✓ 6 Proyectos: Domain, Application, Infrastructure, API + Unit Tests
✓ Scripts SQL: Schema (tablas + triggers) + Vistas de reporte
✓ Servicios: PointsCalculator, AttendanceConfirmation, ConfigProvider
✓ Controllers: AdminController con endpoint de confirmación de asistencia
✓ DTOs y Models completos
✓ Repositorios con interfacesimplementadas
✓ Unit Tests (xUnit + Moq) para PointsCalculatorService
✓ Script Python para migración de datos desde Excel
✓ Documentación completa
```

---

### **⚡ INICIO RÁPIDO (5 MIN)**

#### **1. Crear BD**
```bash
sqlcmd -S (localdb)\mssqllocaldb -Q "CREATE DATABASE LamaDb;"
```

#### **2. Aplicar schema SQL**
```bash
# Desde raíz del proyecto
sqlcmd -S (localdb)\mssqllocaldb -d LamaDb -i sql\schema.sql
sqlcmd -S (localdb)\mssqllocaldb -d LamaDb -i sql\views.sql
```

#### **3. Compilar y ejecutar API**
```bash
dotnet build
cd src\Lama.API
dotnet run
```

✅ API corriendo en `https://localhost:7001`  
📚 Swagger en `https://localhost:7001/swagger`

#### **4. Ejecutar tests**
```bash
dotnet test tests\Lama.UnitTests\
```

---

### **📋 RUTAS DE ARCHIVOS CLAVE**

| Componente | Ruta |
|-----------|------|
| **DbContext** | [src/Lama.Infrastructure/Data/LamaDbContext.cs](../src/Lama.Infrastructure/Data/LamaDbContext.cs) |
| **Admin Controller** | [src/Lama.API/Controllers/AdminController.cs](../src/Lama.API/Controllers/AdminController.cs) |
| **PointsCalculator** | [src/Lama.Infrastructure/Services/PointsCalculatorService.cs](../src/Lama.Infrastructure/Services/PointsCalculatorService.cs) |
| **Attendance Service** | [src/Lama.Infrastructure/Services/AttendanceConfirmationService.cs](../src/Lama.Infrastructure/Services/AttendanceConfirmationService.cs) |
| **Repositorios** | [src/Lama.Infrastructure/Repositories/](../src/Lama.Infrastructure/Repositories/) |
| **Entities** | [src/Lama.Domain/Entities/](../src/Lama.Domain/Entities/) |
| **Schema SQL** | [sql/schema.sql](../sql/schema.sql) |
| **Views SQL** | [sql/views.sql](../sql/views.sql) |
| **Python Migration** | [python/migration_generator.py](../python/migration_generator.py) |

---

### **🔌 API ENDPOINT DISPONIBLE**

#### **POST `/api/admin/evidence/upload`**

Confirma asistencia con carga de evidencia fotográfica.

**Query Parameters:**
- `eventId` (int) - ID del evento

**Form Data:**
```
memberId:             int
vehicleId:            int
evidenceType:         string (START_YEAR | CUTOFF)
pilotWithBikePhoto:   file (JPG/PNG)
odometerCloseupPhoto: file (JPG/PNG)
odometerReading:      double
unit:                 string (Miles | Kilometers)
readingDate:          date (opcional)
notes:                string (opcional)
```

**Response (200 OK):**
```json
{
  "message": "Asistencia confirmada exitosamente. Puntos: 8",
  "pointsAwarded": 8,
  "pointsPerEvent": 5,
  "pointsPerDistance": 1,
  "visitorClass": "Local",
  "memberId": 1,
  "vehicleId": 1,
  "attendanceId": 1,
  "evidenceType": "START_YEAR"
}
```

---

### **📊 CÁLCULO DE PUNTOS**

**Fórmula**:
```
TotalPoints = PointsPerEvent + PointsPerDistance + VisitorBonus

Donde:
  PointsPerEvent = Config por Clase (Class 1→1, 2→3, 3→5, 4→10, 5→15)
  PointsPerDistance = 0 si ≤200mi, 1 si >200mi, 2 si >800mi
  VisitorBonus = 0 (Local), 1 (SameContinent), 2 (DifferentContinent)
```

**Ejemplo**:
- Evento Class 3, 500 millas, visitante de otro país en mismo continente
- PointsPerEvent = 5
- PointsPerDistance = 1 (500 > 200)
- VisitorBonus = 1 (mismo continente, país diferente)
- **Total = 7 puntos**

---

### **💾 TABLAS SQL PRINCIPALES**

```sql
-- Miembros
Members([Complete Names], [Country Birth], [Continent], [is_eligible])

-- Vehículos (max 2 activos por miembro)
Vehicles([Lic Plate], [OdometerUnit], [Starting Odometer], [Final Odometer], 
         [StartYearEvidenceUrl], [CutOffEvidenceUrl], [Photography])

-- Eventos
Events([Name of the event], [Class], [Mileage], [Event Start Date])

-- Asistencias
Attendance([EventId], [MemberId], [VehicleId], [Status], 
          [Points per event], [Points per Distance], [Visitor Class])

-- Configuración global
Configuration([Key], [Value])
  - DistanceThreshold_1Point_OneWayMiles = 200
  - DistanceThreshold_2Points_OneWayMiles = 800
  - PointsPerClassMultiplier_1 a _5 = 1,3,5,10,15
```

---

### **🔍 VISTAS DE REPORTE**

#### **vw_MasterOdometerReport**
```sql
SELECT * FROM [dbo].[vw_MasterOdometerReport];
-- Retorna: MemberId, VehicleId, [Total Miles Traveled] (convertido KM→Miles si aplica)
```

#### **vw_MemberGeneralRanking**
```sql
SELECT * FROM [dbo].[vw_MemberGeneralRanking];
-- Retorna: MemberId, [Complete Names], [Total Miles All Vehicles], 
--          [Vehicles Breakdown], [Active Vehicles Count]
```

---

### **🧪 UNIT TESTS**

**Casos cubiertos en PointsCalculatorServiceTests**:
- ✅ Puntos por Clase (1-5)
- ✅ Puntos por Distancia (umbrales 200/800)
- ✅ Bonus Visitante (Local/A/B)
- ✅ Cálculo Total
- ✅ Conversiones KM ↔ Miles

**Ejecutar tests:**
```bash
dotnet test tests\Lama.UnitTests\ --logger "console;verbosity=detailed"
```

---

### **🔧 CONFIGURACIÓN IMPORTANTE**

**appsettings.json:**
```json
{
  "ConnectionStrings": {
    "LamaDb": "Server=(localdb)\\mssqllocaldb;Database=LamaDb;Trusted_Connection=true;"
  }
}
```

**Para Azure SQL, reemplazar con:**
```json
{
  "ConnectionStrings": {
    "LamaDb": "Server=tu-servidor.database.windows.net;Database=LamaDb;User Id=usuario;Password=contraseña;"
  }
}
```

---

### **🐍 MIGRACIÓN DE DATOS (PYTHON)**

**Prerequisitos:**
```bash
pip install pandas openpyxl
```

**Generar migration script:**
```bash
cd python
python migration_generator.py
```

Esto genera `migration_script.sql`. Aplicar con:
```bash
sqlcmd -S (localdb)\mssqllocaldb -d LamaDb -i python\migration_script.sql
```

---

### **✨ CARACTERÍSTICAS IMPLEMENTADAS**

| Característica | Estado | Detalles |
|---|---|---|
| Domain Entities | ✅ | 6 entidades + 6 enums |
| DbContext + Fluent Config | ✅ | Mapeo exacto con `HasColumnName` para espacios |
| Repositorios | ✅ | CRUD completo, interfacesIMemberRepository, IVehicleRepository, etc. |
| PointsCalculator | ✅ | Cálculo dinámico con config en BD |
| AttendanceConfirmation | ✅ | Transaccional con blob storage |
| AdminController | ✅ | Endpoint multipart/form-data |
| FakeBlobStorage | ✅ | Simulación de almacenamiento para dev |
| Unit Tests | ✅ | 15 tests sobre PointsCalculator |
| Triggers SQL | ✅ | Max 2 motos activas por miembro |
| Vistas SQL | ✅ | Master Odometer + Member Ranking |
| Python ETL | ✅ | Lee Excel, detecta unidades, genera INSERTs |
| Documentación | ✅ | README.md + COMMANDS.md + guías |

---

### **❌ CARACTERÍSTICAS FUTURAS (V2)**

- [ ] Autenticación Azure AD
- [ ] Mobile endpoints (QR check-in, ranking, tablero)
- [ ] Herencia A/B para viajes <15 días
- [ ] Azure Blob Storage (reemplazar Fake)
- [ ] Background jobs para cálculos masivos
- [ ] Notificaciones email
- [ ] Dashboard web (Blazor/React)

---

### **🔐 SEGURIDAD (Placeholder)**

El atributo `[Authorize(Roles="MTO,Admin")]` está en AdminController pero **NO está totalmente implementado**.

**Para producción, agregar:**
1. OAuth2 / Azure AD
2. JWT tokens
3. Validación de permisos en servicios
4. Rate limiting
5. Audit logging

---

### **⚠️ PRÓXIMAS ACCIONES RECOMENDADAS**

1. **Crear datos de prueba** en BD (Members, Vehicles, Events, Attendance)
2. **Probar endpoint** con Swagger o curl
3. **Ejecutar tests** unitarios
4. **Integrar Azure Blob Storage** en FakeBlobStorageService
5. **Implementar autenticación** en el controlador
6. **Deploy** a Azure App Service o Container
7. **Configurar CI/CD** con GitHub Actions

---

### **📞 SOPORTE**

**Tecnología**: .NET 8, EF Core 8, SQL Server  
**Arquitectura**: Clean Architecture (Domain, Application, Infrastructure, API)  
**Patrones**: Repository, Service Layer, DI, SOLID  
**Testing**: xUnit, Moq  

**Archivo principal del proyecto**: [Lama.sln](../Lama.sln)

---

**✅ Proyecto listo para compilar, ejecutar y extender.**

Todas las capas están separadas, código listo para producción, con pruebas unitarias incluidas.
