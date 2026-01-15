# 🏍️ COR L.A.MA - Plataforma SaaS de Gestión Mototurística

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Azure](https://img.shields.io/badge/Azure-Bicep%20IaC-0078D4?logo=microsoftazure)](https://azure.microsoft.com/)
[![Next.js](https://img.shields.io/badge/Next.js-14-000000?logo=nextdotjs)](https://nextjs.org/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

**Sistema APIM-ready de gestión mototurística** con arquitectura Clean, versionado API, infraestructura como código en Azure y CI/CD automatizado.

---

## 📋 Tabla de Contenidos

- [🏗️ Arquitectura](#️-arquitectura)
- [📁 Estructura del Proyecto](#-estructura-del-proyecto)
- [🚀 Quick Start](#-quick-start)
- [☁️ Despliegue en Azure](#️-despliegue-en-azure)
- [🛠️ Desarrollo Local](#️-desarrollo-local)
- [📚 Documentación](#-documentación)

---

## 🏗️ Arquitectura

### Clean Architecture + Domain-Driven Design

```
┌─────────────────────────────────────────────────────────┐
│                      Presentation                        │
│  ┌──────────────────────────────────────────────────┐   │
│  │   Lama.API (ASP.NET Core 8)                      │   │
│  │   - Controllers (REST API versioned /api/v1)     │   │
│  │   - Swagger/OpenAPI con ejemplos                 │   │
│  │   - ProblemDetails (RFC 7807)                    │   │
│  │   - Kebab-case URLs + PascalCase JSON            │   │
│  └──────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
                            ▼
┌─────────────────────────────────────────────────────────┐
│                      Application                         │
│  ┌──────────────────────────────────────────────────┐   │
│  │   Lama.Application                               │   │
│  │   - Use Cases / Services                         │   │
│  │   - DTOs / ViewModels                            │   │
│  │   - Interfaces (Repository, Service)             │   │
│  └──────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
                            ▼
┌─────────────────────────────────────────────────────────┐
│                        Domain                            │
│  ┌──────────────────────────────────────────────────┐   │
│  │   Lama.Domain                                    │   │
│  │   - Entities (Activity, Membership, Evidence)    │   │
│  │   - Value Objects                                │   │
│  │   - Domain Events                                │   │
│  └──────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
                            ▼
┌─────────────────────────────────────────────────────────┐
│                     Infrastructure                       │
│  ┌──────────────────────────────────────────────────┐   │
│  │   Lama.Infrastructure (EF Core 8)                │   │
│  │   - DbContext + Migrations                       │   │
│  │   - Repository Implementations                   │   │
│  │   - External Services (Azure Storage, Redis)     │   │
│  └──────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
```

### Infraestructura Azure

```
┌────────────────────────────────────────────────────────────┐
│                      Azure Resources                       │
├────────────────────────────────────────────────────────────┤
│  Frontend (Next.js)                                        │
│  ┌──────────────────────────┐                             │
│  │ Static Web App (Free)    │◄──── CDN Global             │
│  │ - SSG + ISR              │                              │
│  │ - GitHub auto-deploy     │                              │
│  └──────────────────────────┘                             │
│            │                                                │
│            ▼ HTTPS                                          │
│  ┌─────────────────────────────────────────────────────┐  │
│  │          API (ASP.NET Core 8)                        │  │
│  │  App Service (Basic B1 → Premium P1v3)              │  │
│  │  - Managed Identity                                  │  │
│  │  - KeyVault Integration                              │  │
│  │  - Auto-scaling (prod)                               │  │
│  │  - Always On + Health Checks                         │  │
│  └─────────────────────────────────────────────────────┘  │
│            │                  │                             │
│            ▼                  ▼                             │
│  ┌─────────────────┐  ┌─────────────────┐                │
│  │ Azure SQL DB    │  │ Redis Cache     │                 │
│  │ - S0 → S3       │  │ - C0 → C1       │                 │
│  │ - Zone Redundant│  │ - SSL Enforced  │                 │
│  │ - Auto-backup   │  │ - Persistence   │                 │
│  └─────────────────┘  └─────────────────┘                │
│            │                                                │
│            ▼                                                │
│  ┌─────────────────────────────────────────────────────┐  │
│  │          Blob Storage (evidences)                    │  │
│  │  - LRS → ZRS (prod)                                  │  │
│  │  - Lifecycle management (730 days)                   │  │
│  │  - CORS enabled                                      │  │
│  └─────────────────────────────────────────────────────┘  │
│                                                             │
│  Security & Monitoring                                     │
│  ┌─────────────────────┐  ┌────────────────────────────┐ │
│  │ Key Vault           │  │ Application Insights       │ │
│  │ - Soft Delete (90d) │  │ - Log Analytics Workspace  │ │
│  │ - Purge Protection  │  │ - Metric Alerts            │ │
│  │ - Access Policies   │  │ - 90-day retention (prod)  │ │
│  └─────────────────────┘  └────────────────────────────┘ │
└────────────────────────────────────────────────────────────┘
```

---

## 📁 Estructura del Proyecto

```
COR L.A.MA/
├── src/
│   ├── Lama.Domain/            # Entidades, Value Objects, Enums
│   ├── Lama.Application/       # Use Cases, DTOs, Interfaces
│   ├── Lama.Infrastructure/    # EF Core, Repositories, Azure Services
│   └── Lama.API/               # Controllers, Middlewares, Program.cs
├── frontend/                    # Next.js App (TypeScript)
│   ├── app/                    # App Router (Next.js 14)
│   ├── components/             # React Components
│   └── lib/                    # Utilities, API Client
├── tests/
│   └── Lama.UnitTests/         # xUnit + Moq
├── infra/                       # ⚡ Infrastructure as Code
│   ├── bicep/
│   │   ├── main.bicep          # Main orchestrator
│   │   ├── modules/            # Bicep modules (sql, storage, etc.)
│   │   └── parameters.*.bicepparam
│   ├── scripts/                # Helper scripts (OIDC, secrets, validate)
│   ├── README.md               # Arquitectura, costos, guías
│   └── SETUP-GUIDE.md          # 🚀 Guía paso a paso
├── .github/workflows/
│   ├── deploy-infra.yml        # CI/CD para infraestructura
│   └── deploy-app.yml          # CI/CD para aplicación
├── sql/
│   ├── schema.sql              # Tablas, triggers, constraints
│   └── views.sql               # Vistas de reporting
└── python/
    └── migration_generator.py  # ETL desde Excel legacy

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
```

---

## 🚀 Quick Start

### Opción A: Despliegue Completo en Azure (Recomendado)

```bash
# 1. Clonar repositorio
git clone https://github.com/tu-usuario/COR-LAMA.git
cd COR-LAMA

# 2. Configurar OIDC y GitHub Environments (una sola vez)
cd infra/scripts
./setup-oidc.sh  # Sigue las instrucciones
cd ../..

# 3. Desplegar infraestructura (automático con push)
git add .
git commit -m "feat: initial commit"
git push origin main

# 4. Verificar despliegue
cd infra/scripts
./validate-deployment.sh dev

# ✅ Listo! API en: https://app-lama-dev.azurewebsites.net
```

**📖 Guía completa**: [`infra/SETUP-GUIDE.md`](infra/SETUP-GUIDE.md)

### Opción B: Desarrollo Local (Sin Azure)

```bash
# 1. Requisitos previos
# - .NET 8 SDK
# - SQL Server (LocalDB o Express)
# - Node.js 20+

# 2. Configurar base de datos
cd sql
sqlcmd -S (localdb)\mssqllocaldb -Q "CREATE DATABASE LamaDb"
sqlcmd -S (localdb)\mssqllocaldb -d LamaDb -i schema.sql
sqlcmd -S (localdb)\mssqllocaldb -d LamaDb -i views.sql

# 3. Restaurar dependencias
cd ../src
dotnet restore Lama.API/Lama.API.csproj

# 4. Ejecutar migraciones EF Core
cd Lama.Infrastructure
dotnet ef database update --startup-project ../Lama.API

# 5. Ejecutar API
cd ../Lama.API
dotnet run

# 6. Ejecutar Frontend (otra terminal)
cd ../../frontend
npm install
npm run dev

# ✅ API: http://localhost:5000/swagger
# ✅ Frontend: http://localhost:3000
```

---

## ☁️ Despliegue en Azure

### Infraestructura como Código (Bicep)

**Multi-ambiente**: dev, test, prod con SKUs diferenciados

| Recurso | SKU Dev | SKU Test | SKU Prod | Costo Mensual |
|---------|---------|----------|----------|---------------|
| SQL Database | S0 (10 DTU) | S1 (20 DTU) | S3 (100 DTU) ZRS | $15 → $200 |
| App Service | Basic B1 | Standard S1 | Premium P1v3 | $13 → $105 |
| Redis Cache | C0 (250MB) | C0 (250MB) | C1 (1GB) Standard | $17 → $75 |
| Storage | LRS | LRS | ZRS + lifecycle | $2 → $5 |
| Static Web App | Free | Free | Standard (CDN) | $0 → $9 |
| **Total/mes** | **~$51** | **~$114** | **~$409** | |

### CI/CD Automatizado (GitHub Actions)

**Workflow infraestructura** ([`.github/workflows/deploy-infra.yml`](.github/workflows/deploy-infra.yml)):
- ✅ Validación Bicep en cada PR
- 🚀 Deploy automático a DEV en push a `main`
- 🔐 Deploy manual a TEST/PROD con aprobación
- 🧪 What-if analysis antes de deploy
- 🧹 Destroy job para cleanup

**Workflow aplicación** ([`.github/workflows/deploy-app.yml`](.github/workflows/deploy-app.yml)):
- 🔨 Build .NET 8 + Next.js en paralelo
- 📦 Deploy API a App Service + Slot swap (prod)
- 🌐 Deploy Frontend a Static Web App
- 🗄️ EF Core migrations automáticas
- 🩺 Health checks post-deployment

### Seguridad

- 🔑 **OIDC Federation** (sin credenciales en GitHub)
- 🔐 **Azure Key Vault** para secretos
- 🛡️ **Managed Identity** (App Service → KeyVault/SQL)
- 🔒 **SSL/TLS enforced** (Redis, SQL, Storage)
- 📊 **Application Insights** + Log Analytics

---

## 🛠️ Desarrollo Local

### Prerrequisitos

- **Visual Studio 2022** (v17.8+) o **VS Code** + .NET CLI
- **.NET 8 SDK** ([descargar](https://dotnet.microsoft.com/download/dotnet/8.0))
- **SQL Server 2022** o **LocalDB** (incluido en VS)
- **Node.js 20+** ([descargar](https://nodejs.org/))
- **Azure CLI** (opcional, para testing con recursos Azure)

### Configuración Inicial

#### 1. Base de Datos Local

```bash
# Crear base de datos
sqlcmd -S (localdb)\mssqllocaldb -Q "CREATE DATABASE LamaDb"

# Aplicar esquema SQL
cd sql
sqlcmd -S (localdb)\mssqllocaldb -d LamaDb -i schema.sql
sqlcmd -S (localdb)\mssqllocaldb -d LamaDb -i views.sql

# O usar EF Core Migrations (recomendado):
cd ../src/Lama.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../Lama.API
dotnet ef database update --startup-project ../Lama.API
```

#### 2. Configurar Connection Strings

Edita [`src/Lama.API/appsettings.Development.json`](src/Lama.API/appsettings.Development.json):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=LamaDb;Trusted_Connection=true;",
    "RedisConnection": "localhost:6379"
  },
  "BlobStorage": {
    "ConnectionString": "UseDevelopmentStorage=true",
    "ContainerName": "evidences"
  }
}
```

**Nota**: Para Azure Storage local, instala [Azurite](https://learn.microsoft.com/azure/storage/common/storage-use-azurite):
```bash
npm install -g azurite
azurite --silent
```

#### 3. Migración de Datos desde Excel (Opcional)

Si tienes datos legacy en Excel:

```bash
# Instalar dependencias Python
pip install pandas openpyxl

# Ejecutar script de migración
cd python
python migration_generator.py

# Aplicar SQL generado
sqlcmd -S (localdb)\mssqllocaldb -d LamaDb -i migration_script.sql
```

#### 4. Ejecutar API + Frontend

```bash
# Terminal 1: API
cd src/Lama.API
dotnet watch run  # Hot reload habilitado

# Terminal 2: Frontend
cd frontend
npm run dev

# ✅ API Swagger: https://localhost:7001/swagger
# ✅ Frontend: http://localhost:3000
```

### Endpoints API

**Base URL (local)**: `https://localhost:7001/api/v1`

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/activities` | Lista todas las actividades |
| GET | `/activities/{id}` | Obtiene actividad por ID |
| POST | `/activities` | Crea nueva actividad |
| PUT | `/activities/{id}` | Actualiza actividad |
| DELETE | `/activities/{id}` | Elimina actividad |
| GET | `/memberships` | Lista membresías activas |
| GET | `/evidences/by-activity/{id}` | Evidencias de una actividad |
| POST | `/evidences/upload` | Sube foto a Azure Blob Storage |

**Swagger UI**: `https://localhost:7001/swagger/index.html`

### Testing

```bash
# Ejecutar tests unitarios
dotnet test tests/Lama.UnitTests/

# Con cobertura
dotnet test /p:CollectCoverage=true /p:CoverageReportsFormat=opencover

# Filtrar por categoría
dotnet test --filter Category=Unit
```

---

## 📚 Documentación

### Infraestructura

- 📖 **[`infra/README.md`](infra/README.md)**: Arquitectura detallada, costos, recursos Azure
- 🚀 **[`infra/SETUP-GUIDE.md`](infra/SETUP-GUIDE.md)**: Guía paso a paso para configurar OIDC y desplegar
- 🔧 **Scripts auxiliares**:
  - [`setup-oidc.sh`](infra/scripts/setup-oidc.sh): Configura OIDC en Azure AD
  - [`get-secrets.ps1`](infra/scripts/get-secrets.ps1): Obtiene secretos de KeyVault para debug local
  - [`get-swa-tokens.ps1`](infra/scripts/get-swa-tokens.ps1): Obtiene API tokens de Static Web Apps
  - [`validate-deployment.sh`](infra/scripts/validate-deployment.sh): Valida que todos los recursos funcionen

### API

- **OpenAPI Spec**: `https://app-lama-{env}.azurewebsites.net/swagger/v1/swagger.json`
- **Versionado**: `/api/v1` (actual), `/api/v2` (futuro)
- **Autenticación**: Bearer Token (Azure AD B2C - próximamente)
- **Rate Limiting**: 1000 requests/min (prod)

### Arquitectura

```
Clean Architecture Layers:

Domain (Core)
  ├── Entities: Activity, Membership, Evidence, Report
  ├── Enums: ActivityType, MembershipStatus, Gender
  └── Interfaces: IEntity, IRepository<T>

Application (Use Cases)
  ├── Services: ActivityService, EvidenceService
  ├── DTOs: ActivityDTO, MembershipDTO
  └── Interfaces: IActivityService, IBlobStorageService

Infrastructure (External)
  ├── Persistence: LamaDbContext, EF Core Repositories
  ├── Azure: BlobStorageService, RedisCacheService
  └── Migrations: EF Core code-first migrations

API (Presentation)
  ├── Controllers: ActivitiesController, MembershipsController
  ├── Middlewares: ExceptionHandlingMiddleware
  └── Configuration: Swagger, CORS, ProblemDetails
```

### Database Schema

**Principales tablas**:
- `Activities`: Actividades mototurísticas (ODO, KM, fecha)
- `Memberships`: Información de miembros
- `Evidences`: Metadata de fotos (Azure Blob Storage)
- `Reports`: Reportes históricos

**Vistas SQL**:
- `vw_MasterOdometerReport`: Odómetro maestro consolidado
- `vw_MemberGeneralRanking`: Ranking general por KM recorridos

---

## 🤝 Contribuir

### Branching Strategy

```bash
main            # Producción (protegido)
  ├── develop   # Integración continua
  │   ├── feature/nueva-funcionalidad
  │   ├── bugfix/correccion-error
  │   └── hotfix/parche-urgente
```

### Pull Request Process

1. Crea una branch desde `develop`
2. Implementa cambios + tests
3. Ejecuta `dotnet test` y `npm run lint`
4. Abre PR con descripción detallada
5. Espera aprobación (1+ reviewer)
6. Squash merge a `develop`

---

## 📄 Licencia

Este proyecto está licenciado bajo MIT License - ver [`LICENSE`](LICENSE)

---

## 📞 Soporte

- 📧 Email: soporte@corlama.com
- 📚 Docs: [`infra/README.md`](infra/README.md)
- 🐛 Issues: [GitHub Issues](https://github.com/tu-usuario/COR-LAMA/issues)
- 💬 Discussions: [GitHub Discussions](https://github.com/tu-usuario/COR-LAMA/discussions)

---

**Hecho con ❤️ para la comunidad mototurística de L.A.M.A.** 🏍️
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
