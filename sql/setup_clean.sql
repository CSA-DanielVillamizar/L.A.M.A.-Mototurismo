-- ============================================================================
-- SETUP LIMPIO: Recrea todas las tablas, triggers y vistas
-- Base de datos: LamaDb
-- Credenciales: sa / Mc901128365-2**
-- Servidor: P-DVILLAMIZARA
-- ============================================================================

USE [LamaDb];
GO

-- Eliminar objetos en orden inverso de dependencias
IF OBJECT_ID('[dbo].[vw_MemberGeneralRanking]', 'V') IS NOT NULL 
    DROP VIEW [dbo].[vw_MemberGeneralRanking];
GO

IF OBJECT_ID('[dbo].[vw_MasterOdometerReport]', 'V') IS NOT NULL 
    DROP VIEW [dbo].[vw_MasterOdometerReport];
GO

IF OBJECT_ID('[dbo].[tr_MaxTwoActiveVehiclesPerMember]', 'TR') IS NOT NULL 
    DROP TRIGGER [dbo].[tr_MaxTwoActiveVehiclesPerMember];
GO

IF OBJECT_ID('[dbo].[Attendance]', 'U') IS NOT NULL 
    DROP TABLE [dbo].[Attendance];
GO

IF OBJECT_ID('[dbo].[Events]', 'U') IS NOT NULL 
    DROP TABLE [dbo].[Events];
GO

IF OBJECT_ID('[dbo].[Vehicles]', 'U') IS NOT NULL 
    DROP TABLE [dbo].[Vehicles];
GO

IF OBJECT_ID('[dbo].[Members]', 'U') IS NOT NULL 
    DROP TABLE [dbo].[Members];
GO

IF OBJECT_ID('[dbo].[Chapters]', 'U') IS NOT NULL 
    DROP TABLE [dbo].[Chapters];
GO

IF OBJECT_ID('[dbo].[Configuration]', 'U') IS NOT NULL 
    DROP TABLE [dbo].[Configuration];
GO

-- ============================================================================
-- CREATE TABLES
-- ============================================================================

-- Tabla: Chapters (Capítulos de miembros)
CREATE TABLE [dbo].[Chapters] (
    [ChapterId] INT PRIMARY KEY IDENTITY(1,1),
    [Name] NVARCHAR(100) NOT NULL,
    [Country] NVARCHAR(100),
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE()
);

-- Tabla: Members (Miembros del campeonato)
CREATE TABLE [dbo].[Members] (
    [MemberId] INT PRIMARY KEY IDENTITY(1,1),
    [ChapterId] INT NOT NULL,
    [ Complete Names] NVARCHAR(200) NOT NULL,
    [Country Birth] NVARCHAR(100),
    [Continent] NVARCHAR(50),
    [In Lama Since] INT,
    [STATUS] NVARCHAR(20) DEFAULT 'ACTIVE',
    [is_eligible] BIT DEFAULT 1,
    [Order] INT,
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_Members_Chapters] FOREIGN KEY ([ChapterId]) REFERENCES [dbo].[Chapters]([ChapterId]),
    CONSTRAINT [CK_Members_Status] CHECK ([STATUS] IN ('ACTIVE', 'INACTIVE'))
);

-- Tabla: Vehicles (Motos de los miembros)
CREATE TABLE [dbo].[Vehicles] (
    [VehicleId] INT PRIMARY KEY IDENTITY(1,1),
    [MemberId] INT NOT NULL,
    [ Motorcycle Data] NVARCHAR(255),
    [Lic Plate] NVARCHAR(20) UNIQUE NULL,
    [Trike] BIT DEFAULT 0,
    [OdometerUnit] NVARCHAR(20) DEFAULT 'Miles',
    [Starting Odometer] FLOAT DEFAULT 0,
    [Final Odometer] FLOAT DEFAULT 0,
    [StartYearEvidenceUrl] NVARCHAR(500),
    [StartYearEvidenceValidatedAt] DATETIME2,
    [CutOffEvidenceUrl] NVARCHAR(500),
    [CutOffEvidenceValidatedAt] DATETIME2,
    [EvidenceValidatedBy] INT,
    [Photography] NVARCHAR(20) DEFAULT 'PENDING',
    [IsActiveForChampionship] BIT DEFAULT 1,
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_Vehicles_Members] FOREIGN KEY ([MemberId]) REFERENCES [dbo].[Members]([MemberId]),
    CONSTRAINT [CK_Vehicles_OdometerUnit] CHECK ([OdometerUnit] IN ('Miles', 'Kilometers')),
    CONSTRAINT [CK_Vehicles_Photography] CHECK ([Photography] IN ('PENDING', 'VALIDATED'))
);

-- Tabla: Events (Eventos del campeonato)
CREATE TABLE [dbo].[Events] (
    [EventId] INT PRIMARY KEY IDENTITY(1,1),
    [ChapterId] INT NOT NULL,
    [Event Start Date (AAAA/MM/DD)] DATE,
    [Name of the event] NVARCHAR(200) NOT NULL,
    [Class] INT CHECK ([Class] BETWEEN 1 AND 5),
    [Mileage] FLOAT,
    [Points per event] INT,
    [Points per Distance] INT,
    [Points awarded per member] INT,
    [Country] NVARCHAR(100),
    [Continent] NVARCHAR(50),
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_Events_Chapters] FOREIGN KEY ([ChapterId]) REFERENCES [dbo].[Chapters]([ChapterId])
);

-- Tabla: Attendance (Asistencias a eventos)
CREATE TABLE [dbo].[Attendance] (
    [AttendanceId] INT PRIMARY KEY IDENTITY(1,1),
    [EventId] INT NOT NULL,
    [MemberId] INT NOT NULL,
    [VehicleId] INT NOT NULL,
    [Status] NVARCHAR(20) DEFAULT 'PENDING',
    [Points per event] INT DEFAULT 0,
    [Points per Distance] INT DEFAULT 0,
    [Points awarded per member] INT DEFAULT 0,
    [Visitor Class] NVARCHAR(20),
    [ConfirmedAt] DATETIME2,
    [ConfirmedBy] INT,
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_Attendance_Events] FOREIGN KEY ([EventId]) REFERENCES [dbo].[Events]([EventId]),
    CONSTRAINT [FK_Attendance_Members] FOREIGN KEY ([MemberId]) REFERENCES [dbo].[Members]([MemberId]),
    CONSTRAINT [FK_Attendance_Vehicles] FOREIGN KEY ([VehicleId]) REFERENCES [dbo].[Vehicles]([VehicleId]),
    CONSTRAINT [UQ_Attendance_EventMember] UNIQUE ([EventId], [MemberId]),
    CONSTRAINT [CK_Attendance_Status] CHECK ([Status] IN ('PENDING', 'CONFIRMED', 'REJECTED')),
    CONSTRAINT [CK_Attendance_VisitorClass] CHECK ([Visitor Class] IN ('LOCAL', 'VisitorA', 'VisitorB', NULL))
);

-- Tabla: Configuration (Configuración global del sistema)
CREATE TABLE [dbo].[Configuration] (
    [ConfigurationId] INT PRIMARY KEY IDENTITY(1,1),
    [Key] NVARCHAR(100) UNIQUE NOT NULL,
    [Value] NVARCHAR(500),
    [UpdatedAt] DATETIME2 DEFAULT GETUTCDATE()
);

-- ============================================================================
-- CREATE TRIGGER: Máximo 2 vehículos activos por miembro
-- ============================================================================
GO

CREATE TRIGGER [tr_MaxTwoActiveVehiclesPerMember]
ON [dbo].[Vehicles]
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Verificar si un miembro intenta tener más de 2 vehículos activos
    IF EXISTS (
        SELECT [MemberId]
        FROM [dbo].[Vehicles]
        WHERE [IsActiveForChampionship] = 1
        GROUP BY [MemberId]
        HAVING COUNT(*) > 2
    )
    BEGIN
        RAISERROR('Un miembro no puede tener más de 2 vehículos activos en el campeonato.', 16, 1);
        ROLLBACK TRANSACTION;
    END
END;
GO

-- ============================================================================
-- CREATE VIEWS
-- ============================================================================

-- Vista: Master Odometer Report
CREATE VIEW [dbo].[vw_MasterOdometerReport] AS
SELECT 
    v.[VehicleId],
    v.[MemberId],
    m.[ Complete Names] AS MemberName,
    v.[Lic Plate],
    v.[ Motorcycle Data],
    v.[Starting Odometer] AS [Starting Odometer Original],
    v.[Final Odometer] AS [Final Odometer Original],
    v.[OdometerUnit],
    CASE 
        WHEN v.[OdometerUnit] = 'Kilometers' THEN ROUND((v.[Final Odometer] - v.[Starting Odometer]) * 0.621371, 2)
        ELSE ROUND(v.[Final Odometer] - v.[Starting Odometer], 2)
    END AS [Total Miles Traveled],
    v.[Photography],
    v.[IsActiveForChampionship]
FROM [dbo].[Vehicles] v
INNER JOIN [dbo].[Members] m ON v.[MemberId] = m.[MemberId];
GO

-- Vista: Member General Ranking (Suma por miembro de todas las motos)
CREATE VIEW [dbo].[vw_MemberGeneralRanking] AS
SELECT 
    m.[MemberId],
    m.[ Complete Names],
    ROUND(SUM(
        CASE 
            WHEN v.[OdometerUnit] = 'Kilometers' THEN (v.[Final Odometer] - v.[Starting Odometer]) * 0.621371
            ELSE (v.[Final Odometer] - v.[Starting Odometer])
        END
    ), 2) AS [Total Miles All Vehicles],
    STRING_AGG(
        CONCAT(
            'Moto: ', v.[Lic Plate], ' (',
            CASE 
                WHEN v.[OdometerUnit] = 'Kilometers' THEN ROUND((v.[Final Odometer] - v.[Starting Odometer]) * 0.621371, 2)
                ELSE ROUND(v.[Final Odometer] - v.[Starting Odometer], 2)
            END,
            ' mi)'
        ),
        ' | '
    ) AS [Vehicles Breakdown],
    COUNT(CASE WHEN v.[IsActiveForChampionship] = 1 THEN 1 END) AS [Active Vehicles Count]
FROM [dbo].[Members] m
LEFT JOIN [dbo].[Vehicles] v ON m.[MemberId] = v.[MemberId]
GROUP BY m.[MemberId], m.[ Complete Names];
GO

-- ============================================================================
-- POPULATE CONFIGURATION (Valores por defecto)
-- ============================================================================

INSERT INTO [dbo].[Configuration] ([Key], [Value]) VALUES
    ('DistanceThreshold_1Point_OneWayMiles', '200'),
    ('DistanceThreshold_2Points_OneWayMiles', '800'),
    ('PointsPerClassMultiplier_1', '1'),
    ('PointsPerClassMultiplier_2', '3'),
    ('PointsPerClassMultiplier_3', '5'),
    ('PointsPerClassMultiplier_4', '10'),
    ('PointsPerClassMultiplier_5', '15'),
    ('VisitorBonus_SameContinent', '1'),
    ('VisitorBonus_DifferentContinent', '2'),
    ('TripWindowDays_ABInheritance', '15');
GO

-- ============================================================================
-- VERIFICACIÓN FINAL
-- ============================================================================

PRINT '✅ Schema creado exitosamente.';
PRINT '';
PRINT 'Tablas:';
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' ORDER BY TABLE_NAME;
PRINT '';
PRINT 'Vistas:';
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'VIEW' AND TABLE_SCHEMA = 'dbo';
PRINT '';
PRINT 'Configuración inicial:';
SELECT * FROM [dbo].[Configuration];
GO
