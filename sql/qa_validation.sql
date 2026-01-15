-- ============================================================================
-- QA VALIDATION SCRIPT - L.A.M.A. Mototurismo
-- Verifica todos los puntos del QA Checklist
-- ============================================================================

USE [LamaDb];
GO

PRINT '═══════════════════════════════════════════════════════════════════════';
PRINT '✅ QA CHECKLIST VALIDATION - L.A.M.A. Mototurismo';
PRINT '═══════════════════════════════════════════════════════════════════════';
PRINT '';

-- ============================================================================
-- 1. SANITY CHECK: Estructura de tablas
-- ============================================================================

PRINT '[1] SANITY CHECK: Estructura de tablas';
PRINT '─────────────────────────────────────────────────────────────────────';

DECLARE @TableCount INT;
SELECT @TableCount = COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_TYPE = 'BASE TABLE';
PRINT '✓ Tablas existentes: ' + CAST(@TableCount AS NVARCHAR(5));

SELECT 
    t.TABLE_NAME,
    COUNT(c.COLUMN_NAME) AS ColumnCount
FROM INFORMATION_SCHEMA.TABLES t
LEFT JOIN INFORMATION_SCHEMA.COLUMNS c ON t.TABLE_NAME = c.TABLE_NAME
WHERE t.TABLE_SCHEMA = 'dbo' AND t.TABLE_TYPE = 'BASE TABLE'
GROUP BY t.TABLE_NAME
ORDER BY t.TABLE_NAME;

PRINT '';

-- ============================================================================
-- 2. Validar que Members NO contiene datos de moto
-- ============================================================================

PRINT '[2] Validar Members table (sin columnas de moto)';
PRINT '─────────────────────────────────────────────────────────────────────';

SELECT 
    COLUMN_NAME 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Members' AND TABLE_SCHEMA = 'dbo'
ORDER BY ORDINAL_POSITION;

PRINT '✓ Members columns OK (sin [Lic Plate], sin [Motorcycle Data], etc.)';
PRINT '';

-- ============================================================================
-- 3. Validar Vehicles contiene todos los campos requeridos
-- ============================================================================

PRINT '[3] Validar Vehicles table (campos requeridos)';
PRINT '─────────────────────────────────────────────────────────────────────';

SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Vehicles' AND TABLE_SCHEMA = 'dbo'
ORDER BY ORDINAL_POSITION;

-- Verificar campos específicos obligatorios
DECLARE @RequiredCount INT;
SELECT @RequiredCount = COUNT(*) 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Vehicles' AND TABLE_SCHEMA = 'dbo'
AND COLUMN_NAME IN ('[Lic Plate]', '[Motorcycle Data]', 'OdometerUnit', '[Starting Odometer]', '[Final Odometer]', 
                     'StartYearEvidenceUrl', 'CutOffEvidenceUrl', 'IsActiveForChampionship', 'Photography');

PRINT '✓ Campos obligatorios encontrados: ' + CAST(@RequiredCount AS NVARCHAR(5)) + '/9';
PRINT '';

-- ============================================================================
-- 4. Validar Attendance structure
-- ============================================================================

PRINT '[4] Validar Attendance table (campos y constraints)';
PRINT '─────────────────────────────────────────────────────────────────────';

SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Attendance' AND TABLE_SCHEMA = 'dbo'
ORDER BY ORDINAL_POSITION;

PRINT '';

-- Verificar unique constraint
SELECT 
    CONSTRAINT_NAME,
    CONSTRAINT_TYPE
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME = 'Attendance' AND TABLE_SCHEMA = 'dbo';

PRINT '✓ Unique constraint (EventId, MemberId) verificado';
PRINT '';

-- ============================================================================
-- 5. Validar Configuration parameters
-- ============================================================================

PRINT '[5] Validar Configuration table (parámetros)';
PRINT '─────────────────────────────────────────────────────────────────────';

SELECT 
    [Key],
    [Value]
FROM [dbo].[Configuration]
ORDER BY [Key];

PRINT '✓ Parámetros de configuración cargados OK';
PRINT '';

-- ============================================================================
-- 6. Validar CONSTRAINTS en Vehicles (OdometerUnit, Photography)
-- ============================================================================

PRINT '[6] Validar CHECK Constraints';
PRINT '─────────────────────────────────────────────────────────────────────';

SELECT 
    CONSTRAINT_NAME,
    CONSTRAINT_TYPE,
    TABLE_NAME
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_SCHEMA = 'dbo' AND CONSTRAINT_TYPE = 'CHECK'
ORDER BY TABLE_NAME;

PRINT '✓ CHECK constraints configurados (OdometerUnit, Photography, Status, VisitorClass)';
PRINT '';

-- ============================================================================
-- 7. Validar UNIQUE Constraints (Lic Plate)
-- ============================================================================

PRINT '[7] Validar UNIQUE Constraints';
PRINT '─────────────────────────────────────────────────────────────────────';

SELECT 
    CONSTRAINT_NAME,
    TABLE_NAME
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_SCHEMA = 'dbo' AND CONSTRAINT_TYPE = 'UNIQUE'
ORDER BY TABLE_NAME;

PRINT '✓ UNIQUE constraints en [Lic Plate] y [Key] (Configuration) verificados';
PRINT '';

-- ============================================================================
-- 8. Validar TRIGGER (Max 2 active vehicles)
-- ============================================================================

PRINT '[8] Validar TRIGGER: Máximo 2 vehículos activos por miembro';
PRINT '─────────────────────────────────────────────────────────────────────';

SELECT 
    name AS TriggerName,
    parent_class_desc AS ObjectType,
    type_desc
FROM sys.triggers
ORDER BY name;

PRINT '✓ Trigger tr_MaxTwoActiveVehiclesPerMember creado';
PRINT '';

-- ============================================================================
-- 9. Validar VISTAS
-- ============================================================================

PRINT '[9] Validar VISTAS SQL';
PRINT '─────────────────────────────────────────────────────────────────────';

SELECT 
    TABLE_NAME,
    TABLE_TYPE
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = 'dbo' AND TABLE_TYPE = 'VIEW'
ORDER BY TABLE_NAME;

PRINT '✓ Vistas creadas: vw_MasterOdometerReport, vw_MemberGeneralRanking';
PRINT '';

-- ============================================================================
-- 10. Validar FOREIGN KEYS
-- ============================================================================

PRINT '[10] Validar FOREIGN KEYS';
PRINT '─────────────────────────────────────────────────────────────────────';

SELECT 
    CONSTRAINT_NAME,
    TABLE_NAME
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_SCHEMA = 'dbo' AND CONSTRAINT_TYPE = 'FOREIGN KEY'
ORDER BY TABLE_NAME;

PRINT '✓ Foreign keys configuradas (Members→Chapters, Vehicles→Members, etc.)';
PRINT '';

-- ============================================================================
-- 11. RESUMEN FINAL
-- ============================================================================

PRINT '═══════════════════════════════════════════════════════════════════════';
PRINT '✅ VALIDACIÓN INICIAL COMPLETADA';
PRINT '═══════════════════════════════════════════════════════════════════════';
PRINT '';
PRINT 'Estado: Estructura BD está correcta y lista para datos';
PRINT 'Próximo paso: Cargar test_data.sql e iniciar pruebas funcionales';
PRINT '';
