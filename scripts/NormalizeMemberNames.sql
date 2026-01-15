-- ==============================================================================
-- Script SQL para Normalizar Datos de Miembros (Performance Optimization)
-- ==============================================================================
-- Propósito: Poblar la columna CompleteNamesNormalized con valores normalizados
--            (sin tildes, mayúsculas, espacios trimmed) para búsquedas rápidas
--
-- Ejecutar DESPUÉS de aplicar migración: AddCompleteNamesNormalized
-- Tiempo estimado: ~2 segundos para 4,000 registros
-- ==============================================================================

USE LamaDb;
GO

-- Verificar que la columna existe
IF NOT EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID('Members') 
    AND name = 'CompleteNamesNormalized'
)
BEGIN
    RAISERROR('Error: La columna CompleteNamesNormalized no existe. Ejecutar migración primero.', 16, 1);
    RETURN;
END
GO

-- Verificar que el índice existe
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes 
    WHERE object_id = OBJECT_ID('Members') 
    AND name = 'IX_Members_CompleteNamesNormalized'
)
BEGIN
    RAISERROR('Advertencia: El índice IX_Members_CompleteNamesNormalized no existe.', 10, 1);
END
GO

PRINT '========================================';
PRINT 'Iniciando normalización de nombres...';
PRINT '========================================';

-- Mostrar estadísticas previas
DECLARE @TotalMembers INT = (SELECT COUNT(*) FROM Members);
DECLARE @NullNormalized INT = (SELECT COUNT(*) FROM Members WHERE CompleteNamesNormalized IS NULL);

PRINT '';
PRINT 'Estadísticas:';
PRINT '  Total de miembros: ' + CAST(@TotalMembers AS VARCHAR);
PRINT '  Sin normalizar: ' + CAST(@NullNormalized AS VARCHAR);
PRINT '';

-- ==============================================================================
-- NORMALIZACIÓN DE NOMBRES
-- ==============================================================================
-- Estrategia:
--   1. Reemplazar caracteres con tildes por sus equivalentes sin tildes
--   2. Convertir a mayúsculas (UPPER)
--   3. Trimear espacios (LTRIM/RTRIM)
--
-- Tildes soportadas: á, é, í, ó, ú, ñ, ü (minúsculas y mayúsculas)
-- ==============================================================================

BEGIN TRANSACTION;

BEGIN TRY
    UPDATE Members
    SET CompleteNamesNormalized = LTRIM(RTRIM(UPPER(
        TRANSLATE(
            [ Complete Names],
            'áéíóúñüÁÉÍÓÚÑÜàèìòùÀÈÌÒÙäëïö',
            'aeiounuAEIOUNUaeiouAEIOUaeio'
        )
    )))
    WHERE CompleteNamesNormalized IS NULL
       OR CompleteNamesNormalized = ''
       OR CompleteNamesNormalized <> LTRIM(RTRIM(UPPER([ Complete Names])));

    DECLARE @RowsUpdated INT = @@ROWCOUNT;

    COMMIT TRANSACTION;

    PRINT '';
    PRINT '========================================';
    PRINT 'Normalización completada exitosamente';
    PRINT '========================================';
    PRINT '  Filas actualizadas: ' + CAST(@RowsUpdated AS VARCHAR);
    PRINT '';

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    
    PRINT '';
    PRINT '========================================';
    PRINT 'ERROR en normalización';
    PRINT '========================================';
    PRINT 'Error: ' + ERROR_MESSAGE();
    PRINT 'Línea: ' + CAST(ERROR_LINE() AS VARCHAR);
    PRINT '';
    
    THROW;
END CATCH;

-- ==============================================================================
-- VERIFICACIÓN DE RESULTADOS
-- ==============================================================================

PRINT 'Verificando resultados...';
PRINT '';

-- Verificar que no queden registros sin normalizar
DECLARE @StillNull INT = (SELECT COUNT(*) FROM Members WHERE CompleteNamesNormalized IS NULL);

IF @StillNull > 0
BEGIN
    PRINT 'ADVERTENCIA: ' + CAST(@StillNull AS VARCHAR) + ' registros aún sin normalizar.';
    PRINT 'Revisar registros con [Complete Names] = NULL o vacío.';
END
ELSE
BEGIN
    PRINT 'ÉXITO: Todos los registros normalizados correctamente.';
END

PRINT '';

-- Mostrar ejemplos de normalización (primeros 10)
PRINT 'Ejemplos de normalización (primeros 10):';
PRINT '----------------------------------------';

SELECT TOP 10
    Id,
    [ Complete Names] AS Original,
    CompleteNamesNormalized AS Normalizado
FROM Members
ORDER BY Id;

PRINT '';
PRINT '========================================';
PRINT 'Script completado';
PRINT '========================================';
PRINT 'Siguiente paso: Reiniciar aplicación para usar índice';
PRINT '';

GO
