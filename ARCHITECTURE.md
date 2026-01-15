# ARQUITECTURA - LAMA MOTOTURISMO

## **ARQUITECTURA GENERAL**

```
┌─────────────────────────────────────────────────────────────────────┐
│                         PRESENTATION LAYER                           │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │  Lama.API (Web API - ASP.NET Core 8)                        │   │
│  │  ├─ Controllers/AdminController.cs                          │   │
│  │  │  └─ POST /api/admin/evidence/upload [multipart/form]    │   │
│  │  ├─ Extensions/ServiceCollectionExtensions.cs              │   │
│  │  ├─ Program.cs                                              │   │
│  │  └─ appsettings.json                                        │   │
│  └─────────────────────────────────────────────────────────────┘   │
└──────────────────┬──────────────────────────────────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────────────────────────────────┐
│                    APPLICATION LAYER                                 │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │  Lama.Application (Interfaces & DTOs)                       │   │
│  │  ├─ Services/                                               │   │
│  │  │  ├─ IAppConfigProvider.cs                               │   │
│  │  │  ├─ IPointsCalculatorService.cs                         │   │
│  │  │  ├─ IBlobStorageService.cs                              │   │
│  │  │  ├─ IAttendanceConfirmationService.cs                   │   │
│  │  │  └─ AttendanceModels.cs (DTOs)                          │   │
│  │  └─ Repositories/                                           │   │
│  │     ├─ IMemberRepository.cs                                │   │
│  │     ├─ IVehicleRepository.cs                               │   │
│  │     ├─ IEventRepository.cs                                 │   │
│  │     └─ IAttendanceRepository.cs                            │   │
│  └─────────────────────────────────────────────────────────────┘   │
└──────────────────┬──────────────────────────────────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────────────────────────────────┐
│                  INFRASTRUCTURE LAYER                                │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │  Lama.Infrastructure (Implementations & Data Access)        │   │
│  │  ├─ Data/                                                   │   │
│  │  │  ├─ LamaDbContext.cs (EF Core DbContext)               │   │
│  │  │  └─ Configurations/ (Fluent Mappings)                  │   │
│  │  │     ├─ ChapterConfiguration.cs                         │   │
│  │  │     ├─ MemberConfiguration.cs                          │   │
│  │  │     ├─ VehicleConfiguration.cs                         │   │
│  │  │     ├─ EventConfiguration.cs                           │   │
│  │  │     ├─ AttendanceConfiguration.cs                      │   │
│  │  │     └─ ConfigurationConfiguration.cs                   │   │
│  │  ├─ Repositories/ (Repository Implementations)             │   │
│  │  │  ├─ MemberRepository.cs                                │   │
│  │  │  ├─ VehicleRepository.cs                               │   │
│  │  │  ├─ EventRepository.cs                                 │   │
│  │  │  └─ AttendanceRepository.cs                            │   │
│  │  └─ Services/ (Service Implementations)                    │   │
│  │     ├─ AppConfigProvider.cs                               │   │
│  │     ├─ PointsCalculatorService.cs                         │   │
│  │     ├─ FakeBlobStorageService.cs                          │   │
│  │     └─ AttendanceConfirmationService.cs                   │   │
│  └─────────────────────────────────────────────────────────────┘   │
└──────────────────┬──────────────────────────────────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────────────────────────────────┐
│                     DOMAIN LAYER                                     │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │  Lama.Domain (Pure Business Logic)                          │   │
│  │  ├─ Entities/                                               │   │
│  │  │  ├─ Chapter.cs                                          │   │
│  │  │  ├─ Member.cs                                           │   │
│  │  │  ├─ Vehicle.cs                                          │   │
│  │  │  ├─ Event.cs                                            │   │
│  │  │  ├─ Attendance.cs                                       │   │
│  │  │  └─ Configuration.cs                                    │   │
│  │  └─ Enums/                                                  │   │
│  │     ├─ OdometerUnit.cs                                     │   │
│  │     ├─ AttendanceStatus.cs                                 │   │
│  │     ├─ VisitorClass.cs                                     │   │
│  │     ├─ EvidenceType.cs                                     │   │
│  │     ├─ PhotographyStatus.cs                                │   │
│  │     └─ MemberStatus.cs                                     │   │
│  └─────────────────────────────────────────────────────────────┘   │
└──────────────────┬──────────────────────────────────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────────────────────────────────┐
│                   EXTERNAL DEPENDENCIES                              │
│  ┌────────────────────────────────────────────────────────────┐    │
│  │  • SQL Server / Azure SQL Database                         │    │
│  │  • Entity Framework Core 8                                 │    │
│  │  • .NET 8 Runtime                                          │    │
│  └────────────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────────────┘
```

---

## **FLUJO DE CONFIRMACIÓN DE ASISTENCIA**

```
┌────────────────────────────────────────────────────────────────────┐
│ 1. CLIENT REQUEST                                                   │
│    POST /api/admin/evidence/upload?eventId=1                       │
│    Form Data:                                                       │
│    - memberId, vehicleId, evidenceType                             │
│    - 2 archivos JPG (piloto+moto, odómetro)                       │
│    - odometerReading, unit, fecha, notas                          │
└────────────┬─────────────────────────────────────────────────────┘
             │
             ▼
┌────────────────────────────────────────────────────────────────────┐
│ 2. AdminController.UploadEvidenceAsync()                            │
│    - Validación de entrada                                          │
│    - Lectura de streams de archivos                                 │
│    - Creación UploadEvidenceRequest                                │
└────────────┬─────────────────────────────────────────────────────┘
             │
             ▼
┌────────────────────────────────────────────────────────────────────┐
│ 3. IAttendanceConfirmationService.ConfirmAttendanceAsync()         │
│    ├─ BEGIN TRANSACTION                                            │
│    │                                                               │
│    ├─ A. VALIDAR DATOS                                            │
│    │   - Obtener Event por ID                                    │
│    │   - Obtener Vehicle por ID                                  │
│    │   - Obtener Member del vehículo                             │
│    │   - Obtener Attendance (EventId, MemberId)                  │
│    │                                                               │
│    ├─ B. SUBIR FOTOS A BLOB STORAGE                              │
│    │   - IBlobStorageService.UploadAsync(pilotPhoto)             │
│    │   - IBlobStorageService.UploadAsync(odometerPhoto)          │
│    │   → Retorna URLs                                            │
│    │                                                               │
│    ├─ C. ACTUALIZAR VEHICLE                                      │
│    │   IF evidenceType == "START_YEAR"                           │
│    │     vehicle.StartingOdometer = odometerReading              │
│    │     vehicle.StartYearEvidenceUrl = pilotPhotoUrl            │
│    │   ELSE IF evidenceType == "CUTOFF"                          │
│    │     vehicle.FinalOdometer = odometerReading                 │
│    │     vehicle.CutOffEvidenceUrl = pilotPhotoUrl               │
│    │   vehicle.OdometerUnit = unit                               │
│    │   vehicle.Photography = "VALIDATED"                         │
│    │   → UpdateAsync(vehicle)                                    │
│    │                                                               │
│    ├─ D. CALCULAR PUNTOS                                         │
│    │   IPointsCalculatorService.CalculateAsync(                 │
│    │     eventMileage,                                           │
│    │     eventClass,                                             │
│    │     memberCountry,                                          │
│    │     memberContinent,                                        │
│    │     eventLocationCountry,                                   │
│    │     eventLocationContinent                                  │
│    │   )                                                          │
│    │   → PointsCalculationResult {                               │
│    │      PointsPerEvent,                                        │
│    │      PointsPerDistance,                                     │
│    │      VisitorBonus,                                          │
│    │      TotalPoints,                                           │
│    │      VisitorClassification                                  │
│    │    }                                                          │
│    │                                                               │
│    ├─ E. ACTUALIZAR ATTENDANCE                                   │
│    │   attendance.Status = "CONFIRMED"                           │
│    │   attendance.PointsPerEvent = calculatedPoints.PerEvent     │
│    │   attendance.PointsPerDistance = calculatedPoints.PerDist   │
│    │   attendance.PointsAwardedPerMember = calculatedPoints.Total│
│    │   attendance.VisitorClass = calculatedPoints.Classification │
│    │   attendance.ConfirmedAt = DateTime.UtcNow                  │
│    │   attendance.ConfirmedBy = validatedByMemberId              │
│    │   → UpdateAsync(attendance)                                 │
│    │                                                               │
│    ├─ F. GUARDAR CAMBIOS                                         │
│    │   SaveChangesAsync()                                        │
│    │                                                               │
│    └─ COMMIT TRANSACTION                                          │
└────────────┬─────────────────────────────────────────────────────┘
             │
             ▼
┌────────────────────────────────────────────────────────────────────┐
│ 4. RESPONSE (200 OK)                                                │
│    {                                                                │
│      "message": "Asistencia confirmada exitosamente. Puntos: 8",  │
│      "pointsAwarded": 8,                                           │
│      "pointsPerEvent": 5,                                          │
│      "pointsPerDistance": 1,                                       │
│      "visitorClass": "Local",                                      │
│      "memberId": 1,                                                │
│      "vehicleId": 1,                                               │
│      "attendanceId": 1,                                            │
│      "evidenceType": "START_YEAR"                                  │
│    }                                                                │
└────────────────────────────────────────────────────────────────────┘
```

---

## **CÁLCULO DE PUNTOS - DETALLE MATEMÁTICO**

```
ENTRADA:
  - eventMileage (double): distancia en millas
  - eventClass (int): 1-5
  - memberCountry (string): país del miembro
  - memberContinent (string): continente del miembro
  - eventCountry (string): país del evento
  - eventContinent (string): continente del evento

LÓGICA:

1. PUNTOS POR EVENTO (PointsPerEvent)
   ├─ Leer config: PointsPerClassMultiplier_{class}
   ├─ Valores por defecto:
   │  ├─ Class 1 → 1 punto
   │  ├─ Class 2 → 3 puntos
   │  ├─ Class 3 → 5 puntos
   │  ├─ Class 4 → 10 puntos
   │  └─ Class 5 → 15 puntos
   └─ PointsPerEvent = multiplier[eventClass]

2. PUNTOS POR DISTANCIA (PointsPerDistance)
   ├─ Leer config:
   │  ├─ DistanceThreshold_1Point_OneWayMiles (default 200)
   │  └─ DistanceThreshold_2Points_OneWayMiles (default 800)
   ├─ Lógica:
   │  ├─ IF eventMileage > 800 → PointsPerDistance = 2
   │  ├─ ELSE IF eventMileage > 200 → PointsPerDistance = 1
   │  └─ ELSE → PointsPerDistance = 0
   └─ Nota: Convertir de KM a Miles si es necesario

3. BONUS VISITANTE (VisitorBonus + VisitorClass)
   ├─ IF memberCountry == eventCountry
   │  ├─ VisitorClass = LOCAL
   │  └─ VisitorBonus = 0
   ├─ ELSE (países diferentes)
   │  ├─ IF memberContinent == eventContinent
   │  │  ├─ VisitorClass = VISITOR_A (mismo continente)
   │  │  ├─ VisitorBonus = config[VisitorBonus_SameContinent] (default 1)
   │  │  └─ Leer config: VisitorBonus_SameContinent
   │  └─ ELSE
   │     ├─ VisitorClass = VISITOR_B (diferente continente)
   │     ├─ VisitorBonus = config[VisitorBonus_DifferentContinent] (default 2)
   │     └─ Leer config: VisitorBonus_DifferentContinent
   ├─ Fallback: sin datos de continente → VISITOR_A (1 punto)
   └─ Nota: Sensible a mayúsculas/minúsculas (usar .ToUpper())

4. TOTAL (TotalPoints)
   └─ TotalPoints = PointsPerEvent + PointsPerDistance + VisitorBonus

SALIDA:
  PointsCalculationResult {
    PointsPerEvent: int
    PointsPerDistance: int
    VisitorBonus: int
    TotalPoints: int (= PointsPerEvent + PointsPerDistance + VisitorBonus)
    VisitorClassification: enum { Local, VisitorA, VisitorB }
    CalculationDetails: string (auditoría)
  }

EJEMPLO:
  Input:
    - eventMileage = 500
    - eventClass = 3
    - memberCountry = "Mexico"
    - memberContinent = "Americas"
    - eventCountry = "USA"
    - eventContinent = "Americas"
  
  Cálculo:
    - PointsPerEvent = 5 (Class 3)
    - PointsPerDistance = 1 (500 > 200, ≤ 800)
    - VisitorBonus = 1 (Mexico vs USA, mismo continente)
    - TotalPoints = 5 + 1 + 1 = 7
    - VisitorClass = VISITOR_A
  
  Output:
    PointsCalculationResult {
      PointsPerEvent: 5,
      PointsPerDistance: 1,
      VisitorBonus: 1,
      TotalPoints: 7,
      VisitorClassification: VisitorA,
      CalculationDetails: "Class:3 miles:500.00mi PointsPerEvent:5 PointsPerDistance:1 VisitorBonus(VisitorA):1 Total:7"
    }
```

---

## **MODELO RELACIONAL SQL**

```
┌──────────────┐
│   Chapters   │
├──────────────┤
│ Id (PK)      │◄──┬──────────────────┐
│ Name         │   │                  │
│ Country      │   │                  │
│ IsActive     │   │                  │
└──────────────┘   │                  │
                   │                  │
                   ▼                  ▼
        ┌──────────────────┐  ┌──────────────────┐
        │    Members       │  │     Events       │
        ├──────────────────┤  ├──────────────────┤
        │ Id (PK)          │  │ Id (PK)          │
        │ ChapterId (FK) ──┼──┤                  │
        │ Order            │  │ ChapterId (FK)───┼──┐
        │ Complete Names   │  │ Order            │  │
        │ Country Birth    │  │ Event Start Date │  │
        │ Continent        │  │ Name             │  │
        │ Status           │  │ Class (1-5)      │  │
        │ is_eligible      │  │ Mileage          │  │
        └────┬─────────────┘  └────┬─────────────┘  │
             │                     │                │
             │ 1:N                 │ 1:N            │
             ▼                     ▼                │
        ┌──────────────────┐  ┌──────────────────┐ │
        │    Vehicles      │  │   Attendance     │ │
        ├──────────────────┤  ├──────────────────┤ │
        │ Id (PK)          │  │ Id (PK)          │ │
        │ MemberId (FK)────┼─┬┤ EventId (FK)─────┼─┘
        │ Lic Plate (UQ)   │ │ MemberId (FK)────┼───┐
        │ Motorcycle Data  │ │ VehicleId (FK)───┼─┐ │
        │ OdometerUnit     │ │ Status           │ │ │
        │ Starting Odometer│ │ PointsPerEvent   │ │ │
        │ Final Odometer   │ │ PointsPerDist    │ │ │
        │ StartYear... URL │ │ PointsAwarded    │ │ │
        │ CutOff... URL    │ │ VisitorClass     │ │ │
        │ Photography      │ │ ConfirmedAt      │ │ │
        │ IsActive...      │ │ ConfirmedBy (FK) │ │ │
        └──────────────────┘  └──────────────────┘ │
                                                   │
                                 UNIQUE(EventId, MemberId)
                                                   │
                                 FK VisitorClass ──┘
                                    ConfirmedBy ─────→ Members

┌──────────────────────────────┐
│     Configuration            │
├──────────────────────────────┤
│ Id (PK)                      │
│ Key (UQ)                     │
│ Value                        │
│ Description                  │
│ UpdatedAt                    │
└──────────────────────────────┘

ÍNDICES:
  - Members.ChapterId
  - Vehicles.MemberId
  - Vehicles.IsActiveForChampionship
  - Events.ChapterId
  - Attendance.EventId
  - Attendance.MemberId
  - Attendance.Status
  - Attendance.UNIQUE(EventId, MemberId)

TRIGGERS:
  - tr_MaxTwoActiveVehiclesPerMember: Limita max 2 motos activas/miembro
```

---

## **DEPENDENCY INJECTION GRAPH**

```
ServiceCollection
├─ DbContext: LamaDbContext
│  └─ SqlServer Connection
│
├─ Repositories:
│  ├─ IMemberRepository → MemberRepository
│  ├─ IVehicleRepository → VehicleRepository
│  ├─ IEventRepository → EventRepository
│  └─ IAttendanceRepository → AttendanceRepository
│
└─ Services:
   ├─ IAppConfigProvider → AppConfigProvider
   │  └─ Dependencies: LamaDbContext
   │
   ├─ IPointsCalculatorService → PointsCalculatorService
   │  └─ Dependencies: IAppConfigProvider
   │
   ├─ IBlobStorageService → FakeBlobStorageService
   │  └─ No dependencies
   │
   └─ IAttendanceConfirmationService → AttendanceConfirmationService
      └─ Dependencies:
         ├─ IAttendanceRepository
         ├─ IVehicleRepository
         ├─ IEventRepository
         ├─ IBlobStorageService
         ├─ IPointsCalculatorService
         └─ LamaDbContext
```

---

## **PUNTOS CLAVE DE DISEÑO**

### **1. Separación de Capas**
- **Domain**: Puras entidades, enums, sin lógica de acceso a datos
- **Application**: Interfaces que definen contratos, DTOs
- **Infrastructure**: Implementaciones concretas, DbContext, repos
- **API**: Controllers, Program.cs, configuración HTTP

### **2. Dependency Injection**
- Todos los servicios registrados en `ServiceCollectionExtensions`
- Inyección a través de constructores (primary constructor en C# 12)
- Sin singletons de DbContext (scope per request)

### **3. Repository Pattern**
- Abstracción de acceso a datos con interfaces
- Fácil de testear con mocks
- Implementaciones genéricas pero específicas por entidad

### **4. Transactions**
- `AttendanceConfirmationService` usa `DbContext.Database.BeginTransactionAsync()`
- Rollback automático en catch
- Garantiza integridad transaccional de fotos + odómetro + puntos

### **5. Configuración Dinámica**
- `IAppConfigProvider` lee desde tabla Configuration
- Permite ajustar thresholds sin recompilación
- Cache opcional (future optimization)

### **6. Blob Storage Abstracto**
- `IBlobStorageService` como interfaz
- `FakeBlobStorageService` para dev/testing
- Reemplazable con Azure Blob Storage en producción

### **7. Fluent Mapping de EF Core**
- `HasColumnName()` para columnas con espacios SQL
- Configuraciones centralizadas por entidad
- Constraint único para (EventId, MemberId)

---

## **TESTING STRATEGY**

```
Lama.UnitTests/
└─ Services/
   └─ PointsCalculatorServiceTests.cs
      ├─ Points Per Class Tests (Class 1-5)
      ├─ Points Per Distance Tests (umbrales 200/800)
      ├─ Visitor Bonus Tests (Local/A/B)
      ├─ Total Points Calculation Tests
      └─ Conversion Tests (KM ↔ Miles)

Estrategia:
  - Mock IAppConfigProvider
  - Test casos límite (boundary conditions)
  - Cobertura: >90%
  - Framework: xUnit + Moq
```

---

**Arquitectura robusta, escalable y lista para evolucionar a V2 con autenticación, endpoints móviles y Cloud Storage.**
