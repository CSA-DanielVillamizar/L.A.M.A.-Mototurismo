-- ============================================================================
-- AUDITORÍA: Encabezados REALES de Excel vs BD
-- ============================================================================
-- Este script valida que los nombres de columnas en la BD coinciden exactamente
-- con los encabezados del archivo Excel original
-- ============================================================================

USE [LamaDb];
GO

PRINT '╔════════════════════════════════════════════════════════════════════════╗';
PRINT '║           AUDITORÍA: ENCABEZADOS EXCEL vs BASE DE DATOS              ║';
PRINT '╚════════════════════════════════════════════════════════════════════════╝';
PRINT '';

-- Función auxiliar: detectar espacios iniciales
PRINT 'TABLA: Members';
PRINT '─────────────────────────────────────────────────────────────────────────';
SELECT 
    ROW_NUMBER() OVER (ORDER BY ORDINAL_POSITION) AS [#],
    COLUMN_NAME AS [Nombre],
    DATA_TYPE,
    ASCII(LEFT(COLUMN_NAME, 1)) AS [ASCII_First],
    CASE 
        WHEN ASCII(LEFT(COLUMN_NAME, 1)) = 32 THEN '⚠️ ESPACIO INICIAL'
        ELSE '✓ Normal'
    END AS [Tipo]
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Members' AND TABLE_SCHEMA = 'dbo'
ORDER BY ORDINAL_POSITION;

PRINT '';
PRINT 'TABLA: Vehicles';
PRINT '─────────────────────────────────────────────────────────────────────────';
SELECT 
    ROW_NUMBER() OVER (ORDER BY ORDINAL_POSITION) AS [#],
    COLUMN_NAME AS [Nombre],
    DATA_TYPE,
    ASCII(LEFT(COLUMN_NAME, 1)) AS [ASCII_First],
    CASE 
        WHEN ASCII(LEFT(COLUMN_NAME, 1)) = 32 THEN '⚠️ ESPACIO INICIAL'
        ELSE '✓ Normal'
    END AS [Tipo]
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Vehicles' AND TABLE_SCHEMA = 'dbo'
ORDER BY ORDINAL_POSITION;

PRINT '';
PRINT 'TABLA: Events';
PRINT '─────────────────────────────────────────────────────────────────────────';
SELECT 
    ROW_NUMBER() OVER (ORDER BY ORDINAL_POSITION) AS [#],
    COLUMN_NAME AS [Nombre],
    DATA_TYPE,
    ASCII(LEFT(COLUMN_NAME, 1)) AS [ASCII_First],
    CASE 
        WHEN ASCII(LEFT(COLUMN_NAME, 1)) = 32 THEN '⚠️ ESPACIO INICIAL'
        ELSE '✓ Normal'
    END AS [Tipo]
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Events' AND TABLE_SCHEMA = 'dbo'
ORDER BY ORDINAL_POSITION;

PRINT '';
PRINT 'TABLA: Attendance';
PRINT '─────────────────────────────────────────────────────────────────────────';
SELECT 
    ROW_NUMBER() OVER (ORDER BY ORDINAL_POSITION) AS [#],
    COLUMN_NAME AS [Nombre],
    DATA_TYPE,
    ASCII(LEFT(COLUMN_NAME, 1)) AS [ASCII_First],
    CASE 
        WHEN ASCII(LEFT(COLUMN_NAME, 1)) = 32 THEN '⚠️ ESPACIO INICIAL'
        ELSE '✓ Normal'
    END AS [Tipo]
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Attendance' AND TABLE_SCHEMA = 'dbo'
ORDER BY ORDINAL_POSITION;

GO

PRINT '';
PRINT '╔════════════════════════════════════════════════════════════════════════╗';
PRINT '║                     RESUMEN DE VALIDACIÓN                             ║';
PRINT '╚════════════════════════════════════════════════════════════════════════╝';
PRINT '';
PRINT 'COLUMNAS CON ESPACIO INICIAL (según BD):';
SELECT 
    CONCAT(TABLE_NAME, '.', COLUMN_NAME) AS [ColumnPath]
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'dbo' 
  AND ASCII(LEFT(COLUMN_NAME, 1)) = 32
ORDER BY TABLE_NAME, ORDINAL_POSITION;

PRINT '';
PRINT 'Total columnas con espacio inicial:';
SELECT COUNT(*) AS [Total]
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'dbo' 
  AND ASCII(LEFT(COLUMN_NAME, 1)) = 32;

GO
