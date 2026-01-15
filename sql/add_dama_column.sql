-- Agregar columna Dama a Members si no existe
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Members' AND COLUMN_NAME = 'Dama')
BEGIN
    ALTER TABLE [dbo].[Members]
    ADD [Dama] NVARCHAR(10);
    
    PRINT 'Columna [Dama] agregada a Members';
END
ELSE
BEGIN
    PRINT 'Columna [Dama] ya existe';
END
GO
