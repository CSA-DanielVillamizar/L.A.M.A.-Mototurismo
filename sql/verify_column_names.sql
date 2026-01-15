-- ============================================================================
-- AUDIT: Verificar nombres exactos de columnas en BD
-- Detectar si hay espacios iniciales en los nombres
-- ============================================================================

USE [LamaDb];
GO

PRINT '═══════════════════════════════════════════════════════════════════════';
PRINT 'AUDIT: NOMBRES DE COLUMNAS EN BD';
PRINT '═══════════════════════════════════════════════════════════════════════';
PRINT '';

-- Función auxiliar: mostrar caracteres especiales
PRINT '[Tabla: Members]';
SELECT 
    COLUMN_NAME,
    ASCII(LEFT(COLUMN_NAME, 1)) AS FirstCharASCII,
    CASE WHEN ASCII(LEFT(COLUMN_NAME, 1)) = 32 THEN '⚠️ ESPACIO INICIAL' 
         WHEN ASCII(LEFT(COLUMN_NAME, 1)) = 91 THEN '[corchete]' 
         ELSE 'Normal' END AS Estado,
    COLUMN_NAME AS [Nombre Exacto en BD]
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Members' AND TABLE_SCHEMA = 'dbo'
ORDER BY ORDINAL_POSITION;

PRINT '';
PRINT '[Tabla: Vehicles]';
SELECT 
    COLUMN_NAME,
    ASCII(LEFT(COLUMN_NAME, 1)) AS FirstCharASCII,
    CASE WHEN ASCII(LEFT(COLUMN_NAME, 1)) = 32 THEN '⚠️ ESPACIO INICIAL' 
         WHEN ASCII(LEFT(COLUMN_NAME, 1)) = 91 THEN '[corchete]' 
         ELSE 'Normal' END AS Estado,
    COLUMN_NAME AS [Nombre Exacto en BD]
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Vehicles' AND TABLE_SCHEMA = 'dbo'
ORDER BY ORDINAL_POSITION;

PRINT '';
PRINT '[Tabla: Events]';
SELECT 
    COLUMN_NAME,
    ASCII(LEFT(COLUMN_NAME, 1)) AS FirstCharASCII,
    CASE WHEN ASCII(LEFT(COLUMN_NAME, 1)) = 32 THEN '⚠️ ESPACIO INICIAL' 
         WHEN ASCII(LEFT(COLUMN_NAME, 1)) = 91 THEN '[corchete]' 
         ELSE 'Normal' END AS Estado,
    COLUMN_NAME AS [Nombre Exacto en BD]
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Events' AND TABLE_SCHEMA = 'dbo'
ORDER BY ORDINAL_POSITION;

PRINT '';
PRINT '[Tabla: Attendance]';
SELECT 
    COLUMN_NAME,
    ASCII(LEFT(COLUMN_NAME, 1)) AS FirstCharASCII,
    CASE WHEN ASCII(LEFT(COLUMN_NAME, 1)) = 32 THEN '⚠️ ESPACIO INICIAL' 
         WHEN ASCII(LEFT(COLUMN_NAME, 1)) = 91 THEN '[corchete]' 
         ELSE 'Normal' END AS Estado,
    COLUMN_NAME AS [Nombre Exacto en BD]
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Attendance' AND TABLE_SCHEMA = 'dbo'
ORDER BY ORDINAL_POSITION;

GO

-- Comparar con lo que debería estar (según Excel)
PRINT '';
PRINT '═══════════════════════════════════════════════════════════════════════';
PRINT 'ANÁLISIS: COLUMNAS QUE DEBEN TENER ESPACIO INICIAL (del Excel)';
PRINT '═══════════════════════════════════════════════════════════════════════';
PRINT '';
PRINT 'En Members:';
PRINT '  ✓ " Complete Names"  (con espacio inicial)';
PRINT '  ✓ " Country Birth"   (con espacio inicial)';
PRINT '  ✓ " In Lama Since"   (con espacio inicial)';
PRINT '';
PRINT 'En Vehicles:';
PRINT '  ✓ " Motorcycle Data" (con espacio inicial)';
PRINT '  ✓ " Lic Plate"       (con espacio inicial)';
PRINT '  ✓ " Starting Odometer"   (con espacio inicial)';
PRINT '  ✓ " Final Odometer"  (con espacio inicial)';
PRINT '';
PRINT 'En Events:';
PRINT '  ✓ " Event Start Date (AAAA/MM/DD)" (con espacio inicial)';
PRINT '  ✓ " Name of the event"  (con espacio inicial)';
PRINT '  ✓ " Mileage"  (con espacio inicial)';
PRINT '';
PRINT 'En Attendance:';
PRINT '  ✓ " Points per event"  (con espacio inicial)';
PRINT '  ✓ " Points per Distance"  (con espacio inicial)';
PRINT '  ✓ " Visitor Class"  (con espacio inicial)';
PRINT '';
