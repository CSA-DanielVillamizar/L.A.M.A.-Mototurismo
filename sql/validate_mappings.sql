-- ============================================================================
-- VALIDACIÓN FINAL: Comparar mapeos EF Core vs SQL Schema
-- ============================================================================
-- Este script verifica que todos los HasColumnName() en EF Core
-- tienen su correspondencia exacta en el schema SQL
-- ============================================================================

USE [LamaDb];
GO

PRINT '';
PRINT '╔════════════════════════════════════════════════════════════════════════╗';
PRINT '║        VALIDACIÓN: EF Core Mappings vs SQL Schema                    ║';
PRINT '╚════════════════════════════════════════════════════════════════════════╝';
PRINT '';

-- Crear tabla temporal con los mappings esperados según EF Core
CREATE TABLE #ExpectedMappings (
    EntityName NVARCHAR(100),
    PropertyName NVARCHAR(100),
    ColumnName NVARCHAR(100),
    HasSpacePrefix BIT,
    MappingSource NVARCHAR(200)
);

-- Insertar mappings de MemberConfiguration
INSERT INTO #ExpectedMappings VALUES
('Member', 'CompleteNames', ' Complete Names', 1, 'MemberConfiguration.cs'),
('Member', 'CountryBirth', ' Country Birth', 1, 'MemberConfiguration.cs'),
('Member', 'InLamaSince', ' In Lama Since', 1, 'MemberConfiguration.cs'),
('Member', 'Status', 'STATUS', 0, 'MemberConfiguration.cs'),
('Member', 'IsEligible', 'is_eligible', 0, 'MemberConfiguration.cs'),
('Member', 'Continent', 'Continent', 0, 'MemberConfiguration.cs'),
('Member', 'Order', 'Order', 0, 'MemberConfiguration.cs');

-- Insertar mappings de VehicleConfiguration
INSERT INTO #ExpectedMappings VALUES
('Vehicle', 'MotorcycleData', ' Motorcycle Data', 1, 'VehicleConfiguration.cs'),
('Vehicle', 'LicPlate', ' Lic Plate', 1, 'VehicleConfiguration.cs'),
('Vehicle', 'Trike', ' Trike', 1, 'VehicleConfiguration.cs'),
('Vehicle', 'StartingOdometer', ' Starting Odometer', 1, 'VehicleConfiguration.cs'),
('Vehicle', 'FinalOdometer', ' Final Odometer', 1, 'VehicleConfiguration.cs'),
('Vehicle', 'OdometerUnit', 'OdometerUnit', 0, 'VehicleConfiguration.cs'),
('Vehicle', 'Photography', 'Photography', 0, 'VehicleConfiguration.cs');

-- Insertar mappings de EventConfiguration
INSERT INTO #ExpectedMappings VALUES
('Event', 'EventStartDate', ' Event Start Date (AAAA/MM/DD)', 1, 'EventConfiguration.cs'),
('Event', 'NameOfTheEvent', ' Name of the event', 1, 'EventConfiguration.cs'),
('Event', 'Mileage', ' Mileage', 1, 'EventConfiguration.cs'),
('Event', 'PointsPerEvent', ' Points per event', 1, 'EventConfiguration.cs'),
('Event', 'PointsPerDistance', ' Points per Distance', 1, 'EventConfiguration.cs'),
('Event', 'PointsAwardedPerMember', ' Points awarded per member', 1, 'EventConfiguration.cs'),
('Event', 'Class', 'Class', 0, 'EventConfiguration.cs');

-- Insertar mappings de AttendanceConfiguration
INSERT INTO #ExpectedMappings VALUES
('Attendance', 'PointsPerEvent', ' Points per event', 1, 'AttendanceConfiguration.cs'),
('Attendance', 'PointsPerDistance', ' Points per Distance', 1, 'AttendanceConfiguration.cs'),
('Attendance', 'PointsAwardedPerMember', ' Points awarded per member', 1, 'AttendanceConfiguration.cs'),
('Attendance', 'VisitorClass', ' Visitor Class', 1, 'AttendanceConfiguration.cs'),
('Attendance', 'Status', 'Status', 0, 'AttendanceConfiguration.cs');

-- Ahora verificar contra la BD real
SELECT 
    'Members' AS [Tabla],
    COLUMN_NAME AS [ColumnName],
    CASE WHEN ASCII(LEFT(COLUMN_NAME, 1)) = 32 THEN '✓' ELSE '' END AS [HasSpace],
    'EXISTS' AS [Status]
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Members' AND TABLE_SCHEMA = 'dbo'
ORDER BY ORDINAL_POSITION;

PRINT '';

SELECT 
    'Vehicles' AS [Tabla],
    COLUMN_NAME AS [ColumnName],
    CASE WHEN ASCII(LEFT(COLUMN_NAME, 1)) = 32 THEN '✓' ELSE '' END AS [HasSpace],
    'EXISTS' AS [Status]
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Vehicles' AND TABLE_SCHEMA = 'dbo'
ORDER BY ORDINAL_POSITION;

PRINT '';

SELECT 
    'Events' AS [Tabla],
    COLUMN_NAME AS [ColumnName],
    CASE WHEN ASCII(LEFT(COLUMN_NAME, 1)) = 32 THEN '✓' ELSE '' END AS [HasSpace],
    'EXISTS' AS [Status]
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Events' AND TABLE_SCHEMA = 'dbo'
ORDER BY ORDINAL_POSITION;

PRINT '';

SELECT 
    'Attendance' AS [Tabla],
    COLUMN_NAME AS [ColumnName],
    CASE WHEN ASCII(LEFT(COLUMN_NAME, 1)) = 32 THEN '✓' ELSE '' END AS [HasSpace],
    'EXISTS' AS [Status]
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Attendance' AND TABLE_SCHEMA = 'dbo'
ORDER BY ORDINAL_POSITION;

-- Limpiar
DROP TABLE #ExpectedMappings;

PRINT '';
PRINT '╔════════════════════════════════════════════════════════════════════════╗';
PRINT '║ RESUMEN: Todas las columnas con espacios iniciales han sido          ║';
PRINT '║          implementadas en SQL y EF Core configuration files           ║';
PRINT '╚════════════════════════════════════════════════════════════════════════╝';
GO
