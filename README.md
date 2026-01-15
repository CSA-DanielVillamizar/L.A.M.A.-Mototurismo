# LAMA MOTOTURISMO - PLATAFORMA SAAS  
## Instrucciones de Instalación y Ejecución

---

## **CONTENIDO DE LA SOLUCIÓN**

```
COR L.A.MA/
├── Lama.sln                    # Solución Visual Studio
├── src/
│   ├── Lama.Domain/            # Domain Layer (Entities, Enums)
│   ├── Lama.Application/       # Application Layer (Interfaces, Services, DTOs)
│   ├── Lama.Infrastructure/    # Infrastructure (DbContext, Repos, Implementations)
│   └── Lama.API/               # API Layer (Controllers, Program.cs)
├── tests/
│   └── Lama.UnitTests/         # Unit Tests (xUnit, Moq)
├── sql/
│   ├── schema.sql              # Tablas, triggers, constraints
│   └── views.sql               # Vistas de reporte (vw_MasterOdometerReport, vw_MemberGeneralRanking)
├── python/
│   └── migration_generator.py  # Script ETL para migrar datos desde Excel
└── INSUMOS/
    └── (COL) INDIVIDUAL REPORT - REGION NORTE.xlsm  # Datos fuente
```

---

## **REQUISITOS PREVIOS**

- **Visual Studio 2022** (v17.8+) o **VS Code** + .NET CLI
- **.NET 8 SDK** (https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- **SQL Server 2022** o **Azure SQL Database**
  - Para desarrollo local: **SQL Server Express** o usar **(localdb)\mssqllocaldb**
- **Python 3.9+** (para script de migración)
  - Librerías: `pandas`, `openpyxl`

---

## **PASO 1: PREPARAR BASE DE DATOS**

### 1.1 Crear la base de datos

```bash
# Abre SQL Server Management Studio (SSMS) o Azure Portal

# En Query Window, ejecuta:
CREATE DATABASE LamaDb;
GO
```

### 1.2 Aplicar esquema SQL

Ejecuta los scripts en orden:

```bash
# 1. Crear tablas y triggers
sqlcmd -S (localdb)\mssqllocaldb -d LamaDb -i sql\schema.sql

# 2. Crear vistas
sqlcmd -S (localdb)\mssqllocaldb -d LamaDb -i sql\views.sql
```

**Nota**: Si usas **Azure SQL**, reemplaza la conexión:
```bash
sqlcmd -S <servidor>.database.windows.net -U <usuario> -P <contraseña> -d LamaDb -i sql\schema.sql
```

---

## **PASO 2: CONFIGURAR CONEXIÓN A BASE DE DATOS**

Edita `src\Lama.API\appsettings.json`:

```json
{
  "ConnectionStrings": {
    "LamaDb": "Server=(localdb)\\mssqllocaldb;Database=LamaDb;Trusted_Connection=true;"
  }
}
```

**Para Azure SQL:**
```json
{
  "ConnectionStrings": {
    "LamaDb": "Server=<servidor>.database.windows.net;Database=LamaDb;User Id=<usuario>;Password=<contraseña>;"
  }
}
```

---

## **PASO 3: MIGRACIÓN DE DATOS (OPCIONAL)**

Si tienes datos en Excel para migrar:

### 3.1 Instalar dependencias Python

```bash
pip install pandas openpyxl
```

### 3.2 Ejecutar script de migración

```bash
cd python
python migration_generator.py
```

Esto genera `migration_script.sql`. Aplícalo:

```bash
sqlcmd -S (localdb)\mssqllocaldb -d LamaDb -i python\migration_script.sql
```

---

## **PASO 4: COMPILAR Y EJECUTAR**

### 4.1 Restaurar dependencias

```bash
# Desde la raíz del proyecto
dotnet restore
```

### 4.2 Compilar solución

```bash
dotnet build
```

### 4.3 Ejecutar API

```bash
cd src\Lama.API
dotnet run
```

Deberías ver:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7001
```

Abre el navegador en: **https://localhost:7001/swagger**

---

## **PASO 5: EJECUTAR UNIT TESTS**

```bash
# Desde la raíz del proyecto
dotnet test tests\Lama.UnitTests\
```

Deberías ver algo como:
```
Test Run Successful.
Total tests: 15
Passed: 15
Duration: ~500 ms
```

---

## **PASO 6: PROBAR ENDPOINT DE CONFIRMACIÓN DE ASISTENCIA**

### 6.1 Preparar datos de prueba

Inserta datos en la BD para testing:

```sql
-- Insertar capítulo
INSERT INTO [dbo].[Chapters] ([Name], [Country], [IsActive])
VALUES ('Capítulo Pereira', 'Colombia', 1);

-- Insertar miembro
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [Complete Names], [Dama], [Country Birth], [STATUS], [is_eligible])
VALUES 
    (1, 1, 'Juan Pérez', 0, 'Colombia', 'ACTIVE', 1);

-- Insertar vehículo
INSERT INTO [dbo].[Vehicles]
    ([MemberId], [Motorcycle Data], [Lic Plate], [OdometerUnit], [IsActiveForChampionship])
VALUES
    (1, 'Honda CB500 2022', 'ABC-123', 'Miles', 1);

-- Insertar evento
INSERT INTO [dbo].[Events]
    ([ChapterId], [Order], [Event Start Date (AAAA/MM/DD)], [Name of the event ], [Class], [Mileage])
VALUES
    (1, 1, '2026-02-14', 'Rally Pereira - Bogotá', 3, 450.0);

-- Insertar asistencia (PENDING)
INSERT INTO [dbo].[Attendance]
    ([EventId], [MemberId], [VehicleId], [Status])
VALUES
    (1, 1, 1, 'PENDING');
```

### 6.2 Crear archivos de prueba (imágenes)

Necesitas 2 archivos JPG para testing:
- `test_pilot_bike.jpg` (foto de piloto con moto)
- `test_odometer.jpg` (foto de odómetro)

### 6.3 Ejecutar request con curl

```bash
# Windows PowerShell
$eventId = 1
$pilotPhoto = "test_pilot_bike.jpg"
$odometerPhoto = "test_odometer.jpg"

curl -X POST "https://localhost:7001/api/admin/evidence/upload?eventId=$eventId" `
  -F "memberId=1" `
  -F "vehicleId=1" `
  -F "evidenceType=START_YEAR" `
  -F "pilotWithBikePhoto=@$pilotPhoto" `
  -F "odometerCloseupPhoto=@$odometerPhoto" `
  -F "odometerReading=25000.5" `
  -F "unit=Miles" `
  -F "readingDate=2026-02-14" `
  --insecure
```

**Respuesta esperada (200 OK):**
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

## **PASO 7: VERIFICAR DATOS EN BD**

Después de confirmar asistencia:

```sql
-- Ver asistencia actualizada
SELECT [Id], [MemberId], [Status], [Points per event], [Points per Distance], 
       [Points awarded per member], [Visitor Class], [ConfirmedAt]
FROM [dbo].[Attendance]
WHERE [Id] = 1;

-- Ver vehículo actualizado con evidencia
SELECT [Id], [Lic Plate], [Starting Odometer], [OdometerUnit], 
       [StartYearEvidenceUrl], [Photography]
FROM [dbo].[Vehicles]
WHERE [Id] = 1;

-- Ver ranking general de miembros
SELECT * FROM [dbo].[vw_MemberGeneralRanking];
```

---

## **PASO 8: SOLUCIÓN DE PROBLEMAS**

### Error: "Database LamaDb does not exist"
```bash
# Crear BD manualmente
sqlcmd -S (localdb)\mssqllocaldb
> CREATE DATABASE LamaDb;
> GO
> EXIT
```

### Error: "Connection Timeout"
- Verificar que SQL Server está corriendo
- Para (localdb): `sqllocaldb start mssqllocaldb`

### Error: "El usuario no tiene permiso para crear tablas"
- Usar conexión con `Trusted_Connection=true` (Windows Auth)
- O asegurarse que el usuario SQL tiene permisos de DBA

### Error: "Columna no encontrada en Excel"
- Verificar que el archivo Excel está en la ruta correcta
- El header debe estar en fila 7 (index 6)

---

## **ARQUITECTURA Y DISEÑO**

### **Clean Architecture**
- **Domain Layer**: Entidades, enums, lógica de negocio pura
- **Application Layer**: Interfaces de servicios, DTOs, cases de uso
- **Infrastructure Layer**: Implementaciones de repos, DbContext, servicios externos
- **API Layer**: Controllers, configuración, Program.cs

### **Flujo de Confirmación de Asistencia**
1. MTO sube 2 fotos + lectura de odómetro vía multipart/form-data
2. `AdminController` valida solicitud
3. `AttendanceConfirmationService` orquesta:
   - Subida de fotos a `IBlobStorageService` (URLs guardadas)
   - Actualización de `Vehicle` con odómetro validado
   - Cálculo de puntos con `IPointsCalculatorService`
   - Actualización de `Attendance` estado CONFIRMED
4. Respuesta con desglose de puntos

### **Cálculo de Puntos**
```
Total Points = PointsPerEvent + PointsPerDistance + VisitorBonus

PointsPerEvent:  Configuración por Clase (1-5)
PointsPerDistance: >200 mi = 1, >800 mi = 2
VisitorBonus: Local = 0, SameContinent = 1, DifferentContinent = 2
```

---

## **CONFIGURACIÓN GLOBAL**

La tabla `Configuration` almacena parámetros ajustables:

```sql
SELECT * FROM [dbo].[Configuration];
```

**Parámetros clave**:
- `DistanceThreshold_1Point_OneWayMiles`: 200
- `DistanceThreshold_2Points_OneWayMiles`: 800
- `PointsPerClassMultiplier_1` a `_5`: 1, 3, 5, 10, 15
- `VisitorBonus_SameContinent`: 1
- `VisitorBonus_DifferentContinent`: 2

---

## **API ENDPOINTS IMPLEMENTADOS**

### **POST /api/admin/evidence/upload**
Confirma asistencia con subida de evidencia fotográfica

**Query Parameters:**
- `eventId` (int, required)

**Form Data:**
- `memberId` (int)
- `vehicleId` (int)
- `evidenceType` (string): START_YEAR | CUTOFF
- `pilotWithBikePhoto` (file)
- `odometerCloseupPhoto` (file)
- `odometerReading` (double)
- `unit` (string): Miles | Kilometers
- `readingDate` (date, optional)
- `notes` (string, optional)

**Response (200):**
```json
{
  "message": "Asistencia confirmada exitosamente. Puntos: X",
  "pointsAwarded": X,
  "pointsPerEvent": X,
  "pointsPerDistance": X,
  "visitorClass": "LOCAL|VISITOR_A|VISITOR_B",
  "memberId": X,
  "vehicleId": X,
  "attendanceId": X,
  "evidenceType": "START_YEAR|CUTOFF"
}
```

---

## **ESTRUCTURA DE DIRECTORIOS FINAL**

```
C:\Users\DanielVillamizar\COR L.A.MA\
├── Lama.sln
├── src/
│   ├── Lama.Domain/
│   │   ├── Entities/
│   │   │   ├── Chapter.cs
│   │   │   ├── Member.cs
│   │   │   ├── Vehicle.cs
│   │   │   ├── Event.cs
│   │   │   ├── Attendance.cs
│   │   │   └── Configuration.cs
│   │   ├── Enums/
│   │   │   ├── OdometerUnit.cs
│   │   │   ├── AttendanceStatus.cs
│   │   │   ├── VisitorClass.cs
│   │   │   ├── EvidenceType.cs
│   │   │   ├── PhotographyStatus.cs
│   │   │   └── MemberStatus.cs
│   │   └── Lama.Domain.csproj
│   ├── Lama.Application/
│   │   ├── Repositories/
│   │   │   ├── IMemberRepository.cs
│   │   │   ├── IVehicleRepository.cs
│   │   │   ├── IEventRepository.cs
│   │   │   └── IAttendanceRepository.cs
│   │   ├── Services/
│   │   │   ├── IAppConfigProvider.cs
│   │   │   ├── IPointsCalculatorService.cs
│   │   │   ├── IBlobStorageService.cs
│   │   │   ├── IAttendanceConfirmationService.cs
│   │   │   └── AttendanceModels.cs
│   │   └── Lama.Application.csproj
│   ├── Lama.Infrastructure/
│   │   ├── Data/
│   │   │   ├── Configurations/
│   │   │   │   ├── ChapterConfiguration.cs
│   │   │   │   ├── MemberConfiguration.cs
│   │   │   │   ├── VehicleConfiguration.cs
│   │   │   │   ├── EventConfiguration.cs
│   │   │   │   ├── AttendanceConfiguration.cs
│   │   │   │   └── ConfigurationConfiguration.cs
│   │   │   └── LamaDbContext.cs
│   │   ├── Repositories/
│   │   │   ├── MemberRepository.cs
│   │   │   ├── VehicleRepository.cs
│   │   │   ├── EventRepository.cs
│   │   │   └── AttendanceRepository.cs
│   │   ├── Services/
│   │   │   ├── AppConfigProvider.cs
│   │   │   ├── PointsCalculatorService.cs
│   │   │   ├── FakeBlobStorageService.cs
│   │   │   └── AttendanceConfirmationService.cs
│   │   └── Lama.Infrastructure.csproj
│   ├── Lama.API/
│   │   ├── Controllers/
│   │   │   └── AdminController.cs
│   │   ├── Extensions/
│   │   │   └── ServiceCollectionExtensions.cs
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   ├── appsettings.Development.json
│   │   └── Lama.API.csproj
├── tests/
│   └── Lama.UnitTests/
│       ├── Services/
│       │   └── PointsCalculatorServiceTests.cs
│       └── Lama.UnitTests.csproj
├── sql/
│   ├── schema.sql
│   └── views.sql
├── python/
│   └── migration_generator.py
└── INSUMOS/
    └── (COL) INDIVIDUAL REPORT - REGION NORTE.xlsm
```

---

## **PRÓXIMOS PASOS (V2)**

- [ ] Autenticación con Azure AD / OAuth2
- [ ] Endpoints para Mobile (tablero, ranking, QR check-in)
- [ ] Endpoints para MTO/Admin (gestión de eventos, validaciones)
- [ ] Azure Blob Storage (reemplazar FakeBlobStorageService)
- [ ] Background jobs para cálculos de puntos masivos
- [ ] Herencia A/B para viajes cercanos (<15 días)
- [ ] Notificaciones por email
- [ ] Dashboard Blazor/React

---

## **CONTACTO Y SOPORTE**

Tech Lead: Daniel Villamizar  
Versión: 1.0 - Enero 2026  
Clean Architecture + .NET 8 + EF Core + SQL Server
