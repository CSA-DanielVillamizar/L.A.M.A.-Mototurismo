-- Crear tabla de valores válidos de STATUS
-- Basado en valores encontrados en los archivos Excel

IF OBJECT_ID('[dbo].[MemberStatusTypes]', 'U') IS NOT NULL 
    DROP TABLE [dbo].[MemberStatusTypes];
GO

CREATE TABLE [dbo].[MemberStatusTypes] (
    [StatusId] INT PRIMARY KEY IDENTITY(1,1),
    [StatusName] NVARCHAR(100) NOT NULL UNIQUE,
    [Category] NVARCHAR(50), -- MEMBER, OFFICER, PROSPECT, INACTIVE
    [IsActive] BIT DEFAULT 1,
    [DisplayOrder] INT,
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE()
);
GO

-- Insertar valores válidos normalizados
INSERT INTO [dbo].[MemberStatusTypes] ([StatusName], [Category], [DisplayOrder])
VALUES
    -- Oficiales del Chapter
    ('CHAPTER PRESIDENT', 'OFFICER', 1),
    ('CHAPTER VICEPRESIDENT', 'OFFICER', 2),
    ('CHAPTER SECRETARY', 'OFFICER', 3),
    ('CHAPTER TREASURER', 'OFFICER', 4),
    ('CHAPTER MTO', 'OFFICER', 5),
    ('CHAPTER BUSSINESS MANAGER', 'OFFICER', 6),
    
    -- Miembros activos
    ('FULL COLOR MEMBER', 'MEMBER', 10),
    
    -- Prospectos
    ('PROSPECT', 'PROSPECT', 20),
    ('ROCKET PROSPECT', 'PROSPECT', 21),
    
    -- Oficiales regionales
    ('REGIONAL PRESIDENT', 'OFFICER', 30),
    
    -- Estados generales
    ('ACTIVE', 'MEMBER', 40),
    ('INACTIVE', 'INACTIVE', 50);
GO

-- Normalizar valores existentes en Members
-- Actualizar variaciones de Full Color Member
UPDATE [dbo].[Members]
SET [STATUS] = 'FULL COLOR MEMBER'
WHERE [STATUS] IN ('FUL COLOR MEMBER', 'Full Color Member');

-- Actualizar variaciones de Chapter President
UPDATE [dbo].[Members]
SET [STATUS] = 'CHAPTER PRESIDENT'
WHERE [STATUS] IN ('Chapter President');

-- Actualizar variaciones de Chapter Vice-President
UPDATE [dbo].[Members]
SET [STATUS] = 'CHAPTER VICEPRESIDENT'
WHERE [STATUS] IN ('Chapter Vice-President', 'CHAPTER VICE-PRESIDEN');

-- Actualizar variaciones de Prospect
UPDATE [dbo].[Members]
SET [STATUS] = 'PROSPECT'
WHERE [STATUS] IN ('Prospect');

GO

-- Mostrar resumen después de normalización
SELECT [STATUS], COUNT(*) AS Cantidad
FROM [dbo].[Members]
GROUP BY [STATUS]
ORDER BY Cantidad DESC;
GO

PRINT 'Tabla MemberStatusTypes creada y valores normalizados.';
