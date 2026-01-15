-- ============================================================================
-- CORRECCIÓN: Recrear tablas con nombres de columna CORRECTOS (con espacios)
-- Esto es CRÍTICO porque el Excel tiene espacios iniciales
-- ============================================================================

USE [LamaDb];
GO

PRINT '═══════════════════════════════════════════════════════════════════════';
PRINT 'INICIANDO CORRECCIÓN DE NOMBRES DE COLUMNAS';
PRINT '═══════════════════════════════════════════════════════════════════════';
PRINT '';

-- Paso 1: Hacer backup de datos (crear tablas temporales)
PRINT 'PASO 1: Backup de datos';
GO

SELECT * INTO [dbo].[Members_BACKUP] FROM [dbo].[Members];
SELECT * INTO [dbo].[Vehicles_BACKUP] FROM [dbo].[Vehicles];
SELECT * INTO [dbo].[Events_BACKUP] FROM [dbo].[Events];
SELECT * INTO [dbo].[Attendance_BACKUP] FROM [dbo].[Attendance];

PRINT '✓ Backup creado en tablas _BACKUP';
PRINT '';
GO

-- Paso 2: Eliminar vistas y FK que dependen de las tablas
PRINT 'PASO 2: Eliminar vistas y referencias';
GO

DROP VIEW IF EXISTS [dbo].[vw_MasterOdometerReport];
DROP VIEW IF EXISTS [dbo].[vw_MemberGeneralRanking];
DROP VIEW IF EXISTS [dbo].[vw_EventDetailWithAttendance];

PRINT '✓ Vistas eliminadas';
GO

-- Paso 3: Eliminar las tablas viejas
PRINT 'PASO 3: Eliminar tablas antiguas';
GO

DROP TABLE IF EXISTS [dbo].[Attendance];
DROP TABLE IF EXISTS [dbo].[Events];
DROP TABLE IF EXISTS [dbo].[Vehicles];
DROP TABLE IF EXISTS [dbo].[Members];

PRINT '✓ Tablas antiguas eliminadas';
PRINT '';
GO

-- Paso 4: Recrear tablas CON ESPACIOS INICIALES CORRECTOS
PRINT 'PASO 4: Crear tablas con nombres de columna CORRECTOS';
GO

CREATE TABLE [dbo].[Members] (
    [MemberId] INT IDENTITY(1,1) PRIMARY KEY,
    [ChapterId] INT NOT NULL,
    [ Complete Names] NVARCHAR(200) NOT NULL,
    [ Country Birth] NVARCHAR(100),
    [ In Lama Since] INT,
    [Continent] NVARCHAR(100),
    [STATUS] NVARCHAR(20) DEFAULT 'ACTIVE',
    [is_eligible] BIT DEFAULT 1,
    [Order] INT,
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_Members_Chapters] FOREIGN KEY ([ChapterId]) REFERENCES [dbo].[Chapters]([ChapterId])
);

CREATE TABLE [dbo].[Vehicles] (
    [VehicleId] INT IDENTITY(1,1) PRIMARY KEY,
    [MemberId] INT NOT NULL,
    [ Motorcycle Data] NVARCHAR(255),
    [ Lic Plate] NVARCHAR(20) UNIQUE NOT NULL,
    [ Trike] BIT DEFAULT 0,
    [OdometerUnit] NVARCHAR(10) DEFAULT 'km',
    [ Starting Odometer] FLOAT DEFAULT 0,
    [ Final Odometer] FLOAT DEFAULT 0,
    [StartYearEvidenceUrl] NVARCHAR(500),
    [StartYearEvidenceValidatedAt] DATETIME2,
    [CutOffEvidenceUrl] NVARCHAR(500),
    [CutOffEvidenceValidatedAt] DATETIME2,
    [EvidenceValidatedBy] INT,
    [Photography] NVARCHAR(500),
    [IsActiveForChampionship] BIT DEFAULT 1,
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_Vehicles_Members] FOREIGN KEY ([MemberId]) REFERENCES [dbo].[Members]([MemberId])
);

CREATE TABLE [dbo].[Events] (
    [EventId] INT IDENTITY(1,1) PRIMARY KEY,
    [ChapterId] INT NOT NULL,
    [ Event Start Date (AAAA/MM/DD)] DATE,
    [ Name of the event] NVARCHAR(255),
    [Class] NVARCHAR(100),
    [ Mileage] FLOAT,
    [ Points per event] INT DEFAULT 0,
    [ Points per Distance] INT DEFAULT 0,
    [ Points awarded per member] INT DEFAULT 0,
    [Country] NVARCHAR(100),
    [Continent] NVARCHAR(100),
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_Events_Chapters] FOREIGN KEY ([ChapterId]) REFERENCES [dbo].[Chapters]([ChapterId])
);

CREATE TABLE [dbo].[Attendance] (
    [AttendanceId] INT IDENTITY(1,1) PRIMARY KEY,
    [EventId] INT NOT NULL,
    [MemberId] INT NOT NULL,
    [VehicleId] INT NOT NULL,
    [Status] NVARCHAR(50) DEFAULT 'ATTENDED',
    [ Points per event] INT DEFAULT 0,
    [ Points per Distance] INT DEFAULT 0,
    [ Points awarded per member] INT DEFAULT 0,
    [ Visitor Class] NVARCHAR(100),
    [ConfirmedAt] DATETIME2,
    [ConfirmedBy] INT,
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_Attendance_Events] FOREIGN KEY ([EventId]) REFERENCES [dbo].[Events]([EventId]),
    CONSTRAINT [FK_Attendance_Members] FOREIGN KEY ([MemberId]) REFERENCES [dbo].[Members]([MemberId]),
    CONSTRAINT [FK_Attendance_Vehicles] FOREIGN KEY ([VehicleId]) REFERENCES [dbo].[Vehicles]([VehicleId])
);

PRINT '✓ Tablas recreadas con espacios iniciales correctos';
PRINT '';
GO

-- Paso 5: Restaurar datos desde backup (ajustando nombres de columna)
PRINT 'PASO 5: Restaurar datos desde backup';
GO

SET IDENTITY_INSERT [dbo].[Members] ON;
INSERT INTO [dbo].[Members] 
    ([MemberId], [ChapterId], [ Complete Names], [ Country Birth], [ In Lama Since], [Continent], [STATUS], [is_eligible], [Order], [CreatedAt])
SELECT 
    [MemberId], [ChapterId], [Complete Names], [Country Birth], [In Lama Since], [Continent], [STATUS], [is_eligible], [Order], [CreatedAt]
FROM [dbo].[Members_BACKUP];
SET IDENTITY_INSERT [dbo].[Members] OFF;

SET IDENTITY_INSERT [dbo].[Vehicles] ON;
INSERT INTO [dbo].[Vehicles] 
    ([VehicleId], [MemberId], [ Motorcycle Data], [ Lic Plate], [ Trike], [OdometerUnit], [ Starting Odometer], [ Final Odometer], 
     [StartYearEvidenceUrl], [StartYearEvidenceValidatedAt], [CutOffEvidenceUrl], [CutOffEvidenceValidatedAt], 
     [EvidenceValidatedBy], [Photography], [IsActiveForChampionship], [CreatedAt])
SELECT 
    [VehicleId], [MemberId], [Motorcycle Data], [Lic Plate], [Trike], [OdometerUnit], [Starting Odometer], [Final Odometer],
    [StartYearEvidenceUrl], [StartYearEvidenceValidatedAt], [CutOffEvidenceUrl], [CutOffEvidenceValidatedAt],
    [EvidenceValidatedBy], [Photography], [IsActiveForChampionship], [CreatedAt]
FROM [dbo].[Vehicles_BACKUP];
SET IDENTITY_INSERT [dbo].[Vehicles] OFF;

SET IDENTITY_INSERT [dbo].[Events] ON;
INSERT INTO [dbo].[Events] 
    ([EventId], [ChapterId], [ Event Start Date (AAAA/MM/DD)], [ Name of the event], [Class], [ Mileage], 
     [ Points per event], [ Points per Distance], [ Points awarded per member], [Country], [Continent], [CreatedAt])
SELECT 
    [EventId], [ChapterId], [Event Start Date (AAAA/MM/DD)], [Name of the event], [Class], [Mileage],
    [Points per event], [Points per Distance], [Points awarded per member], [Country], [Continent], [CreatedAt]
FROM [dbo].[Events_BACKUP];
SET IDENTITY_INSERT [dbo].[Events] OFF;

SET IDENTITY_INSERT [dbo].[Attendance] ON;
INSERT INTO [dbo].[Attendance] 
    ([AttendanceId], [EventId], [MemberId], [VehicleId], [Status], [ Points per event], [ Points per Distance], 
     [ Points awarded per member], [ Visitor Class], [ConfirmedAt], [ConfirmedBy], [CreatedAt])
SELECT 
    [AttendanceId], [EventId], [MemberId], [VehicleId], [Status], [Points per event], [Points per Distance],
    [Points awarded per member], [Visitor Class], [ConfirmedAt], [ConfirmedBy], [CreatedAt]
FROM [dbo].[Attendance_BACKUP];
SET IDENTITY_INSERT [dbo].[Attendance] OFF;

PRINT '✓ Datos restaurados';
PRINT '';
GO

-- Paso 6: Limpiar backups
PRINT 'PASO 6: Limpiar archivos de backup';
GO

DROP TABLE [dbo].[Members_BACKUP];
DROP TABLE [dbo].[Vehicles_BACKUP];
DROP TABLE [dbo].[Events_BACKUP];
DROP TABLE [dbo].[Attendance_BACKUP];

PRINT '✓ Backup eliminado';
PRINT '';
GO

PRINT '═══════════════════════════════════════════════════════════════════════';
PRINT '✓ CORRECCIÓN DE BD COMPLETADA EXITOSAMENTE';
PRINT '═══════════════════════════════════════════════════════════════════════';
PRINT '';
PRINT 'PRÓXIMOS PASOS:';
PRINT '1. Actualizar Fluent Configurations en C# con espacios iniciales';
PRINT '2. Actualizar Python ETL (migration_generator.py) con espacios iniciales';
PRINT '3. Compilar y ejecutar tests';
PRINT '';
